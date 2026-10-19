using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class LoseHealthUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private RectTransform bar;
    [SerializeField] private RectTransform message;
    [SerializeField] private RectTransform healthContainer;
    [SerializeField] private Image[] healthIcons;
    [SerializeField] private Sprite regularHealthSprite;
    [SerializeField] private Sprite lostHealthSprite;

    [Header("Animation")]
    [SerializeField, Min(0.01f)] private float barDuration = 0.5f;
    [SerializeField, Min(0.01f)] private float popupDuration = 0.25f;
    [SerializeField, Min(0.01f)] private float healthDuration = 0.35f;
    [SerializeField, Min(0f)] private float holdDuration = 1f;
    [SerializeField] private int sortingOrder = 11;

    private CanvasGroup group;
    private Sequence sequence;
    private Vector2 barPosition;
    private Vector3 messageScale;
    private Vector3 containerScale;
    private Vector3[] iconScales;
    private bool initialized;
    public bool IsPlaying { get; private set; }

    private void Awake() => Initialize();

    private void Initialize()
    {
        if (initialized) return;
        initialized = true;
        group = GetComponent<CanvasGroup>();
        if (group == null) group = gameObject.AddComponent<CanvasGroup>();
        Canvas canvas = GetComponent<Canvas>();
        if (canvas == null) canvas = gameObject.AddComponent<Canvas>();
        canvas.overrideSorting = true;
        canvas.sortingOrder = sortingOrder;
        if (GetComponent<GraphicRaycaster>() == null) gameObject.AddComponent<GraphicRaycaster>();
        // Keep the overlay modal without changing any RectTransform anchors.
        Image blocker = GetComponent<Image>();
        if (blocker == null) blocker = gameObject.AddComponent<Image>();
        blocker.color = Color.clear;
        blocker.raycastTarget = true;
        barPosition = bar.anchoredPosition;
        messageScale = message.localScale;
        containerScale = healthContainer.localScale;
        iconScales = new Vector3[healthIcons.Length];
        for (int i = 0; i < healthIcons.Length; i++) iconScales[i] = healthIcons[i].transform.localScale;
        Hide();
    }

    public void Show(int previousHealth, int currentHealth, Action onComplete)
    {
        Initialize();
        if (IsPlaying) return;
        gameObject.SetActive(true);
        IsPlaying = true;
        group.alpha = 1f;
        group.blocksRaycasts = true;
        group.interactable = false;
        Canvas.ForceUpdateCanvases();
        float distance = ((RectTransform)bar.parent).rect.width + bar.rect.width;
        bar.anchoredPosition = barPosition + Vector2.left * distance;
        message.localScale = Vector3.zero;
        healthContainer.localScale = Vector3.zero;
        for (int i = 0; i < healthIcons.Length; i++)
        {
            healthIcons[i].sprite = i < previousHealth ? regularHealthSprite : lostHealthSprite;
            healthIcons[i].transform.localScale = iconScales[i];
        }

        sequence = DOTween.Sequence().SetUpdate(true);
        sequence.Append(bar.DOAnchorPos(barPosition, barDuration).SetEase(Ease.OutCubic));
        sequence.Append(message.DOScale(messageScale, popupDuration).SetEase(Ease.OutBack));
        sequence.Join(healthContainer.DOScale(containerScale, popupDuration).SetEase(Ease.OutBack));
        for (int i = Mathf.Min(previousHealth, healthIcons.Length) - 1; i >= Mathf.Max(0, currentHealth); i--)
        {
            Image icon = healthIcons[i];
            Vector3 scale = iconScales[i];
            sequence.Append(icon.transform.DOScale(scale * 0.6f, healthDuration * 0.5f).SetEase(Ease.InBack));
            sequence.AppendCallback(() => icon.sprite = lostHealthSprite);
            sequence.Append(icon.transform.DOScale(scale, healthDuration * 0.5f).SetEase(Ease.OutBack));
        }
        sequence.AppendInterval(holdDuration);
        sequence.Append(message.DOScale(Vector3.zero, popupDuration).SetEase(Ease.InBack));
        sequence.Join(healthContainer.DOScale(Vector3.zero, popupDuration).SetEase(Ease.InBack));
        sequence.Append(bar.DOAnchorPos(barPosition + Vector2.right * distance, barDuration).SetEase(Ease.InCubic));
        sequence.OnComplete(() =>
        {
            Hide();
            onComplete?.Invoke();
        });
    }

    private void Hide()
    {
        IsPlaying = false;
        group.alpha = 0f;
        group.blocksRaycasts = false;
        group.interactable = false;
    }

    private void OnDisable()
    {
        sequence?.Kill();
        if (group != null) Hide();
    }

    private void OnDestroy() => sequence?.Kill();
}
