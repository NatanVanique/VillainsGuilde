using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;

public static class LanguagePreference
{
    public const string Key = "SelectedLanguage";

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void RestoreLanguage()
    {
        if (!PlayerPrefs.HasKey(Key))
            return;
        LocalizationSettings.InitializationOperation.Completed += operation =>
        {
            if (operation.Status != UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationStatus.Succeeded)
                return;
            Locale locale = LocalizationSettings.AvailableLocales.GetLocale(
                new LocaleIdentifier(PlayerPrefs.GetString(Key)));
            if (locale != null)
                LocalizationSettings.SelectedLocale = locale;
        };
    }
}
