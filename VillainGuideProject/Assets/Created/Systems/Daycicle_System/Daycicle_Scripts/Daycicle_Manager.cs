using System;
using UnityEngine;

public sealed class DaycicleManager : MonoBehaviour
{
    [Header("System Variables")]
    private float curtime;

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
        if (Main_Referencer.Instance.value.currentHour_HourValues >= 24) Main_Referencer.Instance.value.currentHour_HourValues = 0;
        else Main_Referencer.Instance.value.currentHour_HourValues++;
        curtime = 0;
    }
}
