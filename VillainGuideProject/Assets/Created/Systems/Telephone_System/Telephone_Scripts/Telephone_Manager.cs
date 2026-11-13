using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

public sealed class TelephoneManager : MonoBehaviour
{
    private void Start()
    {
        StartCoroutine(tickTelephoneRoutine());
    }

    private IEnumerator tickTelephoneRoutine()
    {
        while (true)
        {
            TryRinging();

            yield return new WaitForSeconds(
                (Main_Referencer.Instance.value.dayInMinutes_HourValues * 60f / 24f)
                * Main_Referencer.Instance.value.hoursToTryTick_TelephoneValues);
        }
    }

    private void TryRinging()
    {
        int tries = Mathf.RoundToInt(
            (float)Main_Referencer.Instance.value.popularityLevel_Popularity /
            Main_Referencer.Instance.value.popularitysToNewTry_TelephoneValues);

        float chance = Main_Referencer.Instance.value.callChancePerHour_TelephoneValues
            .Evaluate(Main_Referencer.Instance.value.currentHour_HourValues);

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
        var animator = Main_Referencer.Instance.visual.telephoneAnimator_Telephone;

        StopCoroutine("timeToStopRinging");

        animator.SetBool("Ringing", true);
        animator.Play("Ringing", 0, 0f);
        animator.Update(0f);

        Main_Referencer.Instance.value.telephoneRinging = true;
        Main_Referencer.Instance.visual.telephoneButton_Telephone.enabled = true;

        StartCoroutine("timeToStopRinging");
    }

    private IEnumerator timeToStopRinging()
    {
        var animator = Main_Referencer.Instance.visual.telephoneAnimator_Telephone;

        float duration = animator.GetCurrentAnimatorStateInfo(0).length;

        yield return new WaitForSeconds(duration * Main_Referencer.Instance.value.ringingRepeats_TelephoneValues);

        AccepptCall();
    }

    public void AccepptCall()
    {
        StopCoroutine("timeToStopRinging");

        Main_Referencer.Instance.visual.telephoneAnimator_Telephone
            .SetBool("Ringing", false);

        Main_Referencer.Instance.visual.telephoneButton_Telephone.enabled = false;
        Main_Referencer.Instance.value.telephoneRinging = false;

        Main_Referencer.Instance.manager.questManager.OpenVoiceMail();
    }
}