using TMPro;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.UI;
using System;
using System.Collections.Generic;

public class JournalNewsItem : MonoBehaviour
{
    [SerializeField] private TMP_Text titleText;
    [SerializeField] private TMP_Text descriptionText;
    [SerializeField] private Image monsterImage;
    private LocalizedString titleSource;
    private LocalizedString descriptionSource;
    private RectTransform newsRect;
    private LayoutElement layoutElement;
    private float initialHeight;
    private float descriptionTop;
    private float titleHeight;
    private float descriptionHeight;
    private bool sizingReady;
    private bool updatingSize;
    private readonly List<Decoration> decorations = new List<Decoration>();

    private class Decoration
    {
        public RectTransform Rect;
        public float Top;
        public float Height;
        public bool FillsArticle;
        public bool FollowsTitleHeight;
        public bool BelowTitle;
    }

    public event Action LayoutChanged;

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
        ResolveReferences(preferSplashArt);
        PrepareSizing();
        if (titleText != null)
            titleText.text = string.Empty;
        if (descriptionText != null)
            descriptionText.text = string.Empty;

        ApplyMonsterImage(monsterImage, entry, preferSplashArt);
        BindTitle(entry.Quest.QuestTitle);
        BindDescription(entry.PlayerWon
            ? entry.Quest.ResultDescriptionGood
            : entry.Quest.ResultDescriptionBad);
        UpdateSize();

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
        ResolveReferences(false);
        PrepareSizing();
        if (titleText != null)
        {
            titleText.text = warningTitle;
        }
        if (descriptionText != null)
            descriptionText.text = "Requirements not configured.";

        ApplyMonsterImage(monsterImage, null, false);
        if (emergencyQuest != null)
            BindDescription(emergencyQuest.EmergencyRequirements);
        UpdateSize();
    }

    private void ResolveReferences(bool resolveMonsterImage)
    {
        TMP_Text[] texts = GetComponentsInChildren<TMP_Text>(true);

        foreach (TMP_Text text in texts)
        {
            if (titleText == null && text.name == "Title")
                titleText = text;
            else if (descriptionText == null && text.name == "Description")
                descriptionText = text;
        }

        if (resolveMonsterImage && monsterImage == null)
        {
            foreach (Image image in GetComponentsInChildren<Image>(true))
            {
                if (image.name == "CharacterBg" || image.name == "Image")
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
        if (titleText != null)
        {
            titleText.text = value ?? string.Empty;
            UpdateSize();
        }
    }

    private void UpdateDescription(string value)
    {
        if (descriptionText != null && !string.IsNullOrWhiteSpace(value))
        {
            descriptionText.text = value;
            UpdateSize();
        }
    }

    private void PrepareSizing()
    {
        if (sizingReady || titleText == null || descriptionText == null)
            return;

        newsRect = transform as RectTransform;
        if (newsRect == null || titleText.rectTransform.parent != newsRect ||
            descriptionText.rectTransform.parent != newsRect)
            return;

        initialHeight = newsRect.rect.height;
        titleHeight = titleText.rectTransform.rect.height;
        descriptionHeight = descriptionText.rectTransform.rect.height;
        float titleTop = AnchorToTop(titleText.rectTransform);
        descriptionTop = AnchorToTop(descriptionText.rectTransform);

        RectTransform background = null;
        foreach (RectTransform child in newsRect)
        {
            if (child == titleText.rectTransform || child == descriptionText.rectTransform)
                continue;

            float height = child.rect.height;
            float top = AnchorToTop(child);
            bool fillsArticle = height >= initialHeight * 0.9f;
            decorations.Add(new Decoration
            {
                Rect = child,
                Top = top,
                Height = height,
                FillsArticle = fillsArticle,
                FollowsTitleHeight = !fillsArticle && Mathf.Abs(top - titleTop) < 3f &&
                    Mathf.Abs(height - titleHeight) < 3f,
                BelowTitle = !fillsArticle && top >= titleTop + titleHeight - 3f
            });
            if (fillsArticle && child.GetComponent<Image>() != null)
                background = child;
        }
        if (background != null)
            background.SetAsFirstSibling();

        // The serialized auto-size value can be based on placeholder text, not the real headline.
        if (titleText.enableAutoSizing)
            titleText.fontSize = Mathf.Clamp(20f, titleText.fontSizeMin, titleText.fontSizeMax);
        titleText.enableAutoSizing = false;
        descriptionText.enableAutoSizing = false;
        layoutElement = GetComponent<LayoutElement>();
        if (layoutElement == null)
            layoutElement = gameObject.AddComponent<LayoutElement>();
        sizingReady = true;
    }

    private float AnchorToTop(RectTransform child)
    {
        // Local coordinates remain valid while the journal's opening animation has zero scale.
        Rect rect = child.rect;
        Vector3 local = child.localPosition;
        float width = newsRect.rect.width;
        float left = local.x + rect.xMin - newsRect.rect.xMin;
        float right = local.x + rect.xMax - newsRect.rect.xMin;
        float top = newsRect.rect.yMax - (local.y + rect.yMax);
        float height = rect.height;
        child.anchorMin = new Vector2(width > 0f ? left / width : 0f, 1f);
        child.anchorMax = new Vector2(width > 0f ? right / width : 1f, 1f);
        child.pivot = new Vector2(0.5f, 1f);
        child.sizeDelta = new Vector2(0f, height);
        child.anchoredPosition = new Vector2(0f, -top);
        return top;
    }

    private void UpdateSize()
    {
        if (!sizingReady || updatingSize)
            return;

        updatingSize = true;

        float newTitleHeight = Mathf.Max(titleHeight, titleText.GetPreferredValues(
            titleText.text, titleText.rectTransform.rect.width, Mathf.Infinity).y);
        float newDescriptionHeight = Mathf.Max(descriptionHeight, descriptionText.GetPreferredValues(
            descriptionText.text, descriptionText.rectTransform.rect.width, Mathf.Infinity).y);
        titleText.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, newTitleHeight);
        descriptionText.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, newDescriptionHeight);
        Vector2 position = descriptionText.rectTransform.anchoredPosition;
        position.y = -(descriptionTop + newTitleHeight - titleHeight);
        descriptionText.rectTransform.anchoredPosition = position;

        float height = initialHeight + (newTitleHeight - titleHeight) +
            (newDescriptionHeight - descriptionHeight);
        float titleGrowth = newTitleHeight - titleHeight;
        foreach (Decoration decoration in decorations)
        {
            if (decoration.Rect == null)
                continue;
            float decorationHeight = decoration.FillsArticle ? height :
                decoration.Height + (decoration.FollowsTitleHeight ? titleGrowth : 0f);
            decoration.Rect.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, decorationHeight);
            Vector2 decorationPosition = decoration.Rect.anchoredPosition;
            decorationPosition.y = -(decoration.Top + (decoration.BelowTitle ? titleGrowth : 0f));
            decoration.Rect.anchoredPosition = decorationPosition;
        }

        bool changed = Mathf.Abs(layoutElement.preferredHeight - height) > 0.01f;
        layoutElement.preferredHeight = height;
        newsRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, height);
        updatingSize = false;
        if (changed)
            LayoutChanged?.Invoke();
    }

    private void OnRectTransformDimensionsChange()
    {
        if (sizingReady)
            UpdateSize();
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

            if (data.JournalSprite != null)
                return data.JournalSprite;

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
