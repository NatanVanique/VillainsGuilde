using TMPro;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.UI;

public class JournalNewsItem : MonoBehaviour
{
    [SerializeField] private TMP_Text titleText;
    [SerializeField] private TMP_Text descriptionText;
    [SerializeField] private Image monsterImage;
    private LocalizedString titleSource;
    private LocalizedString descriptionSource;

    public void SetReferences(TMP_Text title, TMP_Text description, Image image)
    {
        titleText = title;
        descriptionText = description;
        monsterImage = image;
    }

    public void Setup(JournalRegister.JournalEntry entry, bool preferSplashArt = false)
    {
        if (entry == null || entry.Quest == null)
            return;

        ClearBindings();
        ResolveReferences();
        if (titleText != null)
            titleText.text = string.Empty;
        if (descriptionText != null)
            descriptionText.text = string.Empty;

        ApplyMonsterImage(monsterImage, entry, preferSplashArt);
        BindTitle(entry.Quest.QuestTitle);
        BindDescription(entry.PlayerWon
            ? entry.Quest.ResultDescriptionGood
            : entry.Quest.ResultDescriptionBad);

        if (entry.Quest.QuestTitle == null || entry.Quest.QuestTitle.IsEmpty)
            Debug.LogWarning($"Quest '{entry.Quest.name}' has no QuestTitle configured.", entry.Quest);
        LocalizedString result = entry.PlayerWon
            ? entry.Quest.ResultDescriptionGood
            : entry.Quest.ResultDescriptionBad;
        if (result == null || result.IsEmpty)
            Debug.LogWarning($"Quest '{entry.Quest.name}' has no {(entry.PlayerWon ? "ResultDescriptionGood" : "ResultDescriptionBad")} configured.", entry.Quest);
    }

    public void SetupWarning(QuestData emergencyQuest, string warningTitle)
    {
        ClearBindings();
        ResolveReferences();
        if (titleText != null)
        {
            titleText.text = warningTitle;
        }
        if (descriptionText != null)
            descriptionText.text = "Requirements not configured.";

        ApplyMonsterImage(monsterImage, null, false);
        if (emergencyQuest != null)
            BindDescription(emergencyQuest.EmergencyRequirements);
    }

    private void ResolveReferences()
    {
        TMP_Text[] texts = GetComponentsInChildren<TMP_Text>(true);

        foreach (TMP_Text text in texts)
        {
            if (titleText == null && text.name == "Title")
                titleText = text;
            else if (descriptionText == null && text.name == "Description")
                descriptionText = text;
        }

        if (monsterImage == null)
        {
            foreach (Image image in GetComponentsInChildren<Image>(true))
            {
                if (image.name == "Image")
                {
                    monsterImage = image;
                    break;
                }
            }
        }
    }

    private void BindTitle(LocalizedString source)
    {
        if (titleText == null || source == null || source.IsEmpty)
            return;
        titleSource = source;
        titleSource.StringChanged += UpdateTitle;
    }

    private void BindDescription(LocalizedString source)
    {
        if (descriptionText == null || source == null || source.IsEmpty)
            return;
        descriptionSource = source;
        descriptionSource.StringChanged += UpdateDescription;
    }

    private void UpdateTitle(string value)
    {
        if (titleText != null && !string.IsNullOrWhiteSpace(value))
            titleText.text = value;
    }

    private void UpdateDescription(string value)
    {
        if (descriptionText != null && !string.IsNullOrWhiteSpace(value))
            descriptionText.text = value;
    }

    private void ClearBindings()
    {
        if (titleSource != null)
            titleSource.StringChanged -= UpdateTitle;
        if (descriptionSource != null)
            descriptionSource.StringChanged -= UpdateDescription;
        titleSource = null;
        descriptionSource = null;
    }

    private void OnDestroy()
    {
        ClearBindings();
    }

    public static void ApplyMonsterImage(
        Image target,
        JournalRegister.JournalEntry entry,
        bool preferSplashArt)
    {
        if (target == null)
            return;

        Sprite sprite = GetMonsterSprite(entry, preferSplashArt);
        target.sprite = sprite;
        target.enabled = sprite != null;

        if (sprite != null)
        {
            target.color = Color.white;
            target.preserveAspect = true;
        }
    }

    public static Sprite GetMonsterSprite(
        JournalRegister.JournalEntry entry,
        bool preferSplashArt)
    {
        if (entry == null || entry.Monsters == null)
            return null;

        foreach (MonsterInfo monster in entry.Monsters)
        {
            MonsterData data = monster.SelectedData;
            if (data == null)
                continue;

            Sprite preferred = preferSplashArt ? data.SplashArt : data.Icon;
            if (preferred != null)
                return preferred;

            Sprite fallback = preferSplashArt ? data.Icon : data.SplashArt;
            if (fallback != null)
                return fallback;
        }

        return null;
    }
}
