using UnityEngine;
using UnityEngine.EventSystems;

public class DragItem : MonoBehaviour, IPointerDownHandler, IPointerClickHandler, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    Canvas canvas;
    CanvasGroup canvasGroup;
    Transform startParent;
    HorizontalCarousel originCarousel;
    Vector3 startPosition;

    QuestCardSelection _card;

    [SerializeField] MonsterIconPrefab monsterIconPrefab;

    bool dropped;

    void Start()
    {
        canvas = GetComponentInParent<Canvas>();
        canvasGroup = GetComponent<CanvasGroup>();

        if (!canvasGroup)
            canvasGroup = gameObject.AddComponent<CanvasGroup>();

        startParent = transform.parent;
        originCarousel = startParent.GetComponentInParent<HorizontalCarousel>();
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
        QuestCardSelection previousCard = _card;
        _card = null;

        dropped = false;

        transform.SetParent(canvas.transform);
        transform.SetAsLastSibling();
        originCarousel.RefreshAfterItemChange();

        if (previousCard != null)
        {
            previousCard.RemoveMonsterFromTheCard(monsterIconPrefab.Info);
            previousCard.CompactSlots();
        }

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
        _card = null;
        canvasGroup.blocksRaycasts = true;
        transform.SetParent(startParent);
        transform.localPosition = startPosition;
        originCarousel.RefreshAfterItemChange();
    }

    public void Drop(Transform slot, QuestCardSelection card)
    {

        dropped = true;

        _card = card;
        card.AddMonsterToTheCard(monsterIconPrefab.Info);

        transform.SetParent(slot);
        transform.localPosition = Vector3.zero;
    }

    internal void MoveToSlot(Transform slot)
    {
        transform.SetParent(slot);
        transform.localPosition = Vector3.zero;
    }
}
