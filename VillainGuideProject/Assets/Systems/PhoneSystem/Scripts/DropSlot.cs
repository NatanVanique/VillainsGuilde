using UnityEngine;
using UnityEngine.EventSystems;

public class DropSlot : MonoBehaviour, IDropHandler
{
    public void OnDrop(PointerEventData eventData)
    {
        if (transform.childCount > 0)
            return;

        DragItem item = eventData.pointerDrag.GetComponent<DragItem>();

        if (item)
        {
            item.Drop(transform);
        }
    }
}