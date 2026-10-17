using UnityEngine;
using UnityEngine.UI;

public class QuestCardSelection : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private Button cardButton;
    [SerializeField] private GameObject selectedBorder;

    [Header("Selection")]
    [SerializeField] private QuestSelectionManager selectionManager;

    public bool IsSelected { get; private set; }

    private void OnEnable()
    {
        selectionManager = QuestSelectionManager.Instance;
        cardButton.onClick.AddListener(ToggleSelection);
        RefreshBorder();
    }

    private void OnDisable()
    {
        cardButton.onClick.RemoveListener(ToggleSelection);
        selectionManager.Deselect(this);

        IsSelected = false;
        RefreshBorder();
    }

    private void ToggleSelection()
    {
        selectionManager.ToggleSelection(this);
    }

    public bool TrySelect()
    {
        if (selectionManager == null)
            selectionManager = QuestSelectionManager.Instance;

        return selectionManager.TrySelect(this);
    }

    internal void SetSelected(bool selected)
    {
        if (IsSelected && !selected)
        {
            foreach (DropSlot slot in GetComponentsInChildren<DropSlot>(true))
                slot.ClearSlot();
        }

        IsSelected = selected;
        RefreshBorder();
    }

    private void RefreshBorder()
    {
        selectedBorder.SetActive(IsSelected);
    }
}
