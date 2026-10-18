using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ConfigUiManager : UiButtonManager
{
    [SerializeField] private GameObject initialArrow;
    [SerializeField] private GameObject audioArrow;
    [SerializeField] private GameObject videoArrow;
    [SerializeField] private GameObject initialTab;
    [SerializeField] private RectTransform audioTab;
    [SerializeField] private RectTransform videoTab;
    [SerializeField] private RectTransform fullscreenButton;
    [SerializeField] private Toggle fullscreenToggle;
    [SerializeField] private RectTransform[] audioButtons;
    [SerializeField] private RectTransform[] audioSliders;
    [SerializeField] private Image testIcon;
    [SerializeField] private Image closeButton;
    [SerializeField] private Color closeHoverColor = new Color(1f, 0.4f, 0.65f, 1f);
    [SerializeField, Min(1f)] private float closeHoverScale = 1.1f;
    [SerializeField, Min(0f)] private float closeHoverDuration = 0.15f;

    private Vector3 closeInitialScale;
    private Coroutine closeAnimation;
    private GameObject hoveredAudioButton;
    private bool fullscreenHovered;
    private Canvas canvas;
    private readonly Vector3[] corners = new Vector3[4];
    private readonly Dictionary<RectTransform, Vector3> audioInitialScales = new();
    private readonly Dictionary<RectTransform, Coroutine> audioResizeRoutines = new();
    [SerializeField, Min(1f)] private float audioHoverScale = 1.05f;
    [SerializeField, Min(0f)] private float audioHoverDuration = 0.15f;

    public Button CloseButton => closeButton.GetComponent<Button>();

    private void Awake()
    {
        closeInitialScale = closeButton.transform.localScale;
        canvas = GetComponentInParent<Canvas>();
        testIcon.GetComponent<Button>().transition = Selectable.Transition.None;
    }

    private void OnEnable()
    {
        fullscreenToggle.SetIsOnWithoutNotify(Screen.fullScreen);
    }

    protected override void Start()
    {
        base.Start();
        audioArrow.SetActive(false);
        videoArrow.SetActive(false);
        foreach (RectTransform button in audioButtons)
        {
            audioInitialScales.Add(button, button.localScale);
            float shift = (1f - button.pivot.x) * button.rect.width * button.localScale.x;
            button.pivot = new Vector2(1f, button.pivot.y);
            button.anchoredPosition += new Vector2(shift, 0f);
        }
    }

    protected override void ResizeButton(GameObject button, bool highlighted)
    {
        RectTransform rect = (RectTransform)button.transform;
        if (!audioInitialScales.TryGetValue(rect, out Vector3 originalScale))
        {
            base.ResizeButton(button, highlighted);
            return;
        }

        if (audioResizeRoutines.TryGetValue(rect, out Coroutine previous) && previous != null)
            StopCoroutine(previous);
        audioResizeRoutines[rect] = StartCoroutine(AnimateAudioWidth(rect, originalScale, highlighted));
    }

    private IEnumerator AnimateAudioWidth(RectTransform rect, Vector3 originalScale, bool highlighted)
    {
        Vector3 start = rect.localScale;
        Vector3 target = originalScale;
        target.x *= highlighted ? audioHoverScale : 1f;
        float elapsed = 0f;
        while (elapsed < audioHoverDuration)
        {
            elapsed += Time.unscaledDeltaTime;
            rect.localScale = Vector3.Lerp(start, target, elapsed / audioHoverDuration);
            yield return null;
        }
        rect.localScale = target;
    }

    public void OpenAudioTab()
    {
        initialArrow.SetActive(false);
        initialTab.SetActive(false);
        videoTab.gameObject.SetActive(false);
        audioTab.gameObject.SetActive(true);
    }

    public void OpenVideoTab()
    {
        initialArrow.SetActive(false);
        initialTab.SetActive(false);
        audioTab.gameObject.SetActive(false);
        videoTab.gameObject.SetActive(true);
    }

    public void ReturnToInitialTab()
    {
        if (hoveredAudioButton != null)
            base.DisableArrow(hoveredAudioButton);
        hoveredAudioButton = null;
        audioArrow.SetActive(false);
        if (fullscreenHovered)
            base.DisableArrow(fullscreenButton.gameObject);
        fullscreenHovered = false;
        videoArrow.SetActive(false);
        audioTab.gameObject.SetActive(false);
        videoTab.gameObject.SetActive(false);
        initialTab.SetActive(true);
    }

    public override void MoveArrow(GameObject button)
    {
        bool inAudioTab = button.transform.IsChildOf(audioTab);
        SetArrow(inAudioTab ? audioArrow :
            button.transform.IsChildOf(videoTab) ? videoArrow : initialArrow);
        if (inAudioTab)
        {
            ((RectTransform)button.transform).GetWorldCorners(corners);
            RectTransform arrowImage = (RectTransform)audioArrow.transform.GetChild(0);
            float halfArrowWidth = arrowImage.rect.width * Mathf.Abs(arrowImage.lossyScale.x) * 0.5f;
            float gap = 5f * Mathf.Abs(audioTab.lossyScale.x);
            Vector3 position = audioArrow.transform.position;
            position.x = corners[0].x - halfArrowWidth - gap;
            audioArrow.transform.position = position;
        }
        base.MoveArrow(button);
    }

    public override void DisableArrow(GameObject button)
    {
        if (button == fullscreenButton.gameObject)
            return;
        foreach (RectTransform audioButton in audioButtons)
        {
            if (button == audioButton.gameObject)
                return;
        }
        base.DisableArrow(button);
    }

    private void LateUpdate()
    {
        GameObject nextButton = null;
        bool testHovered = false;
        bool nextFullscreenHovered = false;
        if (videoTab.gameObject.activeInHierarchy && Mouse.current != null)
        {
            Rect labelRect = ScreenRect(fullscreenButton);
            Rect boxRect = ScreenRect((RectTransform)fullscreenToggle.transform);
            Rect row = Rect.MinMaxRect(
                Mathf.Min(labelRect.xMin, boxRect.xMin), Mathf.Min(labelRect.yMin, boxRect.yMin),
                Mathf.Max(labelRect.xMax, boxRect.xMax), Mathf.Max(labelRect.yMax, boxRect.yMax));
            nextFullscreenHovered = row.Contains(Mouse.current.position.ReadValue());
        }

        if (fullscreenHovered != nextFullscreenHovered)
        {
            fullscreenHovered = nextFullscreenHovered;
            if (fullscreenHovered)
                MoveArrow(fullscreenButton.gameObject);
            else
                base.DisableArrow(fullscreenButton.gameObject);
        }
        if (audioTab.gameObject.activeInHierarchy && Mouse.current != null)
        {
            Vector2 mouse = Mouse.current.position.ReadValue();
            testHovered = ScreenRect(testIcon.rectTransform).Contains(mouse);
            float closest = float.PositiveInfinity;
            for (int i = 0; i < audioButtons.Length; i++)
            {
                Rect buttonRect = ScreenRect(audioButtons[i]);
                Rect sliderRect = ScreenRect(audioSliders[i]);
                Rect row = Rect.MinMaxRect(
                    Mathf.Min(buttonRect.xMin, sliderRect.xMin),
                    Mathf.Min(buttonRect.yMin, sliderRect.yMin),
                    Mathf.Max(buttonRect.xMax, sliderRect.xMax),
                    Mathf.Max(buttonRect.yMax, sliderRect.yMax));
                float distance = Mathf.Abs(mouse.y - buttonRect.center.y);
                if (row.Contains(mouse) && distance < closest)
                {
                    closest = distance;
                    nextButton = audioButtons[i].gameObject;
                }
            }
        }

        testIcon.color = testHovered ? closeHoverColor : Color.white;

        if (hoveredAudioButton == nextButton)
            return;

        if (hoveredAudioButton != null)
            base.DisableArrow(hoveredAudioButton);
        hoveredAudioButton = nextButton;
        if (hoveredAudioButton != null)
            MoveArrow(hoveredAudioButton);
    }

    private Rect ScreenRect(RectTransform rect)
    {
        rect.GetWorldCorners(corners);
        Camera eventCamera = canvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : canvas.worldCamera;
        Vector2 min = RectTransformUtility.WorldToScreenPoint(eventCamera, corners[0]);
        Vector2 max = RectTransformUtility.WorldToScreenPoint(eventCamera, corners[2]);
        return Rect.MinMaxRect(min.x, min.y, max.x, max.y);
    }

    public void LoadMainMenu()
    {
        SceneManager.LoadScene("01_MenuScene");
    }

    public void SetFullscreen(bool enabled)
    {
        Screen.fullScreen = enabled;
    }

    public void ShowMenuConfirmation(GameObject button)
    {
        ShowConfirmation(button, "Menu", LoadMainMenu);
    }

    public void HighlightClose()
    {
        SetCloseHighlight(true);
    }

    public void ResetClose()
    {
        SetCloseHighlight(false);
    }

    private void SetCloseHighlight(bool highlighted)
    {
        if (closeAnimation != null)
            StopCoroutine(closeAnimation);
        closeAnimation = StartCoroutine(AnimateClose(highlighted));
    }

    private IEnumerator AnimateClose(bool highlighted)
    {
        Color startColor = closeButton.color;
        Vector3 startScale = closeButton.transform.localScale;
        Color targetColor = highlighted ? closeHoverColor : Color.white;
        Vector3 targetScale = closeInitialScale * (highlighted ? closeHoverScale : 1f);
        float elapsed = 0f;
        while (elapsed < closeHoverDuration)
        {
            elapsed += Time.unscaledDeltaTime;
            float progress = Mathf.Clamp01(elapsed / closeHoverDuration);
            closeButton.color = Color.Lerp(startColor, targetColor, progress);
            closeButton.transform.localScale = Vector3.Lerp(startScale, targetScale, progress);
            yield return null;
        }
        closeButton.color = targetColor;
        closeButton.transform.localScale = targetScale;
        closeAnimation = null;
    }

    private void OnDisable()
    {
        hoveredAudioButton = null;
        fullscreenHovered = false;
        foreach (KeyValuePair<RectTransform, Vector3> entry in audioInitialScales)
            entry.Key.localScale = entry.Value;
        audioResizeRoutines.Clear();
        testIcon.color = Color.white;
        audioArrow.SetActive(false);
        videoArrow.SetActive(false);
        initialArrow.SetActive(false);
        closeButton.color = Color.white;
        closeButton.transform.localScale = closeInitialScale;
        closeAnimation = null;
    }
}
