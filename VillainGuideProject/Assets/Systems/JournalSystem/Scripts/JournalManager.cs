using UnityEngine;
using DG.Tweening;
using System.Collections.Generic;

public class JournalManager : MonoBehaviour
{
    [Header("References")]
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
    private readonly List<CanvasGroup> fadeGroups = new();

    public void Setup()
    {
        journalInstance = Instantiate(journalPrefab, transform).GetComponent<JournalView>();
        journalUi = journalInstance.GetComponentInChildren<JournalUi>(true);
        canvasGroup = journalInstance.GetComponent<CanvasGroup>();
        if (canvasGroup == null)
            canvasGroup = journalInstance.gameObject.AddComponent<CanvasGroup>();
        journalInstance.transform.localScale = Vector3.one;
        popup = (RectTransform)journalInstance.AnimationTarget;

        //Fade 
        foreach (RectTransform target in journalInstance.FadeTargets)
        {
            CanvasGroup group = target.GetComponent<CanvasGroup>();
            if (group == null) group = target.gameObject.AddComponent<CanvasGroup>();
            fadeGroups.Add(group);
        }

        // Close Journal Anim
        openPosition = popup.anchoredPosition;
        openScale = popup.localScale;
        if (openScale.sqrMagnitude < 0.001f)
            openScale = Vector3.one;

        journalInstance.CloseButton.onClick.AddListener(CloseJournal);
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
        journalUi.Setup();

        // Anim
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
        journalUi.FinishJournal();
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

        // Close Anim
        foreach (CanvasGroup group in fadeGroups)
        {
            group.DOKill();
            group.DOFade(0f, animationDuration).SetEase(Ease.InCubic).SetUpdate(true);
        }

        popup.DOAnchorPos(hiddenPosition, animationDuration)
            .SetEase(Ease.InCubic)
            .SetUpdate(true)
            .OnComplete(FinishClosingJournal);
    }

    private void FinishClosingJournal()
    {
        HideAfterTransition();
        if (EndingsManager.ShouldEndRun)
        {
            SingleEntryPoint.Instance.EndingsManager.ShowEnding();
            return;
        }
        ContinueAfterJournal();
    }

    public void ShowJournalAfterHealthLoss()
    {
        HealthManager health = SingleEntryPoint.Instance.ManagerManager.HealthMan;
        if (health == null || health.PendingHealthLoss <= 0)
        {
            ActiveJournal();
            return;
        }

        LoseHealthUI lossScreen = FindFirstObjectByType<LoseHealthUI>(FindObjectsInactive.Include);
        if (lossScreen == null)
        {
            ActiveJournal();
            return;
        }

        int lostHealth = health.ConsumePendingHealthLoss();
        lossScreen.Show(health.CurrentHealth + lostHealth, health.CurrentHealth, ActiveJournal);
    }

    public void ContinueAfterJournal()
    {
        if (!closing || journalInstance.gameObject.activeSelf ||
            MonsterUiDisplay.IsShowing || EndingsManager.ShouldEndRun)
            return;

        PhoneController.Instance.PhoneRing();
    }

    public void HideAfterTransition()
    {
        journalInstance.gameObject.SetActive(false);
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
