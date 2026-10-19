using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class EndingScreenUi : UiButtonManager
{
    [SerializeField] private bool victory;
    [SerializeField] private Image background;
    [SerializeField] private RectTransform title;
    [SerializeField, Min(0.01f)] private float backgroundDuration = 0.8f;
    [SerializeField, Min(0.01f)] private float titleDuration = 0.7f;
    private Graphic titleFade;
    private float backgroundAlpha;
    private float titleAlpha;
    private Vector2 titlePosition;
    private Vector3 titleScale;
    private Sequence entrance;
    private bool initialized;

    protected override void Start() { }

    private void Initialize()
    {
        if (initialized) return;
        initialized = true;
        base.Start();
        titlePosition = title.anchoredPosition;
        titleScale = title.localScale;
        titleFade = title.GetComponent<Graphic>();
        backgroundAlpha = background.color.a;
        titleAlpha = titleFade.color.a;
    }

    public void MainMenu()
    {
        TransitionManager.Instance.LoadSceneAsync("01_MenuScene");
    }

    public void Prepare()
    {
        gameObject.SetActive(true);
        Initialize();
        entrance?.Kill();
        Color backgroundColor = background.color;
        backgroundColor.a = 0f;
        background.color = backgroundColor;
        Color titleColor = titleFade.color;
        titleColor.a = 0f;
        titleFade.color = titleColor;
        title.localScale = victory ? Vector3.zero : titleScale * 1.08f;
        title.anchoredPosition = titlePosition + (victory ? Vector2.zero : Vector2.up * 80f);
    }

    public void AnimateEntrance()
    {
        entrance = DOTween.Sequence().SetUpdate(true);
        entrance.Append(background.DOFade(backgroundAlpha, backgroundDuration).SetEase(Ease.OutSine));
        entrance.Append(titleFade.DOFade(titleAlpha, titleDuration));
        entrance.Join(title.DOScale(titleScale, titleDuration).SetEase(victory ? Ease.OutBack : Ease.OutQuad));
        if (!victory)
            entrance.Join(title.DOAnchorPos(titlePosition, titleDuration).SetEase(Ease.InQuad));
    }

    private void OnDisable() { entrance?.Kill(); }
}
