using UnityEngine;
using UnityEngine.EventSystems;

public class DropSlot : MonoBehaviour, IDropHandler
{
    public bool IsEmpty => GetComponentInChildren<DragItem>(true) == null;

    public void ClearSlot()
    {
        foreach (DragItem item in GetComponentsInChildren<DragItem>(true))
            item.ReturnToOrigin();
    }

    public void OnDrop(PointerEventData eventData)
    {
        QuestCardSelection card = GetComponentInParent<QuestCardSelection>();
        if (card != null)
            card.TryDropItem(eventData);
    }
}
