using TriInspector;
using UnityEngine;

[DeclareBoxGroup("References", Title = "Referências")]
public class Main_Referencer : MonoBehaviour
{
    public static Main_Referencer Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        gameObject.AddComponent<GameSaveSystem>();
    }

    private void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
        }
    }

    [Group("References")]
    public VisualReferencer visual;

    [Group("References")]
    public ValuesReferencer value;

    [Group("References")]
    public MonsterReferencer monster;
    
    [Group("References")] 
    public QuestReferencer quest;
    
    [Group("References")] 
    public ManagerReferencer manager;
}
