using UnityEngine;

public class ManagerManager : MonoBehaviour {
    [Header("Quests System")]
    public QuestSelectionManager QuestSelectionManager;
    [SerializeField] GameObject questMngPrefab;
    public QuestSelector QuestSelector;
    [SerializeField] GameObject questSelectorMngPrefab;


    [Header("Day System")]
    public DaySystem DaySystem;
    [SerializeField] GameObject daySysPrefab;

    [Header("Monsters")]
    [SerializeField] MonstersManager MonsterManPrefab;
    [HideInInspector] public MonstersManager MonsterMan;

    [Header("Journal")]
    [SerializeField] GameObject JournalRegisterPrefab;
    [SerializeField] QuestValidator QuestValPrefab;
    [HideInInspector] public JournalRegister JournalRegister;
    [HideInInspector] public QuestValidator QuestVal;

    public void Setup() {
        if (questMngPrefab != null) {
            questMngPrefab = Instantiate(questMngPrefab, transform);
            QuestSelectionManager = questMngPrefab.GetComponent<QuestSelectionManager>();
        }
        if (questSelectorMngPrefab != null) {
            questSelectorMngPrefab = Instantiate(questSelectorMngPrefab, transform);
            QuestSelector = questSelectorMngPrefab.GetComponent<QuestSelector>();
        }
        if (daySysPrefab != null) {
            daySysPrefab = Instantiate(daySysPrefab, transform);
            DaySystem = daySysPrefab.GetComponent<DaySystem>();
        }
        if (daySysPrefab != null) {
            MonsterMan = Instantiate(MonsterManPrefab, transform);
            MonsterMan.Setup();
        }
        if (JournalRegisterPrefab != null)
        {
            JournalRegisterPrefab = Instantiate(JournalRegisterPrefab, transform);
            JournalRegister = JournalRegisterPrefab.GetComponent<JournalRegister>();
        }
        if (QuestValPrefab != null) {
            QuestVal = Instantiate(QuestValPrefab, transform);
            QuestVal.Setup();
        }
    }
}