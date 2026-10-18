using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class QuestCardSelection : MonoBehaviour, IDropHandler
{
    [Header("Components")]
    [SerializeField] private GameObject selectedBorder;
    public QuestCard QuestCard;

    public bool IsSelected { get; private set; }

    public void Setup()
    {

    }
    private void Awake()
    {
        // The card root has no Graphic; a transparent one makes its entire rectangle droppable.
        if (GetComponent<Graphic>() == null)
        {
            Image hitArea = gameObject.AddComponent<Image>();
            hitArea.color = Color.clear;
            hitArea.raycastTarget = true;
        }
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
            QuestCard._listOfMonsters.Clear();
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

    public void OnDrop(PointerEventData eventData)
    {
        TryDropItem(eventData);
    }

    internal bool TryDropItem(PointerEventData eventData, DropSlot requestedSlot = null)
    {
        if (eventData.pointerDrag == null)
            return false;

        DragItem item = eventData.pointerDrag.GetComponent<DragItem>();
        if (item == null)
            return false;

        DropSlot firstEmpty = null;
        foreach (DropSlot slot in GetComponentsInChildren<DropSlot>())
        {
            if (slot.IsEmpty)
            {
                firstEmpty = slot;
                break;
            }
        }

        // Dropping directly on a later slot cannot skip an earlier empty slot.
        if (firstEmpty == null || (requestedSlot != null && requestedSlot != firstEmpty) || !TrySelect())
            return false;

        item.Drop(firstEmpty.transform, this);
        return true;
    }

    internal void CompactSlots()
    {
        DropSlot[] slots = GetComponentsInChildren<DropSlot>();
        int next = 0;
        for (int i = 0; i < slots.Length; i++)
        {
            DragItem item = slots[i].GetComponentInChildren<DragItem>();
            if (item == null)
                continue;
            if (i != next)
                item.MoveToSlot(slots[next].transform);
            next++;
        }
    }
}
