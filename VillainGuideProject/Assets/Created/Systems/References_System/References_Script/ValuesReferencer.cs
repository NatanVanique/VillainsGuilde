using TriInspector;
using UnityEngine;
using UnityEngine.Localization;
using System.Collections.Generic;

[DeclareFoldoutGroup("Hour Values", Expanded = true)]
[DeclareFoldoutGroup("Time Calls", Expanded = true)]
[DeclareFoldoutGroup("Bars", Expanded = true)]
[DeclareFoldoutGroup("Popularity", Expanded = true)]
[DeclareFoldoutGroup("Telephone Values", Expanded = true)]
[DeclareFoldoutGroup("Quest Values", Expanded = true)]
[DeclareFoldoutGroup("Journal Values", Expanded = true)]
public class ValuesReferencer : MonoBehaviour
{
    [Group("Hour Values")] public Gradient dayGradient_HourValues;
    [Group("Hour Values")] public int currentHour_HourValues;
    [Group("Hour Values")] public float dayInMinutes_HourValues;
    [Group("Hour Values")] public int currentDay_HourValues = 1;
    
    [Group("Time Calls")] public int beforeOfficeDayStartHour_TimeCalls = 6;
    [Group("Time Calls")] public int OfficeDayStartHour_TimeCalls = 8;
    [Group("Time Calls")] public int OfficeNightStartHour_TimeCalls = 18;
    
    [Group("Bars")] public float velocity_Bars; 
    
    [Group("Popularity")] [Range(0f, 100f)] public int popularityXp_Popularity;
    [Group("Popularity")] public int maxPopularityLevel_Popularity = 10;
    [Group("Popularity")] [Slider(1, nameof(maxPopularityLevel_Popularity))] public int popularityLevel_Popularity = 1;
    
    [Group("Telephone Values")] public bool  telephoneRinging;
    [Group("Telephone Values")] public AnimationCurve  callChancePerHour_TelephoneValues;
    [Group("Telephone Values")] public int  popularitysToNewTry_TelephoneValues;
    [Group("Telephone Values")] public int  hoursToTryTick_TelephoneValues;
    [Group("Telephone Values")] public int  ringingRepeats_TelephoneValues;
    
    [Group("Quest Values")]
    public int  daysToStartRouteQuest_QuestValues;
    public List<List<(CreateQuest_Data data, CreateMonster_Data monster, bool isWinned, bool attempted)>> realizedDayRegister = new();
    
    [Group("Journal Values")] public LocalizedString  DailyNewsText_JournalValues;
    [Group("Journal Values")] public LocalizedString  RewardsText_JournalValues;
    [Group("Journal Values")] public LocalizedString  RewardsTabText_JournalValues;
    [Group("Journal Values")] public LocalizedString  NewsTabText_JournalValues;
}
