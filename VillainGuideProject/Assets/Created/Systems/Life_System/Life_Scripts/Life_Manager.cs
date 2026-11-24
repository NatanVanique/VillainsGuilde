using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public sealed class LifeManager : MonoBehaviour
{
    [Header("Life")]
    private int playerLives = 3;
    [SerializeField] private Sprite lostHealthSprite;

    [Header("Animation")]
    private float animationDuration = 0.4f;
    private float arrowAnimationDuration = 1.2f;
    private float animationIntensity = 0.6f;

    private RectTransform failedPanel;
    private Vector2 failedPanelPosition;
    private Image[] failedHealthImages;
    private Sprite[] regularHealthSprites;
    private Vector3[] healthIconScales;
    private int displayedLives;
    private Sequence failedSequence;
    private bool failedPanelClosing;

    public int CurrentLives { get; private set; }

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

        RefreshHealth(Visual.healthImages_Life);
        RefreshHealth(Visual.failedHealthImages_Life);

        SetLifeContainers(false);
    }

    public void LoseLife(int amount)
    {
        if (amount <= 0 || CurrentLives == 0) return;

        int previousLives = CurrentLives;
        CurrentLives = Mathf.Max(0, CurrentLives - amount);

        RefreshHealth(Visual.healthImages_Life);

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
                (failedPanel, failedPanelPosition.x, animationDuration, Ease.OutCubic));

        failedSequence.OnComplete(AnimateFailedHealth);
    }

    public void HideFailedPanel()
    {
        if (!Visual.failedContainer.activeSelf || failedPanelClosing) return;

        failedSequence?.Kill();
        failedPanelClosing = true;

        failedSequence = DOTween.Sequence().SetUpdate(true);
        
        failedSequence.Append(DoTweenEffects_Commands.DoTweenAnchorPosX
                (failedPanel, failedPanelPosition.x - FailedPanelDistance(), animationDuration, Ease.InCubic));

        failedSequence.OnComplete(() =>
        {
            SetLifeContainers(false);
            failedPanel.anchoredPosition = failedPanelPosition;
            failedPanelClosing = false;
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
        if (displayedLives <= CurrentLives || failedPanelClosing) return;

        failedSequence = DOTween.Sequence().SetUpdate(true);

        for (int i = displayedLives - 1; i >= CurrentLives; i--)
        {
            int remainingLives = i;
            Image image = i < failedHealthImages.Length ? failedHealthImages[i] : null;
            
           failedSequence.Append(DoTweenEffects_Commands.DoTweenLoseHealth
               (image, lostHealthSprite, healthIconScales[i], animationDuration, animationIntensity));
            failedSequence.AppendCallback(() => displayedLives = remainingLives);
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
        if (failedPanel == null) return;
        failedPanel.anchoredPosition = failedPanelPosition;
        failedPanel.gameObject.SetActive(false);
        failedPanelClosing = false;
        RestoreFailedHealth();
    }

    private void RefreshHealth(Image[] images)
    {
        if (images == null) return;

        for (int i = 0; i < images.Length; i++)
        {
            if (images[i] == null || i < CurrentLives) continue;

            images[i].sprite = lostHealthSprite;
        }
    }

    private void GameOver()
    {
        StartCoroutine(GameOverVisualSequence());
    }

    public void AnimateDefeatArrow()
    {
        RectTransform arrow = (RectTransform)Visual.arrowDefeatScreen.transform;

        arrow.DOKill();
        arrow.anchoredPosition = new Vector2(-1691f, 968f);
        arrow.gameObject.SetActive(true);

        arrow.DOAnchorPos(new Vector2(-235f, 18f), arrowAnimationDuration)
            .SetEase(Ease.OutCubic)
            .SetLink(arrow.gameObject, LinkBehaviour.KillOnDisable);
    }

    private IEnumerator GameOverVisualSequence()
    {
        Visual.fadeAnimator.SetTrigger("FadeIn");
        yield return new WaitForSeconds(2);
        
        RectTransform arrow = (RectTransform)Visual.arrowDefeatScreen.transform;
        arrow.DOKill();
        arrow.anchoredPosition = new Vector2(-1691f, 968f);

        Visual.defeatContainer.SetActive(true);
        Visual.fadeAnimator.SetTrigger("FadeOut");
        AnimateDefeatArrow();
    }

    // Teste!!
    // 1 = perder uma vida, 2 = abrir o painel failed,
    // 3 = fecha painel failed, 4 = game over
    private void Update()
    {
        var keyboard = UnityEngine.InputSystem.Keyboard.current;

        if (keyboard == null) return;

        if (keyboard.digit1Key.wasPressedThisFrame) LoseLife(1);
        if (keyboard.digit2Key.wasPressedThisFrame) ShowFailedPanel();
        if (keyboard.digit3Key.wasPressedThisFrame) HideFailedPanel();
        if (keyboard.digit4Key.wasPressedThisFrame) GameOver();
    }
}
