using UnityEngine;
using TriInspector;

[DeclareBoxGroup("Quest", Title = "Quest")]
[DeclareBoxGroup("Hour", Title = "Hour")]
public class ManagerReferencer : MonoBehaviour
{
    [Group("Quest")]
    public QuestManager questManager;
    
    [Group("Hour")]
    public DaycicleManager dayCicleManager;
}
