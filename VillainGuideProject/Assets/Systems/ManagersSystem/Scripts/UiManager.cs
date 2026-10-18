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
        MissionsScreenController = Instantiate(missionsScreenMngPrefab, transform).GetComponentInChildren<MissionsScreenController>(true);
        MissionsScreenController.Setup();
        GameObject journalManagerObject = Instantiate(journalPrefab, transform);
        JournalManager = journalManagerObject.GetComponent<JournalManager>();

        if (JournalManager != null)
        {
            JournalManager.Setup();
        }
        else
        {
            Debug.LogError("The journal prefab assigned to UiManager must contain JournalManager.");
            Destroy(journalManagerObject);
        }
    }
}
