using UnityEngine;
using TriInspector;

[DeclareBoxGroup("Quest", Title = "Quest")]
[DeclareBoxGroup("Monster", Title = "Monster")]
[DeclareBoxGroup("Journal", Title = "Journal")]
[DeclareBoxGroup("Hour", Title = "Hour")]
[DeclareBoxGroup("Phone", Title = "Phone")]
[DeclareBoxGroup("Room", Title = "Room")]

public class ManagerReferencer : MonoBehaviour
{
    [Group("Quest")] public QuestManager questManager;

    [Group("Monster")] public MonsterManager monsterManager;

    [Group("Journal")] public JournalManager journalManager;
    
    [Group("Hour")] public DaycicleManager dayCicleManager;
    
    [Group("Phone")] public TelephoneManager telephoneManager;
    
    [Group("Room")] public RoomManager roomManager;
}
