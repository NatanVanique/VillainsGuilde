using System;
using UnityEngine;

public sealed class DaycicleManager : MonoBehaviour
{
    [Header("Hours Values")]
    public int currentHour;
    [SerializeField] private float dayInMinutes;
    
    [Header("Times Hours")]
    [SerializeField] private int beforeOfficeDayStartHour = 6;
    [SerializeField] private int OfficeDayStartHour = 8;
    [SerializeField] private int OfficeNightStartHour = 18;
    
    [Header("System Variables")]
    private float curtime;

    private void Update()
    {
        HourUpdate();
    }

    private void HourUpdate()
    {
        if (curtime * 60 >= 24 / dayInMinutes) HourAdd();
        else curtime += Time.fixedDeltaTime;
    }

    private void HourAdd()
    {
        if (currentHour >= 24) currentHour = 0;
        else currentHour++;
        curtime = 0;
    }
}
