using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ConfigUiManager : UiButtonManager
{
    private const string FullscreenPreferenceKey = "FullscreenEnabled";
    [SerializeField] private GameObject initialArrow;
    [SerializeField] private GameObject audioArrow;
    [SerializeField] private GameObject videoArrow;
    [SerializeField] private GameObject languageArrow;
    [SerializeField] private GameObject initialTab;
    [SerializeField] private RectTransform audioTab;
    [SerializeField] private RectTransform videoTab;
    [SerializeField] private RectTransform languageTab;
    [SerializeField] private GameObject englishButton;
    [SerializeField] private GameObject portugueseButton;
    [Header("Language Selection")]
    [Min(1f)] public float selectedLanguageWidthMultiplier = 1.4f;
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
    private readonly Dictionary<GameObject, float> languageWidths = new();
    private GameObject selectedLanguageButton;
    private GameObject hoveredLanguageButton;
    [SerializeField, Min(0f)] private float testIconGap = 10f;
    [SerializeField, Min(1f)] private float audioHoverScale = 1.05f;
    [SerializeField, Min(0f)] private float audioHoverDuration = 0.15f;

    public Button CloseButton => closeButton.GetComponent<Button>();

    private void Awake()
    {
        closeInitialScale = closeButton.transform.localScale;
        canvas = GetComponentInParent<Canvas>();
        testIcon.GetComponent<Button>().transition = Selectable.Transition.None;
        if (languageTab != null)
            languageTab.gameObject.SetActive(false);
        RememberLanguageWidth(englishButton);
        RememberLanguageWidth(portugueseButton);
    }

    private void OnEnable()
    {
        bool fullscreen = PlayerPrefs.HasKey(FullscreenPreferenceKey)
            ? PlayerPrefs.GetInt(FullscreenPreferenceKey) == 1
            : true;
        Screen.fullScreen = fullscreen;
        fullscreenToggle.SetIsOnWithoutNotify(fullscreen);
    }

    protected override void Start()
    {
        base.Start();
        audioArrow.SetActive(false);
        videoArrow.SetActive(false);
        if (languageArrow != null)
            languageArrow.SetActive(false);
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
        if (languageTab != null) languageTab.gameObject.SetActive(false);
        audioTab.gameObject.SetActive(true);
    }

    public void OpenVideoTab()
    {
        initialArrow.SetActive(false);
        initialTab.SetActive(false);
        audioTab.gameObject.SetActive(false);
        if (languageTab != null) languageTab.gameObject.SetActive(false);
        videoTab.gameObject.SetActive(true);
    }

    public void OpenLanguageTab()
    {
        initialArrow.SetActive(false);
        initialTab.SetActive(false);
        audioTab.gameObject.SetActive(false);
        videoTab.gameObject.SetActive(false);
        languageTab.gameObject.SetActive(true);
        RefreshSelectedLanguage();
    }

    public void SelectEnglish() => SelectLanguage("en-US");
    public void SelectPortuguese() => SelectLanguage("pt-BR");

    private void SelectLanguage(string code)
    {
        Locale locale = LocalizationSettings.AvailableLocales.GetLocale(new LocaleIdentifier(code));
        if (locale == null)
        {
            Debug.LogWarning($"Language locale '{code}' is not available.");
            return;
        }
        LocalizationSettings.SelectedLocale = locale;
        PlayerPrefs.SetString(LanguagePreference.Key, code);
        PlayerPrefs.Save();
        RefreshSelectedLanguage();
    }

    private void RefreshSelectedLanguage()
    {
        Locale locale = LocalizationSettings.SelectedLocale;
        selectedLanguageButton = locale != null && locale.Identifier.Code.StartsWith("pt", System.StringComparison.OrdinalIgnoreCase)
            ? portugueseButton : englishButton;
        UpdateLanguageAppearance(englishButton);
        UpdateLanguageAppearance(portugueseButton);
        UpdateLanguageArrow();
    }

    private void RememberLanguageWidth(GameObject button)
    {
        if (button != null && button.transform is RectTransform rect)
            languageWidths[button] = rect.rect.width;
    }

    private void UpdateLanguageAppearance(GameObject button)
    {
        if (button == null || !languageWidths.TryGetValue(button, out float width)) return;
        bool selected = button == selectedLanguageButton;
        bool hovered = button == hoveredLanguageButton;
        Button selectable = button.GetComponent<Button>();
        if (selectable != null) selectable.transition = Selectable.Transition.None;
        Image image = button.GetComponent<Image>();
        if (image != null)
        {
            image.CrossFadeColor(Color.white, 0f, true, true);
            image.color = selected ? closeHoverColor : hovered ? Color.white : Color.black;
        }
        TMP_Text label = button.GetComponentInChildren<TMP_Text>(true);
        if (label != null) label.color = selected || hovered ? Color.black : Color.white;
        ((RectTransform)button.transform).SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal,
            width * (selected ? Mathf.Max(1f, selectedLanguageWidthMultiplier) : 1f));
    }

    private void UpdateLanguageArrow()
    {
        if (languageArrow == null || selectedLanguageButton == null || languageTab == null) return;
        languageArrow.SetActive(languageTab.gameObject.activeInHierarchy);
        ((RectTransform)selectedLanguageButton.transform).GetWorldCorners(corners);
        RectTransform arrowImage = (RectTransform)languageArrow.transform.GetChild(0);
        float halfWidth = arrowImage.rect.width * Mathf.Abs(arrowImage.lossyScale.x) * 0.5f;
        Vector3 position = languageArrow.transform.position;
        // A seta da seleção fica dentro do botão, próxima à borda esquerda.
        position.x = corners[0].x + halfWidth + 10f * Mathf.Abs(languageTab.lossyScale.x);
        position.y = selectedLanguageButton.transform.position.y;
        languageArrow.transform.position = position;
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
        if (languageArrow != null) languageArrow.SetActive(false);
        audioTab.gameObject.SetActive(false);
        videoTab.gameObject.SetActive(false);
        if (languageTab != null) languageTab.gameObject.SetActive(false);
        hoveredLanguageButton = null;
        initialTab.SetActive(true);
    }

    public override void MoveArrow(GameObject button)
    {
        if (button == englishButton || button == portugueseButton)
        {
            hoveredLanguageButton = button;
            UpdateLanguageAppearance(englishButton);
            UpdateLanguageAppearance(portugueseButton);
            return;
        }
        if (languageTab != null && button.transform.IsChildOf(languageTab))
        {
            Image image = button.GetComponent<Image>();
            if (image != null) image.color = Color.white;
            TMP_Text label = button.GetComponentInChildren<TMP_Text>(true);
            if (label != null) label.color = Color.black;
            base.ResizeButton(button, true);
            return;
        }
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
        if (button == englishButton || button == portugueseButton)
        {
            if (hoveredLanguageButton == button) hoveredLanguageButton = null;
            UpdateLanguageAppearance(button);
            return;
        }
        if (languageTab != null && button.transform.IsChildOf(languageTab))
        {
            Image image = button.GetComponent<Image>();
            if (image != null) image.color = Color.black;
            TMP_Text label = button.GetComponentInChildren<TMP_Text>(true);
            if (label != null) label.color = Color.white;
            base.ResizeButton(button, false);
            return;
        }
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
            UpdateTestIconPosition();
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

    private void UpdateTestIconPosition()
    {
        if (testIcon == null || audioButtons == null || audioButtons.Length == 0)
            return;

        // The Test SFX label is the row immediately above the test checkbox.
        // Its right edge moves when the label is resized, so keep the checkbox
        // attached to that edge instead of relying on a fixed anchored position.
        RectTransform testLabel = audioButtons[audioButtons.Length - 1];
        testLabel.GetWorldCorners(corners);
        Vector3 position = testIcon.rectTransform.position;
        position.x = corners[2].x + testIcon.rectTransform.rect.width *
            Mathf.Abs(testIcon.rectTransform.lossyScale.x) * 0.5f +
            testIconGap * Mathf.Abs(audioTab.lossyScale.x);
        position.y = testLabel.position.y;
        testIcon.rectTransform.position = position;
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
        PlayerPrefs.SetInt(FullscreenPreferenceKey, enabled ? 1 : 0);
        PlayerPrefs.Save();
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
        if (languageArrow != null) languageArrow.SetActive(false);
        initialArrow.SetActive(false);
        closeButton.color = Color.white;
        closeButton.transform.localScale = closeInitialScale;
        closeAnimation = null;
    }
}
