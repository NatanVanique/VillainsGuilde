using System;

[Serializable]
public sealed class GameplaySaveData
{
    public int day;
    public int hour;
    public float secondsIntoHour;
    public long balance;
    public int popularityLevel;
    public int popularityXp;
    public int lives;
    public int displayedLives;
    public bool healthPanelOpen;
    public bool failedPanelOpen;
    public bool defeatPanelOpen;
    public float defeatSecondsRemaining;
    public bool defeatArrowVisible;
    public float defeatArrowElapsed;

    public int roomScreen;
    public bool roomNavigationHidden;
    public float barsProgress;
    public bool barsManuallyOpen;
    public bool jobBoardActive;
    public bool jobBoardEnabled;
    public bool contractTooltipActive;
    public bool contractTooltipEnabled;

    public int lastRouteQuestDay;
    public float secondsToPhoneTick;
    public bool phoneOpen;
    public bool phoneRinging;
    public float ringSecondsRemaining;

    public static GameplaySaveData Capture(Main_Referencer references)
    {
        ValuesReferencer values = references.value;
        ManagerReferencer managers = references.manager;
        VisualReferencer visual = references.visual;
        GameplaySaveData data = new()
        {
            day = values.currentDay_HourValues,
            hour = values.currentHour_HourValues,
            secondsIntoHour = managers.dayCicleManager.SecondsIntoHour,
            balance = managers.moneyManager.Balance,
            popularityLevel = values.popularityLevel_Popularity,
            popularityXp = values.popularityXp_Popularity,
            roomScreen = managers.roomManager.CurrentScreen,
            roomNavigationHidden = managers.roomManager.NavigationHidden,
            barsProgress = managers.barsManager.Progress,
            barsManuallyOpen = managers.barsManager.ManuallyOpen,
            jobBoardActive = visual.lookingForJobCanvas_LookingForJob.gameObject.activeSelf,
            jobBoardEnabled = visual.lookingForJobCanvas_LookingForJob.enabled,
            contractTooltipActive = visual.contractTooltipCanvas_ContractTolltip.gameObject.activeSelf,
            contractTooltipEnabled = visual.contractTooltipCanvas_ContractTolltip.enabled
        };
        managers.lifeManager.CaptureState(data);
        managers.telephoneManager.CaptureState(data);
        return data;
    }

    public void Restore(Main_Referencer references)
    {
        ValuesReferencer values = references.value;
        ManagerReferencer managers = references.manager;
        VisualReferencer visual = references.visual;
        values.currentDay_HourValues = day;
        values.currentHour_HourValues = hour;
        managers.dayCicleManager.SecondsIntoHour = secondsIntoHour;
        managers.moneyManager.RestoreBalance(balance);
        values.popularityLevel_Popularity = popularityLevel;
        values.popularityXp_Popularity = popularityXp;
        visual.lookingForJobCanvas_LookingForJob.enabled = jobBoardEnabled;
        visual.contractTooltipCanvas_ContractTolltip.enabled = contractTooltipEnabled;
        visual.lookingForJobCanvas_LookingForJob.gameObject.SetActive(jobBoardActive);
        visual.contractTooltipCanvas_ContractTolltip.gameObject.SetActive(contractTooltipActive);
        managers.lifeManager.RestoreState(this);
        managers.roomManager.RestoreScreen(roomScreen, roomNavigationHidden);
        managers.telephoneManager.RestoreState(this);
        managers.barsManager.RestoreProgress(barsProgress, barsManuallyOpen);
    }
}
