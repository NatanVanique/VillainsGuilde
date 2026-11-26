using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public sealed class Life_Manager : MonoBehaviour
{
    [Header("Life")] private int playerLives = 3;
    [SerializeField] private Sprite lostHealthSprite;

    [Header("Animation")] private float animationDuration = 0.4f;
    private const float FailedPanelSlideDuration = 0.6f;
    private const float FailedPanelHoldDuration = 0.9f;
    private float arrowAnimationDuration = 1.2f;
    private float animationIntensity = 0.6f;

    private RectTransform failedPanel;
    private Vector2 failedPanelPosition;
    private Image[] failedHealthImages;
    private Sprite[] regularHealthSprites;
    private Sprite[] regularHudSprites;
    private Vector3[] healthIconScales;
    private int displayedLives;
    private Sequence failedSequence;
    private bool failedPanelClosing;
    private Action<bool> failedPanelFinished;
    private Coroutine defeatRoutine;
    private double defeatAt;
    private Tween defeatArrowTween;

    public int CurrentLives { get; private set; }
    internal bool HasUnshownLifeLoss => displayedLives > CurrentLives;

    private VisualReferencer Visual => Main_Referencer.Instance.visual;

    private void Awake()
    {
        CurrentLives = playerLives;
    }

    private void Start()
    {
        failedPanel = (RectTransform)Visual.failedContainer.transform;
        failedPanelPosition = failedPanel.anchoredPosition;
        displayedLives = playerLives;

        regularHudSprites = System.Array.ConvertAll(Visual.healthImages_Life,
            image => image != null ? image.sprite : null);
        failedHealthImages = (Image[])Visual.failedHealthImages_Life.Clone();
        regularHealthSprites = new Sprite[failedHealthImages.Length];
        healthIconScales = new Vector3[failedHealthImages.Length];

        for (int i = 0; i < failedHealthImages.Length; i++)
        {
            Image image = failedHealthImages[i];

            if (image == null) continue;

            regularHealthSprites[i] = image.sprite;
            healthIconScales[i] = image.transform.localScale;
        }

        SetLifeContainers(false);
    }

    internal void CaptureState(GameplaySaveData data)
    {
        data.lives = CurrentLives;
        data.displayedLives = displayedLives;
        data.healthPanelOpen = Visual.healthContainer.activeSelf;
        data.failedPanelOpen = Visual.failedContainer.activeSelf &&
                               (!failedPanelClosing || failedPanelFinished != null);
        data.defeatPanelOpen = Visual.defeatContainer.activeSelf;
        data.defeatSecondsRemaining = defeatRoutine == null
            ? -1f
            : Mathf.Max(0f, (float)(defeatAt - Time.timeAsDouble));
        data.defeatArrowVisible = Visual.arrowDefeatScreen.activeSelf;
        data.defeatArrowElapsed = defeatArrowTween.IsActive()
            ? defeatArrowTween.Elapsed()
            : arrowAnimationDuration;
    }

    internal void RestoreState(GameplaySaveData data)
    {
        if (defeatRoutine != null) StopCoroutine(defeatRoutine);
        defeatRoutine = null;
        failedSequence?.Kill();
        failedPanelClosing = false;

        CurrentLives = Mathf.Clamp(data.lives, 0, playerLives);
        displayedLives = Mathf.Clamp(data.displayedLives, CurrentLives, playerLives);
        for (int i = 0; i < Visual.healthImages_Life.Length; i++)
        {
            Image image = Visual.healthImages_Life[i];
            if (image != null)
                image.sprite = i < displayedLives ? regularHudSprites[i] : lostHealthSprite;
        }

        Visual.healthContainer.SetActive(data.healthPanelOpen);
        failedPanel.anchoredPosition = failedPanelPosition;
        RestoreFailedHealth();
        SetLifeContainers(data.failedPanelOpen);
        if (data.failedPanelOpen) AnimateFailedHealth();

        Visual.defeatContainer.SetActive(data.defeatPanelOpen);
        Visual.arrowDefeatScreen.SetActive(data.defeatArrowVisible);
        if (data.defeatArrowVisible && data.defeatPanelOpen)
        {
            AnimateDefeatArrow();
            defeatArrowTween.Goto(data.defeatArrowElapsed, true);
        }

        if (data.defeatSecondsRemaining >= 0f)
            defeatRoutine = StartCoroutine(GameOverVisualSequence(data.defeatSecondsRemaining));
    }

    public void LoseLife(int amount)
    {
        if (amount <= 0 || CurrentLives == 0) return;

        CurrentLives = Mathf.Max(0, CurrentLives - amount);
        GameSaveSystem.RequestAutosave();

        if (Visual.failedContainer.activeSelf &&
            !failedPanelClosing &&
            (failedSequence == null || !failedSequence.IsActive() || !failedSequence.IsPlaying()))
        {
            AnimateFailedHealth();
        }
    }

    public void ShowFailedPanel()
    {
        if (Visual.failedContainer.activeSelf && !failedPanelClosing) return;

        failedSequence?.Kill();
        failedPanelClosing = false;

        RestoreFailedHealth();
        SetLifeContainers(true);

        Canvas.ForceUpdateCanvases();

        failedPanel.anchoredPosition = failedPanelPosition + Vector2.right * FailedPanelDistance();

        failedSequence = DOTween.Sequence().SetUpdate(true);

        failedSequence.Append(DoTweenEffects_Commands.DoTweenAnchorPosX
            (failedPanel, failedPanelPosition.x, FailedPanelSlideDuration, Ease.OutCubic));

        failedSequence.OnComplete(AnimateFailedHealth);
    }

    internal void PlayFailedPanel(Action<bool> onFinished)
    {
        failedPanelFinished = onFinished;
        if (!Visual.failedContainer.activeSelf || failedPanelClosing)
            ShowFailedPanel();
        else if (failedSequence == null || !failedSequence.IsActive() || !failedSequence.IsPlaying())
            AnimateFailedHealth();
    }

    public void HideFailedPanel()
    {
        if (failedPanelFinished != null) return;
        CloseFailedPanel();
    }

    private void CloseFailedPanel()
    {
        if (!Visual.failedContainer.activeSelf || failedPanelClosing) return;

        failedSequence?.Kill();
        failedPanelClosing = true;

        failedSequence = DOTween.Sequence().SetUpdate(true);

        if (failedPanelFinished != null)
            failedSequence.AppendInterval(FailedPanelHoldDuration);

        failedSequence.Append(DoTweenEffects_Commands.DoTweenAnchorPosX
            (failedPanel, failedPanelPosition.x - FailedPanelDistance(), FailedPanelSlideDuration, Ease.InCubic));

        failedSequence.OnComplete(() =>
        {
            SetLifeContainers(false);
            failedPanel.anchoredPosition = failedPanelPosition;
            failedPanelClosing = false;

            Action<bool> finished = failedPanelFinished;
            failedPanelFinished = null;
            if (finished == null) return;

            bool survived = CurrentLives > 0;
            finished(survived);
            if (!survived) GameOver();
        });
    }

    private void SetLifeContainers(bool failedActive)
    {
        Visual.failedContainer.SetActive(failedActive);
    }

    private float FailedPanelDistance()
    {
        RectTransform parent = (RectTransform)failedPanel.parent;

        return parent.rect.width + failedPanel.rect.width * Mathf.Abs(failedPanel.localScale.x);
    }

    private void AnimateFailedHealth()
    {
        if (failedPanelClosing) return;
        if (displayedLives <= CurrentLives)
        {
            if (failedPanelFinished != null) CloseFailedPanel();
            return;
        }

        failedSequence = DOTween.Sequence().SetUpdate(true);

        for (int i = displayedLives - 1; i >= CurrentLives; i--)
        {
            int remainingLives = i;
            Image image = i < failedHealthImages.Length ? failedHealthImages[i] : null;

            if (image != null)
            {
                failedSequence.Append(DoTweenEffects_Commands.DoTweenLoseHealth(
                    image, lostHealthSprite, healthIconScales[i], animationDuration, animationIntensity,
                    () => RevealLostLife(remainingLives)));
            }
            else
            {
                failedSequence.AppendCallback(() => RevealLostLife(remainingLives));
            }
        }

        failedSequence.OnComplete(AnimateFailedHealth);
    }

    private void RestoreFailedHealth()
    {
        if (failedHealthImages == null ||
            healthIconScales == null ||
            regularHealthSprites == null) return;

        for (int i = 0; i < failedHealthImages.Length; i++)
        {
            Image image = failedHealthImages[i];

            if (image == null) continue;

            image.transform.localScale = healthIconScales[i];
            image.sprite = i < displayedLives ? regularHealthSprites[i] : lostHealthSprite;
        }
    }

    private void OnDisable()
    {
        failedSequence?.Kill();
        failedPanelFinished = null;
        if (failedPanel == null) return;
        failedPanel.anchoredPosition = failedPanelPosition;
        failedPanel.gameObject.SetActive(false);
        failedPanelClosing = false;
        RestoreFailedHealth();
    }

    private void RevealLostLife(int remainingLives)
    {
        displayedLives = remainingLives;
        Image[] hudImages = Visual.healthImages_Life;
        if (remainingLives < hudImages.Length && hudImages[remainingLives] != null)
        {
            hudImages[remainingLives].sprite = lostHealthSprite;
        }
    }

    private void GameOver()
    {
        if (defeatRoutine != null) StopCoroutine(defeatRoutine);
        defeatRoutine = StartCoroutine(GameOverVisualSequence(2f));
        GameSaveSystem.RequestAutosave();
    }

    public void AnimateDefeatArrow()
    {
        RectTransform arrow = (RectTransform)Visual.arrowDefeatScreen.transform;

        arrow.DOKill();
        arrow.anchoredPosition = new Vector2(-1691f, 968f);
        arrow.gameObject.SetActive(true);

        defeatArrowTween = arrow.DOAnchorPos(new Vector2(-235f, 18f), arrowAnimationDuration)
            .SetEase(Ease.OutCubic)
            .SetLink(arrow.gameObject, LinkBehaviour.KillOnDisable);
    }

    private IEnumerator GameOverVisualSequence(float remaining)
    {
        Visual.fadeAnimator.SetTrigger("FadeIn");
        defeatAt = Time.timeAsDouble + remaining;
        yield return new WaitForSeconds(remaining);

        RectTransform arrow = (RectTransform)Visual.arrowDefeatScreen.transform;
        arrow.DOKill();
        arrow.anchoredPosition = new Vector2(-1691f, 968f);

        Visual.defeatContainer.SetActive(true);
        Visual.fadeAnimator.SetTrigger("FadeOut");
        AnimateDefeatArrow();
        defeatRoutine = null;
    }
}
