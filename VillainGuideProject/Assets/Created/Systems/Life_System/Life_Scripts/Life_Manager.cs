
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
        failedPanel = (RectTransform)Visual.failedContainer_Life.transform;
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

        SetLifeContainers(true, false);
    }

    public void LoseLife(int amount)
    {
        if (amount <= 0 || CurrentLives == 0) return;

        int previousLives = CurrentLives;
        CurrentLives = Mathf.Max(0, CurrentLives - amount);

        RefreshHealth(Visual.healthImages_Life);

        if (Visual.failedContainer_Life.activeSelf &&
            !failedPanelClosing &&
            (failedSequence == null || !failedSequence.IsActive() || !failedSequence.IsPlaying()))
        {
            AnimateFailedHealth();
        }
    }

    public void ShowFailedPanel()
    {
        if (Visual.failedContainer_Life.activeSelf && !failedPanelClosing) return;

        failedSequence?.Kill();
        failedPanelClosing = false;

        Main_Referencer.Instance.manager.barsManager.ShowBars();

        RestoreFailedHealth();
        SetLifeContainers(false, true);

        Canvas.ForceUpdateCanvases();

        failedPanel.anchoredPosition = failedPanelPosition + Vector2.right * FailedPanelDistance();

        failedSequence = DOTween.Sequence();

        failedSequence.Append(DoTweenEffects_Commands.DoTweenAnchorPosX
                (failedPanel, failedPanelPosition.x, animationDuration, Ease.OutCubic));

        failedSequence.OnComplete(AnimateFailedHealth);
    }

    public void HideFailedPanel()
    {
        if (!Visual.failedContainer_Life.activeSelf || failedPanelClosing) return;

        failedSequence?.Kill();
        failedPanelClosing = true;

        Main_Referencer.Instance.manager.barsManager.HideBars();

        failedSequence = DOTween.Sequence();
        
        failedSequence.Append(DoTweenEffects_Commands.DoTweenAnchorPosX
                (failedPanel, failedPanelPosition.x - FailedPanelDistance(), animationDuration, Ease.InCubic));

        failedSequence.OnComplete(() =>
        {
            SetLifeContainers(true, false);
            failedPanel.anchoredPosition = failedPanelPosition;
            failedPanelClosing = false;
        });
    }

    private void SetLifeContainers(bool healthActive, bool failedActive)
    {
        Visual.healthContainer_Life.SetActive(healthActive);
        Visual.failedContainer_Life.SetActive(failedActive);
    }

    private float FailedPanelDistance()
    {
        RectTransform parent = (RectTransform)failedPanel.parent;

        return parent.rect.width + failedPanel.rect.width * Mathf.Abs(failedPanel.localScale.x);
    }

    private void AnimateFailedHealth()
    {
        if (displayedLives <= CurrentLives || failedPanelClosing) return;

        failedSequence = DOTween.Sequence();

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
            image.sprite = i < displayedLives
                ? regularHealthSprites[i]
                : lostHealthSprite;
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

    // Teste!!
    // 1 = perder uma vida e 2 = abrir o painel failed
    private void Update()
    {
        var keyboard = UnityEngine.InputSystem.Keyboard.current;

        if (keyboard == null) return;

        if (keyboard.digit1Key.wasPressedThisFrame) LoseLife(1);
        if (keyboard.digit2Key.wasPressedThisFrame) ShowFailedPanel();
        if (keyboard.digit3Key.wasPressedThisFrame) HideFailedPanel();
    }
}