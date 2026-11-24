using UnityEngine;

[DisallowMultipleComponent]
public sealed class PopupBars : MonoBehaviour
{
    private Bars_Manager barsManager;

    private void OnEnable()
    {
        RegisterOpenPopup();
    }

    private void Start()
    {
        // An initially active popup can enable before the scene references awaken.
        if (barsManager == null)
        {
            RegisterOpenPopup();
        }
    }

    private void OnDisable()
    {
        if (barsManager != null)
        {
            barsManager.SetPopupOpen(gameObject, false);
        }
    }

    private void RegisterOpenPopup()
    {
        Main_Referencer references = Main_Referencer.Instance;
        if (references == null || references.manager == null)
        {
            return;
        }

        barsManager = references.manager.barsManager;
        if (barsManager != null)
        {
            barsManager.SetPopupOpen(gameObject, true);
        }
    }
}
