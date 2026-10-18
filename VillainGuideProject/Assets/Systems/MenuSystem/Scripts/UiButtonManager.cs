using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public abstract class UiButtonManager : MonoBehaviour
{
    [SerializeField] private GameObject arrow;
    [SerializeField, Min(1f)] private float hoverWidthMultiplier = 1.2f;
    [SerializeField, Min(0f)] private float resizeDuration = 0.15f;

    private readonly Dictionary<GameObject, float> originalWidths = new Dictionary<GameObject, float>();
    private readonly Dictionary<GameObject, Coroutine> resizeRoutines = new Dictionary<GameObject, Coroutine>();
    private GameObject highlightedButton;
    private readonly Dictionary<GameObject, QuitConfirmation> confirmations = new();

    protected virtual void Start()
    {
        arrow.SetActive(false);
        LocalizeCanvas();
    }

    private void LocalizeCanvas()
    {
        Canvas canvas = GetComponentInParent<Canvas>();
        if (canvas == null)
            return;

        foreach (TMP_Text label in canvas.GetComponentsInChildren<TMP_Text>(true))
        {
            string key = label.text.Trim() switch
            {
                "New Game" => "Menu_NewGame",
                "Quit" => "Menu_Quit",
                "Credits" => "Menu_Credits",
                "Options" => "Options_Title",
                "Return" => "Options_Return",
                "Fullscreen" => "Options_Fullscreen",
                "Effects" => "Options_Effects",
                "Test SFX" => "Options_TestSfx",
                "Video" => "Options_Video",
                "Áudio" => "Options_Audio",
                "Audio" => "Options_Audio",
                "Main" => "Options_MainVolume",
                "Main Menu" => "Options_MainMenu",
                "Quit Game" => "Options_QuitGame",
                "Music" => "Options_Music",
                _ => null
            };
            if (key != null)
                UiLocalizedText.Bind(label, key);
        }
    }

    public void ShowQuitConfirmation(GameObject button)
    {
        ShowConfirmation(button, "Quit", Application.Quit);
    }

    protected void ShowConfirmation(GameObject button, string label, UnityEngine.Events.UnityAction action)
    {
        foreach (QuitConfirmation activeConfirmation in confirmations.Values)
            activeConfirmation.CloseForOtherButton(button);

        if (!confirmations.TryGetValue(button, out QuitConfirmation confirmation))
        {
            confirmation = gameObject.AddComponent<QuitConfirmation>();
            confirmation.Initialize(this, button, label, action);
            confirmations.Add(button, confirmation);
        }
        confirmation.ShowConfirmation();
    }

    protected void SetArrow(GameObject newArrow)
    {
        if (arrow == newArrow)
            return;

        arrow.SetActive(false);
        arrow = newArrow;
    }

    public virtual void MoveArrow(GameObject button)
    {
        foreach (QuitConfirmation confirmation in confirmations.Values)
            confirmation.CloseForOtherButton(button);

        if (highlightedButton != null && highlightedButton != button)
            ResetButton(highlightedButton);

        highlightedButton = button;
        arrow.SetActive(true);
        Vector3 position = arrow.transform.position;
        position.y = button.transform.position.y;
        arrow.transform.position = position;

        Button selectable = button.GetComponent<Button>();
        if (selectable != null)
            selectable.transition = Selectable.Transition.None;
        Image image = button.GetComponent<Image>();
        image.CrossFadeColor(Color.white, 0f, true, true);
        image.color = Color.white;
        TMP_Text text = button.GetComponentInChildren<TMP_Text>(true);
        text.color = Color.black;
        ResizeButton(button, true);
    }

    public virtual void DisableArrow(GameObject button)
    {
        ResetButton(button);
        if (highlightedButton == button)
        {
            highlightedButton = null;
            arrow.SetActive(false);
        }
    }

    private void ResetButton(GameObject button)
    {
        Image image = button.GetComponent<Image>();
        image.color = button.name == "CancelConfirmationButton" ? Color.red : Color.black;
        TMP_Text text = button.GetComponentInChildren<TMP_Text>(true);
        text.color = Color.white;
        ResizeButton(button, false);
    }

    public void ClearHighlight(GameObject original, GameObject confirm, GameObject cancel)
    {
        if (highlightedButton != original && highlightedButton != confirm && highlightedButton != cancel)
            return;

        highlightedButton = null;
        arrow.SetActive(false);
    }

    public void ResetConfirmationButton(GameObject button)
    {
        if (resizeRoutines.TryGetValue(button, out Coroutine routine) && routine != null)
            StopCoroutine(routine);
        resizeRoutines.Remove(button);
        originalWidths.Remove(button);
    }

    protected virtual void ResizeButton(GameObject button, bool highlighted)
    {
        RectTransform rect = button.transform as RectTransform;
        if (!originalWidths.TryGetValue(button, out float originalWidth))
        {
            originalWidth = rect.rect.width;
            originalWidths.Add(button, originalWidth);
        }
        if (resizeRoutines.TryGetValue(button, out Coroutine previous) && previous != null)
            StopCoroutine(previous);
        float target = originalWidth * (highlighted ? hoverWidthMultiplier : 1f);
        resizeRoutines[button] = StartCoroutine(AnimateWidth(rect, target));
    }

    private IEnumerator AnimateWidth(RectTransform rect, float target)
    {
        float start = rect.rect.width;
        float elapsed = 0f;
        while (elapsed < resizeDuration)
        {
            elapsed += Time.unscaledDeltaTime;
            rect.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal,
                Mathf.Lerp(start, target, elapsed / resizeDuration));
            yield return null;
        }
        rect.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, target);
    }
}
