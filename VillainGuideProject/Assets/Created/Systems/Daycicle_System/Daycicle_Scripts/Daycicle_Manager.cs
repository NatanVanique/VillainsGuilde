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
        if (curtime * 60 >= 24 / Main_Referencer.Instance.value.dayInMinutes_HourValues) HourAdd();
        else curtime += Time.fixedDeltaTime;
    }

    private void HourAdd()
    {
        if (Main_Referencer.Instance.value.currentHour_HourValues >= 24) Main_Referencer.Instance.value.currentHour_HourValues = 0;
        else Main_Referencer.Instance.value.currentHour_HourValues++;
        curtime = 0;
    }
}
