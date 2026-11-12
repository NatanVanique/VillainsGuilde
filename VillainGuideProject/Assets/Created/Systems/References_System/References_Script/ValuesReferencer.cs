using UnityEngine;

public class ValuesReferencer : MonoBehaviour
{
    [Header("Hours Values")]
    public Gradient dayGradient_HourValues;
    public int currentHour_HourValues;
    public float dayInMinutes_HourValues;
    
    [Header("Times Calls")]
    public int beforeOfficeDayStartHour_TimeCalls = 6;
    public int OfficeDayStartHour_TimeCalls = 8;
    public int OfficeNightStartHour_TimeCalls = 18;
    
    [Header("Bars")] 
    public float velocity_Bars;
    
    
}
