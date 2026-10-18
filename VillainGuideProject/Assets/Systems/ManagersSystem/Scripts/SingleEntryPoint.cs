using UnityEngine;

public class SingleEntryPoint : MonoBehaviour
{
    public static SingleEntryPoint Instance { get; private set; }
    public ManagerManager ManagerManager;
    public UiManager UiManager;
    [SerializeField] GameObject mmPrefab;
    [SerializeField] GameObject umPrefab;

    public void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
            return;
        }
        else
        {
            Instance = this;
        }

        if (UiManager != null)
        {
            UiManager = Instantiate(umPrefab, transform).GetComponent<UiManager>();
            UiManager.Setup();
        }

        if (ManagerManager != null)
        {
            ManagerManager = Instantiate(mmPrefab, transform).GetComponent<ManagerManager>();
            ManagerManager.Setup();
        }
    }
}
