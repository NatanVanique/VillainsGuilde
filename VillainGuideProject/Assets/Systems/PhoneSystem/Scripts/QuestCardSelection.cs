using UnityEngine;
using UnityEngine.UI;

public class QuestCardSelection : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private GameObject selectedBorder;
    [SerializeField] QuestCard questCard;

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
        questCard.AddMonster(info);
    }

    public void RemoveMonsterFromTheCard(MonsterInfo info)
    {
        questCard.RemoveMonster(info);
        int amountOfMonsters = questCard._listOfMonsters.Count;

        if(amountOfMonsters == 0)
        {
            SingleEntryPoint.Instance.ManagerManager.QuestSelectionManager.Deselect(this);
            IsSelected = false;
            RefreshBorder();
        }
    }
}