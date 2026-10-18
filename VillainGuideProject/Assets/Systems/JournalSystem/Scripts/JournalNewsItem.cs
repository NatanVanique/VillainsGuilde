using TMPro;
using UnityEngine;
using UnityEngine.Localization;

public class JournalNewsItem : MonoBehaviour
{
    [SerializeField] private TMP_Text titleText;
    [SerializeField] private TMP_Text descriptionText;

    public void Setup(JournalRegister.JournalEntry entry)
    {
        if (entry == null || entry.Quest == null)
            return;

        ResolveTextReferences();
        if (titleText != null)
            titleText.text = GetLocalizedText(entry.Quest.QuestTitle);
        if (descriptionText != null)
            descriptionText.text = GetResultDescription(entry);
    }

    public void SetupWarning(QuestData emergencyQuest, string warningText)
    {
        ResolveTextReferences();
        if (titleText != null)
        {
            titleText.text = emergencyQuest == null
                ? string.Empty
                : GetLocalizedText(emergencyQuest.QuestTitle);
        }
        if (descriptionText != null)
            descriptionText.text = warningText;
    }

    private void ResolveTextReferences()
    {
        TMP_Text[] texts = GetComponentsInChildren<TMP_Text>(true);

        foreach (TMP_Text text in texts)
        {
            if (titleText == null && text.name == "Title")
                titleText = text;
            else if (descriptionText == null && text.name == "Description")
                descriptionText = text;
        }
    }

    public static string GetResultDescription(JournalRegister.JournalEntry entry)
    {
        if (entry == null || entry.Quest == null)
            return string.Empty;

        return GetLocalizedText(entry.PlayerWon
            ? entry.Quest.ResultDescriptionGood
            : entry.Quest.ResultDescriptionBad);
    }

    public static string GetLocalizedText(LocalizedString localizedString)
    {
        return localizedString == null ? string.Empty : localizedString.GetLocalizedString();
    }
}
