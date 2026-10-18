using UnityEngine;
using UnityEngine.UI;

public class QuestCardSelection : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private GameObject selectedBorder;
    public QuestCard QuestCard;

    public bool IsSelected { get; private set; }

    public void Setup()
    {

    }
    private void OnEnable()
    {
        RefreshBorder();
    }

    private void OnDisable()
    {
        SingleEntryPoint.Instance.ManagerManager.QuestSelectionManager.Deselect(this);
        IsSelected = false;
        RefreshBorder();
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

    public void AddMonsterToTheCard(MonsterInfo info)
    {
        QuestCard.AddMonster(info);
    }

    public void RemoveMonsterFromTheCard(MonsterInfo info)
    {
        QuestCard.RemoveMonster(info);
        int amountOfMonsters = QuestCard._listOfMonsters.Count;

        if(amountOfMonsters == 0)
        {
            SingleEntryPoint.Instance.ManagerManager.QuestSelectionManager.Deselect(this);
            IsSelected = false;
            RefreshBorder();
        }
    }
}