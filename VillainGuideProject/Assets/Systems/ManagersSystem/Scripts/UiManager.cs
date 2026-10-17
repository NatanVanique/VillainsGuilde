using UnityEngine;

public class UiManager : MonoBehaviour
{
    [Header("Missions Ui")]
    public MissionsScreenController MissionsScreenController;
    [SerializeField] GameObject missionsScreenMngPrefab;

    public void Setup()
    {
        MissionsScreenController = Instantiate(missionsScreenMngPrefab).GetComponentInChildren<MissionsScreenController>(true);
    }
}