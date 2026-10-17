using UnityEngine;
using UnityEngine.UI;

public class QuestCardSelection : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private Button cardButton;
    [SerializeField] private GameObject selectedBorder;

    public bool IsSelected { get; private set; }

    public void Setup()
    {

    }
    private void OnEnable()
    {
        cardButton.onClick.AddListener(ToggleSelection);
        RefreshBorder();
    }

    private void OnDisable()
    {
        cardButton.onClick.RemoveListener(ToggleSelection);
        SingleEntryPoint.Instance.ManagerManager.QuestSelectionManager.Deselect(this);

        IsSelected = false;
        RefreshBorder();
    }

    private void ToggleSelection()
    {
        SingleEntryPoint.Instance.ManagerManager.QuestSelectionManager.ToggleSelection(this);
    }

    public bool TrySelect()
    {
        return SingleEntryPoint.Instance.ManagerManager.QuestSelectionManager.TrySelect(this);
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