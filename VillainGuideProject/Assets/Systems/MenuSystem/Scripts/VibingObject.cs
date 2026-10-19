using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading;
using FMOD;
using FMOD.Studio;

public class VibingObject : MonoBehaviour
{
    [Header("Music")]
    [SerializeField] private string musicEvent = "MainMusic";
    [SerializeField] private string destinationMarkerName = "Beat";

    [Header("Values")]
    [SerializeField] private float pulseScale = 1.1f;
    [SerializeField] private float pulseSpeed = 8f;
    private Vector3 initialScale;
    private float currentScale = 1f;
    private static readonly EVENT_CALLBACK callback = OnFmodCallback;
    private static readonly object callbackLock = new object();
    private static readonly Dictionary<IntPtr, List<VibingObject>> listeners = new Dictionary<IntPtr, List<VibingObject>>();
    private EventInstance attachedInstance;
    private int callbackHandleId = -1;
    private int pendingPulses;

    void Start()
    {
        initialScale = transform.localScale;
    }

    void Update()
    {
        FmodPlaybackState state = FmodB8.GetState(musicEvent);
        if (state == FmodPlaybackState.Stopped || state == FmodPlaybackState.Stopping)
            return;

        TryAttachToCurrentInstance();

        if (Interlocked.Exchange(ref pendingPulses, 0) > 0)
            Pulse();

        currentScale = Mathf.MoveTowards(
            currentScale,
            1f,
            pulseSpeed * Time.deltaTime
        );

        transform.localScale = initialScale * currentScale;
    }

    private void TryAttachToCurrentInstance()
    {
        FmodCommands commands = FmodCommands.EnsureInstance();
        FieldInfo instancesField = typeof(FmodCommands).GetField(
            "instancesByHandle", BindingFlags.Instance | BindingFlags.NonPublic);
        if (instancesField == null)
            return;

        var instances = instancesField.GetValue(commands) as IDictionary;
        if (instances == null)
            return;

        object newest = null;
        int newestHandleId = -1;
        foreach (object instance in instances.Values)
        {
            Type instanceType = instance.GetType();
            const BindingFlags propertyFlags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;
            string eventId = (string)instanceType.GetProperty("EventId", propertyFlags).GetValue(instance);
            int handleId = (int)instanceType.GetProperty("HandleId", propertyFlags).GetValue(instance);
            if (eventId != musicEvent || handleId <= callbackHandleId)
                continue;
            if (newest == null || handleId > newestHandleId)
            {
                newest = instance;
                newestHandleId = handleId;
            }
        }

        if (newest == null)
            return;

        EventInstance eventInstance = (EventInstance)newest.GetType()
            .GetProperty("Instance", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
            .GetValue(newest);
        if (!eventInstance.isValid())
            return;

        DetachCallback();
        lock (callbackLock)
        {
            if (!listeners.TryGetValue(eventInstance.handle, out List<VibingObject> subscribers))
            {
                subscribers = new List<VibingObject>();
                listeners.Add(eventInstance.handle, subscribers);
                if (eventInstance.setCallback(callback, EVENT_CALLBACK_TYPE.TIMELINE_MARKER) != RESULT.OK)
                {
                    listeners.Remove(eventInstance.handle);
                    return;
                }
            }
            subscribers.Add(this);
        }
        attachedInstance = eventInstance;
        callbackHandleId = newestHandleId;
    }

    [AOT.MonoPInvokeCallback(typeof(EVENT_CALLBACK))]
    private static RESULT OnFmodCallback(EVENT_CALLBACK_TYPE type, IntPtr instance, IntPtr parameters)
    {
        if (type != EVENT_CALLBACK_TYPE.TIMELINE_MARKER)
            return RESULT.OK;

        TIMELINE_MARKER_PROPERTIES marker = Marshal.PtrToStructure<TIMELINE_MARKER_PROPERTIES>(parameters);
        string markerName = marker.name;
        lock (callbackLock)
        {
            if (listeners.TryGetValue(instance, out List<VibingObject> subscribers))
            {
                foreach (VibingObject subscriber in subscribers)
                {
                    if (markerName == subscriber.destinationMarkerName)
                        Interlocked.Increment(ref subscriber.pendingPulses);
                }
            }
        }

        return RESULT.OK;
    }

    public void Pulse()
    {
        currentScale = pulseScale;
        transform.localScale = initialScale * currentScale;
    }

    private void DetachCallback()
    {
        if (!attachedInstance.hasHandle())
            return;

        bool unregister = false;
        lock (callbackLock)
        {
            if (listeners.TryGetValue(attachedInstance.handle, out List<VibingObject> subscribers))
            {
                subscribers.Remove(this);
                if (subscribers.Count == 0)
                {
                    listeners.Remove(attachedInstance.handle);
                    unregister = true;
                }
            }
        }

        if (unregister && attachedInstance.isValid())
        {
            attachedInstance.setCallback(null, EVENT_CALLBACK_TYPE.TIMELINE_MARKER);
            if (attachedInstance.getSystem(out FMOD.Studio.System system) == RESULT.OK)
                system.flushCommands();
        }
        attachedInstance.clearHandle();
        callbackHandleId = -1;
    }

    void OnDisable()
    {
        DetachCallback();
        transform.localScale = initialScale;
        currentScale = 1f;
        callbackHandleId = -1;
        Interlocked.Exchange(ref pendingPulses, 0);
    }
}
