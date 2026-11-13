using UnityEngine;
using TriInspector;

[DeclareBoxGroup("Quest", Title = "Quest")]
public class ManagerReferencer : MonoBehaviour
{
    [Group("Quest")]
    public QuestManager questManager;
}
