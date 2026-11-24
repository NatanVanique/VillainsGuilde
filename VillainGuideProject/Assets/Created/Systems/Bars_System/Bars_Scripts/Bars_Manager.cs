using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public sealed class Bars_Manager : MonoBehaviour
{
    private const float BackgroundOpacity = 218f / 255f;

    private readonly HashSet<GameObject> openPopups = new();
    private VisualReferencer visual;
    private Coroutine movementBars;
    private bool targetVisible;
    private float progress;

    public event Action<bool> VisibilityChanged;

    private void OnEnable()
    {
        UpdateVisibility();
    }

    private void Start()
    {
        UpdateVisibility();
    }

    private void OnDisable()
    {
        if (movementBars != null)
        {
            StopCoroutine(movementBars);
            movementBars = null;
        }

        if (visual != null)
        {
            ResetAppearance();
        }
    }

    public void ShowBars() => SetPopupOpen(gameObject, true);

    public void HideBars() => SetPopupOpen(gameObject, false);

    public void SetPopupOpen(GameObject popup, bool isOpen)
    {
        if (popup == null)
        {
            return;
        }

        if (isOpen)
        {
            openPopups.Add(popup);
        }
        else
        {
            openPopups.Remove(popup);
        }

        UpdateVisibility();
    }

    private void UpdateVisibility()
    {
        if (!isActiveAndEnabled)
        {
            return;
        }

        if (visual == null)
        {
            Main_Referencer references = Main_Referencer.Instance;
            if (references == null || references.visual == null)
            {
                return;
            }

            visual = references.visual;
            ResetAppearance();
        }

        bool visible = openPopups.Count > 0;
        if (visible == targetVisible)
        {
            return;
        }

        targetVisible = visible;
        if (movementBars != null)
        {
            StopCoroutine(movementBars);
        }

        if (visible)
        {
            visual.barsCanvas_Bars.SetActive(true);
            visual.background_Bars.gameObject.SetActive(true);
        }

        movementBars = StartCoroutine(MoveBars(visible));
        VisibilityChanged?.Invoke(visible);
    }

    private IEnumerator MoveBars(bool visible)
    {
        float targetProgress = visible ? 1f : 0f;
        float speed = Mathf.Max(0.01f, Main_Referencer.Instance.value.velocity_Bars);
        do
        {
            // The journal pauses gameplay while its UI is open.
            yield return null;
            SetProgress(Mathf.MoveTowards(progress, targetProgress, speed * Time.unscaledDeltaTime));
        }
        while (progress != targetProgress);

        if (!visible)
        {
            ResetAppearance();
        }

        movementBars = null;
    }

    private void ResetAppearance()
    {
        targetVisible = false;
        SetProgress(0f);
        visual.background_Bars.gameObject.SetActive(false);
        visual.barsCanvas_Bars.SetActive(false);
    }

    private void SetProgress(float value)
    {
        progress = value;
        SetHeight(visual.top_Bars.transform, value);
        SetHeight(visual.bottom_Bars.transform, value);

        Color color = visual.background_Bars.color;
        color.a = value * BackgroundOpacity;
        visual.background_Bars.color = color;
    }

    private static void SetHeight(Transform bar, float height)
    {
        Vector3 scale = bar.localScale;
        scale.y = height;
        bar.localScale = scale;
    }
}
