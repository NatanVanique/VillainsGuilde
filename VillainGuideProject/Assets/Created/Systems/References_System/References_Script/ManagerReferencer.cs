using UnityEngine;
using TriInspector;

[DeclareBoxGroup("Quest", Title = "Quest")]
[DeclareBoxGroup("Monster", Title = "Monster")]
[DeclareBoxGroup("Journal", Title = "Journal")]
[DeclareBoxGroup("Bars", Title = "Bars")]
[DeclareBoxGroup("Hour", Title = "Hour")]
[DeclareBoxGroup("Phone", Title = "Phone")]
[DeclareBoxGroup("Room", Title = "Room")]
[DeclareBoxGroup("Economy", Title = "Economy")]
[DeclareBoxGroup("Life", Title = "Life")]

public class ManagerReferencer : MonoBehaviour
{
    [Group("Quest")] public QuestManager questManager;

    [Group("Monster")] public MonsterManager monsterManager;
    [Group("Monster")] public ContractManager contractManager;

    [Group("Journal")] public JournalManager journalManager;

    [Group("Bars")] public Bars_Manager barsManager;
    
    [Group("Hour")] public DaycicleManager dayCicleManager;
    
    [Group("Phone")] public TelephoneManager telephoneManager;
    
    [Group("Room")] public RoomManager roomManager;

    [Group("Life")] public Life_Manager lifeManager;

    [Group("Economy")] public MoneyManager moneyManager;
    [Group("Economy")] public PopularityManager popularityManager;
}
