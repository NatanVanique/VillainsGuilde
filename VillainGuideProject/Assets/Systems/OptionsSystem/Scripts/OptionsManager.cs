using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using DG.Tweening;

public class OptionsManager : MonoBehaviour
{
    [SerializeField] private RectTransform optionsCanvas;
    [SerializeField] private float animationDuration = 0.9f;
    private RectTransform popup;
    private Vector3 openScale;
    private CanvasGroup popupGroup;
    private CanvasGroup backgroundFade;
    private bool closing;

    private Button closeButton;
    private Button optionsButton;
    private CursorLockMode previousCursorLock;
    private bool previousCursorVisible;

    private void Awake()
    {
        Transform background = optionsCanvas.Find("BackgroundFade");
        if (background != null)
        {
            backgroundFade = background.GetComponent<CanvasGroup>();
            if (backgroundFade == null) backgroundFade = background.gameObject.AddComponent<CanvasGroup>();
        }
        popup = PopupAnimationTarget.Create(optionsCanvas, background);
        openScale = popup.localScale;
        popupGroup = optionsCanvas.GetComponent<CanvasGroup>();
        if (popupGroup == null) popupGroup = optionsCanvas.gameObject.AddComponent<CanvasGroup>();
        optionsCanvas.gameObject.SetActive(false);
        closeButton = optionsCanvas.GetComponentInChildren<ConfigUiManager>(true).CloseButton;
        closeButton.onClick.AddListener(CloseOptions);

        foreach (Button button in FindObjectsByType<Button>(FindObjectsInactive.Include, FindObjectsSortMode.None))
        {
            if (button.name == "OptionButton")
            {
                optionsButton = button;
                optionsButton.onClick.AddListener(OpenOptions);
                break;
            }
        }
    }

    private void Update()
    {
        if (Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            if (optionsCanvas.gameObject.activeSelf)
                CloseOptions();
            else
                OpenOptions();
        }
    }

    public void OpenOptions()
    {
        if (optionsCanvas.gameObject.activeSelf)
            return;

        previousCursorLock = Cursor.lockState;
        previousCursorVisible = Cursor.visible;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        closing = false;
        popup.DOKill();
        popup.localScale = Vector3.zero;
        popupGroup.interactable = true;
        popupGroup.blocksRaycasts = true;
        if (backgroundFade != null)
        {
            backgroundFade.DOKill();
            backgroundFade.alpha = 0f;
        }
        optionsCanvas.gameObject.SetActive(true);
        popup.DOScale(openScale, animationDuration).SetEase(Ease.OutCubic).SetUpdate(true);
        if (backgroundFade != null)
            backgroundFade.DOFade(1f, animationDuration).SetEase(Ease.OutCubic).SetUpdate(true);
    }

    public void CloseOptions()
    {
        if (closing || !optionsCanvas.gameObject.activeSelf) return;
        closing = true;
        popupGroup.interactable = false;
        popup.DOKill();
        if (backgroundFade != null)
        {
            backgroundFade.DOKill();
            backgroundFade.DOFade(0f, animationDuration).SetEase(Ease.InCubic).SetUpdate(true);
        }
        popup.DOScale(Vector3.zero, animationDuration).SetEase(Ease.InBack).SetUpdate(true)
            .OnComplete(() =>
            {
                optionsCanvas.gameObject.SetActive(false);
                Cursor.lockState = previousCursorLock;
                Cursor.visible = previousCursorVisible;
                closing = false;
            });
    }

    private void OnDestroy()
    {
        if (popup != null) popup.DOKill();
        if (backgroundFade != null) backgroundFade.DOKill();
        if (closeButton != null)
            closeButton.onClick.RemoveListener(CloseOptions);
        if (optionsButton != null)
            optionsButton.onClick.RemoveListener(OpenOptions);
    }
}

// Overlay canvas transforms are driven by Unity; animate a child container instead.
internal static class PopupAnimationTarget
{
    public static RectTransform Create(RectTransform root, Transform excluded = null)
    {
        if (root.localScale.sqrMagnitude < 0.001f) root.localScale = Vector3.one;
        Canvas.ForceUpdateCanvases();
        int childCount = root.childCount;
        var children = new Transform[childCount];
        for (int i = 0; i < childCount; i++) children[i] = root.GetChild(i);
        var target = new GameObject("Popup Animation", typeof(RectTransform)).GetComponent<RectTransform>();
        target.SetParent(root, false);
        target.anchorMin = Vector2.zero;
        target.anchorMax = Vector2.one;
        target.sizeDelta = Vector2.zero;
        foreach (Transform child in children)
            if (child != excluded) child.SetParent(target, true);
        return target;
    }
}
