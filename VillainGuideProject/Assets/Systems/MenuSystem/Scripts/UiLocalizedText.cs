using TMPro;
using UnityEngine;
using UnityEngine.Localization;

// Can be configured on a prefab or attached at runtime to a TMP label.
public class UiLocalizedText : MonoBehaviour
{
    [SerializeField] private string key;
    private TMP_Text label;
    private LocalizedString localizedString;
    private string argument;

    private void Awake()
    {
        if (localizedString == null && !string.IsNullOrEmpty(key))
            SetKey(key);
    }

    public void SetKey(string newKey, string value = null)
    {
        if (localizedString != null)
            localizedString.StringChanged -= UpdateText;

        key = newKey;
        argument = value;
        label = GetComponent<TMP_Text>();
        localizedString = new LocalizedString("UI", key);
        localizedString.StringChanged += UpdateText;
    }

    private void UpdateText(string value)
    {
        if (label != null && !string.IsNullOrEmpty(value))
            label.text = argument == null ? value : value.Replace("{amount}", argument);
    }

    private void OnDestroy()
    {
        if (localizedString != null)
            localizedString.StringChanged -= UpdateText;
    }

    public static void Bind(TMP_Text label, string key, string value = null)
    {
        if (label == null)
            return;
        UiLocalizedText binding = label.GetComponent<UiLocalizedText>();
        if (binding == null)
            binding = label.gameObject.AddComponent<UiLocalizedText>();
        binding.SetKey(key, value);
    }
}
