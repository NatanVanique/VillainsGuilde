using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class JournalUi : MonoBehaviour
{
    [Header("Pages")]
    [SerializeField] private GameObject newsPage;
    [SerializeField] private GameObject resultsPage;

    [Header("News")]
    private JournalRegister rewardsRegister;
    [SerializeField] private GridLayoutGroup newsGrid;
    [SerializeField] private ScrollRect newsScroll;
    [SerializeField] private Scrollbar newsScrollbar;
    [SerializeField] private RectTransform bigNewObject;
    [SerializeField] private TMP_Text bigNewTitle;
    [SerializeField] private TMP_Text bigNewDescription;
    [SerializeField] private GameObject smallNewsPrefab;
    [SerializeField] private GameObject emergencyWarningPrefab;
    [SerializeField] private float bigNewSpacing = 16f;
    [SerializeField, TextArea] private string emergencyWarningText = "An emergency mission is expected tomorrow.";

    [Header("Navigation")]
    [SerializeField] private Button goToResultsButton;
    [SerializeField] private Button doneButton;

    [Header("Results")]
    [SerializeField] private TMP_Text xpResultText;
    [SerializeField] private TMP_Text moneyResultText;
    [SerializeField] private TMP_Text charactersResultText;
    [SerializeField] private float countDuration = 0.8f;

    private readonly List<JournalNewsItem> spawnedNews = new List<JournalNewsItem>();
    private JournalReward journalReward;
    private Coroutine resultAnimation;

    public event Action DoneRequested;

    public void Setup()
    {
        ResolveSystems();
        ConfigureButtons();
        BuildNewsPage();
        ShowNewsPage();
    }

    private void ResolveSystems()
    {
        if (SingleEntryPoint.Instance == null || SingleEntryPoint.Instance.ManagerManager == null)
            return;

        rewardsRegister = SingleEntryPoint.Instance.ManagerManager.JournalRegister;
        if (rewardsRegister != null)
            journalReward = rewardsRegister.GetComponent<JournalReward>();
    }

    private void ConfigureButtons()
    {
        if (goToResultsButton != null)
        {
            goToResultsButton.onClick.RemoveListener(ShowResultsPage);
            goToResultsButton.onClick.AddListener(ShowResultsPage);
        }

        if (doneButton != null)
        {
            doneButton.onClick.RemoveListener(FinishJournal);
            doneButton.onClick.AddListener(FinishJournal);
        }
    }

    public void BuildNewsPage()
    {
        ClearSpawnedNews();

        if (rewardsRegister == null)
            ResolveSystems();

        if (rewardsRegister == null)
            return;

        SetupBigNews(rewardsRegister.BigNews);

        if (newsGrid == null)
            return;

        foreach (JournalRegister.JournalEntry entry in rewardsRegister.SmallNews)
            SpawnNews(smallNewsPrefab, entry);

        QuestData upcomingEmergency = GetUpcomingEmergency();
        if (upcomingEmergency != null && emergencyWarningPrefab != null)
        {
            GameObject warningObject = Instantiate(emergencyWarningPrefab, newsGrid.transform);
            JournalNewsItem warning = GetOrAddNewsItem(warningObject);
            warning.SetupWarning(upcomingEmergency, emergencyWarningText);
            spawnedNews.Add(warning);
        }

        RefreshLayout();
        StartCoroutine(RefreshScrollNextFrame());
    }

    private void SetupBigNews(JournalRegister.JournalEntry entry)
    {
        bool hasBigNews = entry != null && entry.Quest != null;
        if (bigNewObject != null)
            bigNewObject.gameObject.SetActive(hasBigNews);

        if (!hasBigNews)
            return;

        ResolveBigNewsTextReferences();

        if (bigNewTitle != null)
            bigNewTitle.text = JournalNewsItem.GetLocalizedText(entry.Quest.QuestTitle);
        if (bigNewDescription != null)
            bigNewDescription.text = JournalNewsItem.GetResultDescription(entry);
    }

    public void RefreshLayout()
    {
        if (newsGrid == null)
            return;

        bool hasBigNews = bigNewObject != null && bigNewObject.gameObject.activeSelf;

        RectOffset padding = newsGrid.padding;
        padding.top = 0;

        if (hasBigNews && bigNewObject != null)
        {
            padding.top = Mathf.CeilToInt(bigNewObject.rect.height + bigNewSpacing);
        }

        newsGrid.padding = padding;

        ContentSizeFitter fitter = newsGrid.GetComponent<ContentSizeFitter>();
        if (fitter == null)
            fitter = newsGrid.gameObject.AddComponent<ContentSizeFitter>();
        fitter.horizontalFit = ContentSizeFitter.FitMode.Unconstrained;
        fitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;

        LayoutRebuilder.ForceRebuildLayoutImmediate(newsGrid.transform as RectTransform);
        newsGrid.SetLayoutVertical();
    }

    private void SpawnNews(GameObject prefab, JournalRegister.JournalEntry entry)
    {
        if (prefab == null || entry == null)
            return;

        GameObject instanceObject = Instantiate(prefab, newsGrid.transform);
        JournalNewsItem instance = GetOrAddNewsItem(instanceObject);
        instance.Setup(entry);
        spawnedNews.Add(instance);
    }

    private static JournalNewsItem GetOrAddNewsItem(GameObject newsObject)
    {
        JournalNewsItem item = newsObject.GetComponent<JournalNewsItem>();
        return item != null ? item : newsObject.AddComponent<JournalNewsItem>();
    }

    private void ResolveBigNewsTextReferences()
    {
        if (bigNewObject == null)
            return;

        foreach (TMP_Text text in bigNewObject.GetComponentsInChildren<TMP_Text>(true))
        {
            if (bigNewTitle == null && text.name == "Title")
                bigNewTitle = text;
            else if (bigNewDescription == null && text.name == "Description")
                bigNewDescription = text;
        }
    }

    private QuestData GetUpcomingEmergency()
    {
        if (SingleEntryPoint.Instance == null ||
            SingleEntryPoint.Instance.ManagerManager == null ||
            SingleEntryPoint.Instance.ManagerManager.QuestSelector == null)
            return null;

        foreach (QuestData quest in SingleEntryPoint.Instance.ManagerManager.QuestSelector.SelectedQuests)
        {
            if (quest != null && quest.Type == QuestType.Emergency)
                return quest;
        }

        return null;
    }

    private IEnumerator RefreshScrollNextFrame()
    {
        yield return null;
        Canvas.ForceUpdateCanvases();

        RectTransform content = newsGrid != null ? newsGrid.transform as RectTransform : null;
        if (content != null)
            LayoutRebuilder.ForceRebuildLayoutImmediate(content);

        if (newsScroll != null)
        {
            bool needsScroll = content != null && newsScroll.viewport != null &&
                content.rect.height > newsScroll.viewport.rect.height;
            newsScroll.vertical = needsScroll;
            newsScroll.verticalNormalizedPosition = 1f;
            if (newsScrollbar != null)
            {
                newsScroll.verticalScrollbar = newsScrollbar;
                newsScroll.verticalScrollbarVisibility = ScrollRect.ScrollbarVisibility.AutoHide;
                newsScrollbar.gameObject.SetActive(needsScroll);
            }
        }
    }

    private void ShowNewsPage()
    {
        if (newsPage != null)
            newsPage.SetActive(true);
        if (resultsPage != null)
            resultsPage.SetActive(false);
    }

    public void ShowResultsPage()
    {
        if (resultsPage == null)
            return;

        if (newsPage != null)
            newsPage.SetActive(false);
        resultsPage.SetActive(true);

        if (journalReward == null)
            return;

        if (resultAnimation != null)
            StopCoroutine(resultAnimation);
        resultAnimation = StartCoroutine(AnimateResults(journalReward.CalculateRewards()));
    }

    private IEnumerator AnimateResults(JournalReward.RewardResult result)
    {
        SetResultText(xpResultText, 0f);
        SetResultText(moneyResultText, 0f);
        SetResultText(charactersResultText, 0);

        yield return AnimateNumber(xpResultText, result.XP);
        yield return AnimateNumber(moneyResultText, result.Money);
        yield return AnimateNumber(charactersResultText, result.Characters);
        resultAnimation = null;
    }

    private IEnumerator AnimateNumber(TMP_Text label, float target)
    {
        if (label == null)
            yield break;

        float duration = Mathf.Max(0.01f, countDuration);
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.unscaledDeltaTime;
            SetResultText(label, Mathf.Lerp(0f, target, elapsed / duration));
            yield return null;
        }

        SetResultText(label, target);
    }

    private static void SetResultText(TMP_Text label, float value)
    {
        if (label != null)
            label.text = value >= 0f ? "+" + Mathf.RoundToInt(value) : Mathf.RoundToInt(value).ToString();
    }

    public void FinishJournal()
    {
        if (journalReward != null)
            journalReward.CollectRewards();

        if (rewardsRegister != null)
            rewardsRegister.ClearResults();

        DoneRequested?.Invoke();
    }

    private void ClearSpawnedNews()
    {
        foreach (JournalNewsItem item in spawnedNews)
        {
            if (item != null)
                Destroy(item.gameObject);
        }

        spawnedNews.Clear();
    }

    private void OnDestroy()
    {
        if (goToResultsButton != null)
            goToResultsButton.onClick.RemoveListener(ShowResultsPage);
        if (doneButton != null)
            doneButton.onClick.RemoveListener(FinishJournal);
    }
}
