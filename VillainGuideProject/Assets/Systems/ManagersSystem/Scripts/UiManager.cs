using UnityEngine;

public class UiManager : MonoBehaviour
{
    [Header("Missions Ui")]
    public MissionsScreenController MissionsScreenController;
    [SerializeField] GameObject missionsScreenMngPrefab;

    [Header("Journal")]
    public JournalManager JournalManager;   
    [SerializeField] GameObject journalPrefab;


    public void Setup()
    {
        MissionsScreenController = Instantiate(missionsScreenMngPrefab).GetComponentInChildren<MissionsScreenController>(true);
        MissionsScreenController.Setup();
        JournalManager = Instantiate(journalPrefab).GetComponent<JournalManager>();
    }
}