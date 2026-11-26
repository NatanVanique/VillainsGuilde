using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using Random = UnityEngine.Random;

public sealed class TelephoneManager : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private int lastRouteQuestDay = -1;
    private bool isMouseOver;
    private Coroutine tickRoutine;
    private Coroutine ringRoutine;
    private double nextTickAt;
    private double ringEndsAt;

    private float TickInterval => Mathf.Max(0f,
        Main_Referencer.Instance.value.dayInMinutes_HourValues * 60f / 24f *
        Main_Referencer.Instance.value.hoursToTryTick_TelephoneValues);
    
    private void Start()
    {
        Main_Referencer.Instance.visual.telephoneAnimator_Telephone.SetBool("Ringing", false);
        SetPhoneState(isOpen: false);
        tickRoutine = StartCoroutine(TickTelephone(TickInterval));
    }

    internal void CaptureState(GameplaySaveData data)
    {
        data.lastRouteQuestDay = lastRouteQuestDay;
        data.secondsToPhoneTick = SecondsRemaining(nextTickAt);
        data.phoneOpen = Main_Referencer.Instance.visual.openPhone.activeSelf;
        data.phoneRinging = Main_Referencer.Instance.value.telephoneRinging;
        data.ringSecondsRemaining = ringEndsAt < 0 ? -1f : SecondsRemaining(ringEndsAt);
    }

    internal void RestoreState(GameplaySaveData data)
    {
        if (tickRoutine != null) StopCoroutine(tickRoutine);
        if (ringRoutine != null) StopCoroutine(ringRoutine);

        lastRouteQuestDay = data.lastRouteQuestDay;
        tickRoutine = StartCoroutine(TickTelephone(data.secondsToPhoneTick));
        SetPhoneState(data.phoneOpen);
        Main_Referencer.Instance.value.telephoneRinging = data.phoneRinging;
        Main_Referencer.Instance.visual.telephoneAnimator_Telephone.SetBool("Ringing", data.phoneRinging);
        ringRoutine = data.phoneRinging ? StartCoroutine(StopRingingAfter(data.ringSecondsRemaining)) : null;
    }

    private static float SecondsRemaining(double deadline)
    {
        return Mathf.Max(0f, (float)(deadline - Time.timeAsDouble));
    }
    public void OnPointerEnter(PointerEventData eventData)
    {
        isMouseOver = true;
        UpdatePhoneOutline();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        isMouseOver = false;
        UpdatePhoneOutline();
    }

    private IEnumerator TickTelephone(float delay)
    {
        while (true)
        {
            nextTickAt = Time.timeAsDouble + delay;
            yield return new WaitForSeconds(delay);
            TryRinging();
            delay = TickInterval;
        }
    }

    private void TryRinging()
    {
        int currentHour = Main_Referencer.Instance.value.currentHour_HourValues;

        float chance = Main_Referencer.Instance.value.callChancePerHour_TelephoneValues
            .Evaluate(currentHour / 24f);

        if (chance <= 0f)
        {
            return;
        }

        if (RouteDayVerify())
        {
            RingingTelephone();
            return;
        }

        int tries = Mathf.CeilToInt(
            (float)Main_Referencer.Instance.value.popularityLevel_Popularity /
            Main_Referencer.Instance.value.popularitysToNewTry_TelephoneValues);

        for (int i = 0; i < tries; i++)
        {
            if (Random.value < chance)
            {
                RingingTelephone();
                break;
            }
        }
    }

    private void RingingTelephone()
    {
        Main_Referencer references = Main_Referencer.Instance;
        CreateQuest_Data.QuestType type = RouteDayVerify()
            ? CreateQuest_Data.QuestType.Route
            : CreateQuest_Data.QuestType.Single;
        if (!references.manager.questManager.AddQuest(type))
            return;

        if (type == CreateQuest_Data.QuestType.Route)
            lastRouteQuestDay = references.value.currentDay_HourValues;

        var animator = references.visual.telephoneAnimator_Telephone;

        if (ringRoutine != null) StopCoroutine(ringRoutine);

        animator.SetBool("Ringing", true);

        references.value.telephoneRinging = true;

        ringRoutine = StartCoroutine(StopRingingAfter(-1f));
        GameSaveSystem.RequestAutosave();
    }

    private bool RouteDayVerify()
    {
        int currentDay = Main_Referencer.Instance.value.currentDay_HourValues;
        int startDay = Main_Referencer.Instance.value.daysToStartRouteQuest_QuestValues;

        int levelPerQuest = Mathf.Max(1, Mathf.CeilToInt(
            (float)Main_Referencer.Instance.value.maxPopularityLevel_Popularity /
            Main_Referencer.Instance.quest.routeQuests.Length
        ));

        return currentDay > startDay &&
               (currentDay - startDay) % levelPerQuest == 0 &&
               lastRouteQuestDay != currentDay;
    }   
    
    private IEnumerator StopRingingAfter(float delay)
    {
        var animator = Main_Referencer.Instance.visual.telephoneAnimator_Telephone;

        if (delay < 0f)
        {
            // The Animator enters the ringing state on its next update.
            ringEndsAt = -1;
            yield return null;
            float duration = animator.GetCurrentAnimatorStateInfo(0).length;
            int repeats = Mathf.Max(1, Main_Referencer.Instance.value.ringingRepeats_TelephoneValues);
            delay = duration * repeats;
        }

        ringEndsAt = Time.timeAsDouble + delay;
        yield return new WaitForSeconds(delay);

        animator.SetBool("Ringing", false);
        Main_Referencer.Instance.value.telephoneRinging = false;
        ringRoutine = null;
    }
    
    private void UpdatePhoneOutline()
    {
        var visual = Main_Referencer.Instance.visual;
        visual.openPhoneOutline.SetActive(isMouseOver && visual.openPhone.activeSelf);

        foreach (var outline in visual.closedPhoneOutlines)
        {
            outline.SetActive(isMouseOver);
        }
    }
    
    public void SetPhoneState(bool isOpen)
    {
        var buttonTelephone = Main_Referencer.Instance.visual.phoneButton;
        var questManager = Main_Referencer.Instance.manager.questManager;

        buttonTelephone.onClick.RemoveAllListeners();

        if (!isOpen)
            buttonTelephone.onClick.AddListener(questManager.OpenVoiceMail);
        else
            buttonTelephone.onClick.AddListener(questManager.CloseVoiceMail);

        Main_Referencer.Instance.visual.openPhone.SetActive(isOpen);
        Main_Referencer.Instance.visual.closedPhone.SetActive(!isOpen);

        UpdatePhoneOutline();
    }
}
