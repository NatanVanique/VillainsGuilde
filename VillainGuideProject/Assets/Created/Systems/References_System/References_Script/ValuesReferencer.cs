using TriInspector;
using UnityEngine;

[DeclareFoldoutGroup("Hour Values", Expanded = true)]
[DeclareFoldoutGroup("Time Calls", Expanded = true)]
[DeclareFoldoutGroup("Bars", Expanded = true)]
public class ValuesReferencer : MonoBehaviour
{
    [Group("Hour Values")]
    public Gradient dayGradient_HourValues;

    [Group("Hour Values")]
    public int currentHour_HourValues;

    [Group("Hour Values")]
    public float dayInMinutes_HourValues;
    
    [Group("Time Calls")]
    public int beforeOfficeDayStartHour_TimeCalls = 6;

    [Group("Time Calls")]
    public int OfficeDayStartHour_TimeCalls = 8;

    [Group("Time Calls")]
    public int OfficeNightStartHour_TimeCalls = 18;
    
    [Group("Bars")]
    public float velocity_Bars;
}
