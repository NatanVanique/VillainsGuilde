using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class OptionsManager : MonoBehaviour
{
    [SerializeField] private RectTransform optionsCanvas;

    private Button closeButton;
    private Button optionsButton;
    private CursorLockMode previousCursorLock;
    private bool previousCursorVisible;

    private void Awake()
    {
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
        optionsCanvas.gameObject.SetActive(true);
    }

    public void CloseOptions()
    {
        optionsCanvas.gameObject.SetActive(false);
        Cursor.lockState = previousCursorLock;
        Cursor.visible = previousCursorVisible;
    }

    private void OnDestroy()
    {
        if (closeButton != null)
            closeButton.onClick.RemoveListener(CloseOptions);
        if (optionsButton != null)
            optionsButton.onClick.RemoveListener(OpenOptions);
    }
}
