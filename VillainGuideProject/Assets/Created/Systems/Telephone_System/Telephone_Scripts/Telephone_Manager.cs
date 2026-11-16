using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using Random = UnityEngine.Random;

public sealed class TelephoneManager : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private int lastRouteQuestDay = -1;
    private bool isMouseOver;
    
    private void Start()
    {
        Main_Referencer.Instance.visual.phoneButton.onClick.AddListener(AccepptCall);
        Main_Referencer.Instance.visual.telephoneAnimator_Telephone.SetBool("Ringing", false);
        SetPhoneState(isOpen: false);
        StartCoroutine(tickTelephoneRoutine());
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

    private IEnumerator tickTelephoneRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(
                (Main_Referencer.Instance.value.dayInMinutes_HourValues * 60f / 24f)
                * Main_Referencer.Instance.value.hoursToTryTick_TelephoneValues);

            TryRinging();
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
        NewQuest();
        
        var animator = Main_Referencer.Instance.visual.telephoneAnimator_Telephone;

        StopCoroutine("timeToStopRinging");

        animator.SetBool("Ringing", true);

        Main_Referencer.Instance.value.telephoneRinging = true;

        StartCoroutine("timeToStopRinging");
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
    
    private void NewQuest()
    {
        if (RouteDayVerify())
        {
            Main_Referencer.Instance.manager.questManager.AddQuest(QuestManager.QuestType.Route);

            lastRouteQuestDay = Main_Referencer.Instance.value.currentDay_HourValues;
        }
        else
        {
            Main_Referencer.Instance.manager.questManager.AddQuest(QuestManager.QuestType.Single);
        }
    }

    private IEnumerator timeToStopRinging()
    {
        var animator = Main_Referencer.Instance.visual.telephoneAnimator_Telephone;

        yield return null;

        float duration = animator.GetCurrentAnimatorStateInfo(0).length;
        int repeats = Mathf.Max(1, Main_Referencer.Instance.value.ringingRepeats_TelephoneValues);

        yield return new WaitForSeconds(duration * repeats);

        animator.SetBool("Ringing", false);
        Main_Referencer.Instance.value.telephoneRinging = false;
    }

    public void AccepptCall()
    {
        SetPhoneState(isOpen: true);
        Main_Referencer.Instance.manager.questManager.OpenVoiceMail();
        Main_Referencer.Instance.visual.telephoneAnimator_Telephone.SetBool("Ringing", false);
        Main_Referencer.Instance.value.telephoneRinging = false;
    }
    
    private void UpdatePhoneOutline()
    {
        var visual = Main_Referencer.Instance.visual;
        visual.closedPhoneOutline.SetActive(isMouseOver && !visual.openPhone.activeSelf);
        visual.openPhoneOutline.SetActive(isMouseOver && visual.openPhone.activeSelf);
    }
    
    public void SetPhoneState(bool isOpen)
    {
        Main_Referencer.Instance.visual.openPhone.SetActive(isOpen);
        Main_Referencer.Instance.visual.closedPhone.SetActive(!isOpen);
        UpdatePhoneOutline();
    }
}
