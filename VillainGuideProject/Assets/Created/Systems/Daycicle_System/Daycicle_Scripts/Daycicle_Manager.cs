using System;
using UnityEngine;

public sealed class DaycicleManager : MonoBehaviour
{
    [Header("System Variables")]
    private float curtime;

    public event Action BeforeOfficeDayStart;

    public float NormalizedDayProgress
    {
        get
        {
            float secondsPerHour = Mathf.Max(
                0.01f,
                Main_Referencer.Instance.value.dayInMinutes_HourValues * 60f / 24f);

            float hourProgress = Mathf.Clamp01(curtime / secondsPerHour);
            return Mathf.Clamp01(
                (Main_Referencer.Instance.value.currentHour_HourValues + hourProgress) / 24f);
        }
    }

    private void Update()
    {
        HourUpdate();
    }

    private void HourUpdate()
    {
        float secondsPerHour = Mathf.Max(
            0.01f,
            Main_Referencer.Instance.value.dayInMinutes_HourValues * 60f / 24f);

        curtime += Time.deltaTime;

        if (curtime >= secondsPerHour) HourAdd();
    }

    private void HourAdd()
    {
        if (Main_Referencer.Instance.value.currentHour_HourValues >= 23)
        {
            DayAdd();
        }
        else Main_Referencer.Instance.value.currentHour_HourValues++;
        curtime = 0;
        ValuesReferencer values = Main_Referencer.Instance.value;
        if (values.currentHour_HourValues == values.beforeOfficeDayStartHour_TimeCalls)
        {
            BeforeOfficeDayStart?.Invoke();
        }
    }
    
    private void DayAdd()
    {
        Main_Referencer.Instance.value.currentHour_HourValues = 0;
        Main_Referencer.Instance.value.currentDay_HourValues++;
    }
}
