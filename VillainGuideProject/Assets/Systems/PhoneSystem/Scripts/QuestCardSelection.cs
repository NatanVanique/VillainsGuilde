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

    internal void SetSelected(bool selected)
    {
        IsSelected = selected;
        RefreshBorder();
    }

    private void RefreshBorder()
    {
        selectedBorder.SetActive(IsSelected);
    }
}