using UnityEngine;
using TriInspector;

[DeclareBoxGroup("Quest", Title = "Quest")]
[DeclareBoxGroup("Monster", Title = "Monster")]
[DeclareBoxGroup("Hour", Title = "Hour")]
public class ManagerReferencer : MonoBehaviour
{
    [Group("Quest")]
    public QuestManager questManager;

    [Group("Monster")]
    public MonsterManager monsterManager;
    
    [Group("Hour")]
    public DaycicleManager dayCicleManager;
    
    [Group("Phone")]
    public TelephoneManager telephoneManager;
}
