using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

public sealed class TelephoneManager : MonoBehaviour
{
    private int lastRouteQuestDay = -1;
    
    private void Start()
    {
        Main_Referencer.Instance.visual.telephoneAnimator_Telephone.SetBool("Ringing", false);

        StartCoroutine(tickTelephoneRoutine());
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
        int tries = Mathf.CeilToInt(
            (float)Main_Referencer.Instance.value.popularityLevel_Popularity /
            Main_Referencer.Instance.value.popularitysToNewTry_TelephoneValues);

        float chance = Main_Referencer.Instance.value.callChancePerHour_TelephoneValues
            .Evaluate(Main_Referencer.Instance.value.currentHour_HourValues / 24f);

        if (RouteDayVerify())
        {
            RingingTelephone();
            return;
        }

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
        Main_Referencer.Instance.manager.questManager.OpenVoiceMail();

        Main_Referencer.Instance.visual.telephoneAnimator_Telephone
            .SetBool("Ringing", false);

        Main_Referencer.Instance.value.telephoneRinging = false;
    }
}
