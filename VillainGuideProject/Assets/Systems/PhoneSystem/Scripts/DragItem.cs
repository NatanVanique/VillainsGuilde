using UnityEngine;
using UnityEngine.EventSystems;

public class DragItem : MonoBehaviour, IPointerDownHandler, IPointerClickHandler, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    Canvas canvas;
    CanvasGroup canvasGroup;
    Transform startParent;
    Vector3 startPosition;

    bool dropped;

    void Start()
    {
        canvas = GetComponentInParent<Canvas>();
        canvasGroup = GetComponent<CanvasGroup>();

        if (!canvasGroup)
            canvasGroup = gameObject.AddComponent<CanvasGroup>();

        startParent = transform.parent;
        startPosition = transform.localPosition;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        eventData.Use();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        eventData.Use();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        dropped = false;

        transform.SetParent(canvas.transform);
        transform.SetAsLastSibling();

        canvasGroup.blocksRaycasts = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        transform.position = eventData.position;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        canvasGroup.blocksRaycasts = true;

        if (!dropped)
        {
            ReturnToOrigin();
        }
    }

    public void ReturnToOrigin()
    {
        dropped = false;
        canvasGroup.blocksRaycasts = true;
        transform.SetParent(startParent);
        transform.localPosition = startPosition;
    }

    public void Drop(Transform slot)
    {
        dropped = true;

        transform.SetParent(slot);
        transform.localPosition = Vector3.zero;
    }
}
