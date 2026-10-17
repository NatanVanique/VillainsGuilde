using UnityEngine;

public class ManagerManager : MonoBehaviour
{
    [Header("Quests System")]
    public QuestSelectionManager QuestSelectionManager;
    [SerializeField] GameObject questMngPrefab;
    public QuestSelector QuestSelector;
    [SerializeField] GameObject questSelectorMngPrefab;


    [Header("Day System")]
    public DaySystem DaySystem;
    [SerializeField] GameObject daySysPrefab;

    public void Setup()
    {
        if (questMngPrefab != null)
        {
            questMngPrefab = Instantiate(questMngPrefab);
            QuestSelectionManager = questMngPrefab.GetComponent<QuestSelectionManager>();
        }
        if(questSelectorMngPrefab != null)
        {
            questSelectorMngPrefab = Instantiate(questSelectorMngPrefab);
            QuestSelector = questSelectorMngPrefab.GetComponent<QuestSelector>();
        }
        if(daySysPrefab != null)
        {
            daySysPrefab = Instantiate(daySysPrefab);
            DaySystem = daySysPrefab.GetComponent<DaySystem>();
        }
    }

    public void AdvanceDay()
    {
        SingleEntryPoint.Instance.ManagerManager.DaySystem.AdvanceDay();
        SingleEntryPoint.Instance.UiManager.MissionsScreenController.SetupQuests();
    }
}
