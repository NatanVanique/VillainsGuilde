using UnityEngine;
using DG.Tweening;

public class JournalManager : MonoBehaviour
{
    [SerializeField] private GameObject journalPrefab;

    [Header("Animation")]
    [SerializeField] private float animationDuration = 0.5f;

    [Header("Ui Components")]
    private JournalView journalInstance;
    private JournalUi journalUi;
    private CanvasGroup canvasGroup;
    private Vector3 openScale;
    private bool closing;
    private RectTransform popup;
    private Vector2 openPosition;
    private Vector2 hiddenPosition;
    private readonly System.Collections.Generic.List<CanvasGroup> fadeGroups = new();

    public void Setup()
    {
        journalInstance = Instantiate(journalPrefab, transform).GetComponent<JournalView>();
        journalUi = journalInstance.GetComponentInChildren<JournalUi>(true);
        canvasGroup = journalInstance.GetComponent<CanvasGroup>();
        if (canvasGroup == null)
            canvasGroup = journalInstance.gameObject.AddComponent<CanvasGroup>();
        journalInstance.transform.localScale = Vector3.one;
        popup = (RectTransform)journalInstance.AnimationTarget;
        foreach (RectTransform target in journalInstance.FadeTargets)
        {
            CanvasGroup group = target.GetComponent<CanvasGroup>();
            if (group == null) group = target.gameObject.AddComponent<CanvasGroup>();
            fadeGroups.Add(group);
        }
        openPosition = popup.anchoredPosition;
        openScale = popup.localScale;
        if (openScale.sqrMagnitude < 0.001f)
            openScale = Vector3.one;
        journalInstance.CloseButton.onClick.AddListener(CloseJournal);
        if (journalUi != null)
            journalUi.DoneRequested += DeactiveJournal;
        journalInstance.gameObject.SetActive(false);
    }

    public void ActiveJournal()
    {
        closing = false;
        popup.DOKill();
        canvasGroup.DOKill();
        popup.localScale = openScale;
        canvasGroup.alpha = 1f;
        canvasGroup.blocksRaycasts = true;
        canvasGroup.interactable = true;
        journalInstance.gameObject.SetActive(true);
        if (journalUi != null)
            journalUi.Setup();
        Canvas.ForceUpdateCanvases();
        float height = ((RectTransform)journalInstance.transform).rect.height
            * journalInstance.transform.lossyScale.y / popup.parent.lossyScale.y;
        hiddenPosition = openPosition + Vector2.down * (height + popup.rect.height * popup.localScale.y);
        popup.anchoredPosition = hiddenPosition;
        popup.DOAnchorPos(openPosition, animationDuration).SetEase(Ease.OutCubic).SetUpdate(true);
        foreach (CanvasGroup group in fadeGroups)
        {
            group.DOKill();
            group.alpha = 0f;
            group.DOFade(1f, animationDuration).SetEase(Ease.OutCubic).SetUpdate(true);
        }
    }

    private void CloseJournal()
    {
        if (closing || !journalInstance.gameObject.activeSelf)
            return;
        if (journalUi != null)
            journalUi.FinishJournal();
        else
            DeactiveJournal();
    }

    public void DeactiveJournal()
    {
        if (!journalInstance.gameObject.activeSelf || closing)
            return;

        closing = true;
        popup.DOKill();
        canvasGroup.DOKill();
        canvasGroup.blocksRaycasts = true;
        canvasGroup.interactable = false;
        popup.anchoredPosition = openPosition;
        foreach (CanvasGroup group in fadeGroups)
        {
            group.DOKill();
            group.alpha = 1f;
        }
        TransitionManager.Instance.ReloadSceneTransitionIn();
    }

    public void HideAfterTransition()
    {
        if (journalInstance != null) journalInstance.gameObject.SetActive(false);
    }

    private void OnDestroy()
    {
        foreach (CanvasGroup group in fadeGroups)
            if (group != null) group.DOKill();
        if (journalInstance != null)
        {
            if (popup != null) popup.DOKill();
            if (canvasGroup != null)
                canvasGroup.DOKill();
            journalInstance.CloseButton.onClick.RemoveListener(CloseJournal);
            if (journalUi != null)
                journalUi.DoneRequested -= DeactiveJournal;
        }
    }
}
