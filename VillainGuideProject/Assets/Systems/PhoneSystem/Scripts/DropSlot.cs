using UnityEngine;
using UnityEngine.EventSystems;

public class DropSlot : MonoBehaviour, IDropHandler
{
    public void ClearSlot()
    {
        foreach (DragItem item in GetComponentsInChildren<DragItem>(true))
            item.ReturnToOrigin();
    }

    public void OnDrop(PointerEventData eventData)
    {
        if (transform.childCount > 0 || eventData.pointerDrag == null)
            return;

        DragItem item = eventData.pointerDrag.GetComponent<DragItem>();

        if (item)
        {
            QuestCardSelection card = GetComponentInParent<QuestCardSelection>();
            if (card && !card.TrySelect())
                return;

            item.Drop(transform,card);
        }
    }
}
