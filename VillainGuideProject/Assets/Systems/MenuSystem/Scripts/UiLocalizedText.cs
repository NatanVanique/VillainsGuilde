using TMPro;
using UnityEngine;
using UnityEngine.Localization;

// Attached at runtime to labels belonging to the menu and options canvases.
public class UiLocalizedText : MonoBehaviour
{
    private TMP_Text label;
    private LocalizedString localizedString;

    public void SetKey(string key)
    {
        if (localizedString != null)
            localizedString.StringChanged -= UpdateText;

        label = GetComponent<TMP_Text>();
        localizedString = new LocalizedString("UI", key);
        localizedString.StringChanged += UpdateText;
    }

    private void UpdateText(string value)
    {
        if (label != null && !string.IsNullOrEmpty(value))
            label.text = value;
    }

    private void OnDestroy()
    {
        if (localizedString != null)
            localizedString.StringChanged -= UpdateText;
    }

    public static void Bind(TMP_Text label, string key)
    {
        if (label == null)
            return;
        UiLocalizedText binding = label.GetComponent<UiLocalizedText>();
        if (binding == null)
            binding = label.gameObject.AddComponent<UiLocalizedText>();
        binding.SetKey(key);
    }
}
