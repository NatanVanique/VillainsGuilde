using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class HorizontalCarousel : MonoBehaviour
{
    [SerializeField] private Transform content;
    [SerializeField] private Button leftButton;
    [SerializeField] private Button rightButton;
    [SerializeField, Min(1)] private int maxVisibleItems = 3;
    [SerializeField] private float pageDuration = 0.35f;

    private int currentIndex;
    private RectTransform contentRect;
    private Vector2 restingPosition;
    private CanvasGroup contentGroup;
    private Sequence pageTween;
    private Sequence entranceTween;
    private readonly System.Collections.Generic.Dictionary<Transform, Vector3> entranceScales = new();
    private bool changingPage;

    private void Awake()
    {
        maxVisibleItems = Mathf.Max(1, maxVisibleItems);
        contentRect = (RectTransform)content;
        restingPosition = contentRect.anchoredPosition;
        contentGroup = content.GetComponent<CanvasGroup>();
        if (contentGroup == null) contentGroup = content.gameObject.AddComponent<CanvasGroup>();

        // Disable only ScrollRect input; item drag/drop handlers stay enabled.
        ScrollRect scrollRect = GetComponent<ScrollRect>();
        if (scrollRect != null)
        {
            scrollRect.StopMovement();
            scrollRect.enabled = false;
        }
        leftButton.onClick.AddListener(Previous);
        rightButton.onClick.AddListener(Next);
        Refresh();
    }

    private void Next()
    {
        if (currentIndex + maxVisibleItems < content.childCount)
            ChangePage(currentIndex + maxVisibleItems, 1);
    }

    private void Previous()
    {
        if (currentIndex > 0)
            ChangePage(Mathf.Max(0, currentIndex - maxVisibleItems), -1);
    }

    private void ChangePage(int index, int direction)
    {
        if (changingPage) return;
        FinishEntrance();
        changingPage = true;
        contentGroup.blocksRaycasts = false;
        UpdateButtons();
        float distance = ((RectTransform)content.parent).rect.width;
        float halfDuration = pageDuration * 0.5f;
        pageTween = DOTween.Sequence().SetUpdate(true);
        pageTween.Append(contentRect.DOAnchorPosX(restingPosition.x - direction * distance, halfDuration).SetEase(Ease.InQuad));
        pageTween.AppendCallback(() =>
        {
            currentIndex = index;
            UpdateItems();
            contentRect.anchoredPosition = restingPosition + Vector2.right * direction * distance;
        });
        pageTween.Append(contentRect.DOAnchorPosX(restingPosition.x, halfDuration).SetEase(Ease.OutQuad));
        pageTween.OnComplete(() =>
        {
            changingPage = false;
            contentGroup.blocksRaycasts = true;
            UpdateButtons();
        });
    }

    public void Refresh()
    {
        FinishEntrance();
        pageTween?.Kill();
        changingPage = false;
        contentGroup.blocksRaycasts = true;
        contentRect.anchoredPosition = restingPosition;
        currentIndex = 0;
        UpdateItems();
    }

    public void AnimateEntrance(float delay)
    {
        FinishEntrance();
        entranceTween = DOTween.Sequence();
        entranceTween.AppendInterval(delay);
        for (int i = currentIndex; i < Mathf.Min(currentIndex + maxVisibleItems, content.childCount); i++)
        {
            Transform item = content.GetChild(i);
            Vector3 scale = item.localScale;
            entranceScales[item] = scale;
            item.localScale = Vector3.zero;
            entranceTween.Append(item.DOScale(scale, 0.22f).SetEase(Ease.OutBack));
        }
        entranceTween.OnComplete(() => entranceScales.Clear());
    }

    private void FinishEntrance()
    {
        entranceTween?.Kill();
        foreach (var entry in entranceScales)
            if (entry.Key != null) entry.Key.localScale = entry.Value;
        entranceScales.Clear();
    }

    private void UpdateItems()
    {
        for (int i = 0; i < content.childCount; i++)
        {
            GameObject item = content.GetChild(i).gameObject;
            bool visible = i >= currentIndex && i < currentIndex + maxVisibleItems;
            CanvasGroup group = item.GetComponent<CanvasGroup>();
            if (group == null) group = item.AddComponent<CanvasGroup>();
            LayoutElement layout = item.GetComponent<LayoutElement>();
            if (layout == null) layout = item.AddComponent<LayoutElement>();
            // Do not disable cards: OnDisable clears their selection and drops.
            group.alpha = visible ? 1f : 0f;
            group.interactable = visible;
            group.blocksRaycasts = visible;
            layout.ignoreLayout = !visible;
        }
        LayoutRebuilder.ForceRebuildLayoutImmediate(contentRect);
        UpdateButtons();
    }

    private void UpdateButtons()
    {
        bool hasPrevious = currentIndex > 0;
        bool hasNext = currentIndex + maxVisibleItems < content.childCount;
        leftButton.gameObject.SetActive(hasPrevious);
        rightButton.gameObject.SetActive(hasNext);
        leftButton.interactable = hasPrevious && !changingPage;
        rightButton.interactable = hasNext && !changingPage;
    }

    private void OnDisable()
    {
        FinishEntrance();
        pageTween?.Kill();
        if (contentRect == null) return;
        changingPage = false;
        contentRect.anchoredPosition = restingPosition;
        contentGroup.blocksRaycasts = true;
        UpdateButtons();
    }
}
