using TMPro;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class QuitConfirmation : MonoBehaviour
{
    private UiButtonManager menu;
    private Button originalButton;
    private Image originalImage;
    private TMP_Text originalText;
    private EventTrigger originalTrigger;
    private GameObject confirmButton;
    private GameObject cancelButton;
    private bool confirming;
    private Canvas canvas;
    private string confirmLabel;
    private UnityEngine.Events.UnityAction confirmAction;
    private readonly List<RaycastResult> raycastResults = new List<RaycastResult>();

    public void Initialize(UiButtonManager uiManager, GameObject buttonObject, string label, UnityEngine.Events.UnityAction action)
    {
        menu = uiManager;
        confirmLabel = label;
        confirmAction = action;
        originalButton = buttonObject.GetComponent<Button>();
        originalImage = buttonObject.GetComponent<Image>();
        originalText = buttonObject.GetComponentInChildren<TMP_Text>();
        originalTrigger = buttonObject.GetComponent<EventTrigger>();
        canvas = buttonObject.GetComponentInParent<Canvas>();
    }

    public void ShowConfirmation()
    {
        if (confirmButton == null)
            CreateButtons();

        ResetConfirmationLayout(confirmButton, false);
        ResetConfirmationLayout(cancelButton, true);

        confirming = true;
        menu.DisableArrow(originalButton.gameObject);
        originalTrigger.enabled = false;
        originalButton.enabled = false;
        originalImage.enabled = false;
        originalText.gameObject.SetActive(false);
        confirmButton.GetComponent<Image>().color = Color.black;
        cancelButton.GetComponent<Image>().color = Color.red;
        confirmButton.GetComponentInChildren<TMP_Text>(true).color = Color.white;
        cancelButton.GetComponentInChildren<TMP_Text>(true).color = Color.white;
        confirmButton.SetActive(true);
        cancelButton.SetActive(true);
    }

    private void CreateButtons()
    {
        GameObject original = originalButton.gameObject;
        confirmButton = Instantiate(original, original.transform.parent);
        cancelButton = Instantiate(original, original.transform.parent);
        SetupButton(confirmButton, false);
        SetupButton(cancelButton, true);
    }

    private void SetupButton(GameObject buttonObject, bool cancel)
    {
        buttonObject.name = cancel ? "CancelConfirmationButton" : "QuitConfirmationButton";
        buttonObject.SetActive(false);
        RectTransform rect = (RectTransform)buttonObject.transform;
        rect.SetParent(originalButton.transform, false);
        rect.localScale = Vector3.one;
        ResetConfirmationLayout(buttonObject, cancel);

        Image image = buttonObject.GetComponent<Image>();
        image.color = cancel ? Color.red : Color.black;
        TMP_Text label = buttonObject.GetComponentInChildren<TMP_Text>();
        label.text = cancel ? "Cancel" : confirmLabel;
        UiLocalizedText.Bind(label, cancel ? "Confirmation_Cancel" :
            confirmLabel == "Menu" ? "Confirmation_Menu" : "Menu_Quit");
        label.color = Color.white;
        label.enableAutoSizing = true;

        Button button = buttonObject.GetComponent<Button>();
        button.transition = Selectable.Transition.None;
        for (int i = 0; i < button.onClick.GetPersistentEventCount(); i++)
            button.onClick.SetPersistentListenerState(i, UnityEngine.Events.UnityEventCallState.Off);
        button.onClick.RemoveAllListeners();
        if (cancel)
            button.onClick.AddListener(CancelQuit);
        else
            button.onClick.AddListener(ConfirmQuit);

        EventTrigger trigger = buttonObject.GetComponent<EventTrigger>();
        trigger.triggers.Clear();
        AddHoverEvent(trigger, EventTriggerType.PointerEnter, () => menu.MoveArrow(buttonObject));
        AddHoverEvent(trigger, EventTriggerType.PointerExit, () => menu.DisableArrow(buttonObject));
    }

    private static void AddHoverEvent(EventTrigger trigger, EventTriggerType type, UnityEngine.Events.UnityAction action)
    {
        EventTrigger.Entry entry = new EventTrigger.Entry { eventID = type };
        entry.callback.AddListener(_ => action());
        trigger.triggers.Add(entry);
    }

    public void ConfirmQuit()
    {
        confirmAction();
    }

    public void CancelQuit()
    {
        CloseConfirmation(true);
    }

    public void CloseForOtherButton(GameObject button)
    {
        if (confirming && button != originalButton.gameObject &&
            button != confirmButton && button != cancelButton)
            CloseConfirmation(false);
    }

    private void CloseConfirmation(bool restoreHover)
    {
        confirming = false;
        confirmButton.SetActive(false);
        cancelButton.SetActive(false);
        originalImage.enabled = true;
        originalImage.color = Color.black;
        originalText.gameObject.SetActive(true);
        originalText.color = Color.white;
        originalButton.enabled = true;
        originalTrigger.enabled = true;
        menu.ClearHighlight(originalButton.gameObject, confirmButton, cancelButton);

        if (restoreHover && isActiveAndEnabled && Mouse.current != null && ContainsPointer((RectTransform)originalButton.transform))
            menu.MoveArrow(originalButton.gameObject);
    }

    private void LateUpdate()
    {
        if (!confirming || Mouse.current == null)
            return;

        if (EventSystem.current != null)
        {
            PointerEventData pointer = new PointerEventData(EventSystem.current);
            pointer.position = Mouse.current.position.ReadValue();
            raycastResults.Clear();
            EventSystem.current.RaycastAll(pointer, raycastResults);
            if (raycastResults.Count > 0)
            {
                Button hoveredButton = raycastResults[0].gameObject.GetComponentInParent<Button>();
                if (hoveredButton != null && hoveredButton != originalButton &&
                    hoveredButton.gameObject != confirmButton && hoveredButton.gameObject != cancelButton)
                {
                    CloseConfirmation(false);
                    return;
                }
            }
        }

        if (!ContainsPointer((RectTransform)originalButton.transform) &&
            !ContainsPointer((RectTransform)confirmButton.transform) &&
            !ContainsPointer((RectTransform)cancelButton.transform))
            CloseConfirmation(false);
    }

    private void ResetConfirmationLayout(GameObject button, bool cancel)
    {
        menu.ResetConfirmationButton(button);
        RectTransform rect = (RectTransform)button.transform;
        rect.anchorMin = new Vector2(cancel ? 0.5f : 0f, 0f);
        rect.anchorMax = new Vector2(cancel ? 1f : 0.5f, 1f);
        rect.offsetMin = new Vector2(cancel ? 3f : 0f, 0f);
        rect.offsetMax = new Vector2(cancel ? 0f : -3f, 0f);
    }

    private bool ContainsPointer(RectTransform rect)
    {
        Camera eventCamera = canvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : canvas.worldCamera;
        return RectTransformUtility.RectangleContainsScreenPoint(rect, Mouse.current.position.ReadValue(), eventCamera);
    }

    private void OnDisable()
    {
        if (confirming)
            CloseConfirmation(false);
    }
}
