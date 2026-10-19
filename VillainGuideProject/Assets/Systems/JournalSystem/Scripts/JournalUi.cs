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
    [SerializeField] private VerticalLayoutGroup newsGrid;
    [SerializeField] private ScrollRect newsScroll;
    [SerializeField] private Slider newsSlider;
    [SerializeField] private GameObject headerPrefab;
    [SerializeField] private GameObject bigNewsPrefab;
    [SerializeField] private GameObject smallNewsPrefab1;
    [SerializeField] private GameObject smallNewsPrefab2;
    [SerializeField] private GameObject emergencyWarningPrefab;
    [SerializeField] private GameObject resultsNewsPrefab;
    [SerializeField, Min(0f)] private float rowSpacing = 6f;
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
    private Coroutine layoutRefresh;
    private bool resetScrollOnRefresh;
    private readonly List<GameObject> spawnedRoots = new List<GameObject>();
    private readonly List<RectTransform> newsRows = new List<RectTransform>();
    private RectTransform columnDivider;

    public event Action DoneRequested;

    public void Setup()
    {
        ResolveSystems();
        ConfigureButtons();
        ShowNewsPage();
        BuildNewsPage();
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

        if (newsSlider != null)
        {
            newsSlider.onValueChanged.RemoveListener(SetScrollPosition);
            newsSlider.onValueChanged.AddListener(SetScrollPosition);
        }
        if (newsScroll != null)
        {
            newsScroll.onValueChanged.RemoveListener(SyncSliderPosition);
            newsScroll.onValueChanged.AddListener(SyncSliderPosition);
        }
    }

    public void BuildNewsPage()
    {
        ClearSpawnedNews();

        if (rewardsRegister == null)
            ResolveSystems();

        if (rewardsRegister == null)
            return;

        if (newsGrid == null)
            return;
        newsGrid.gameObject.SetActive(true);
        if (newsScroll != null && newsScroll.viewport != null)
        {
            GameObject viewport = newsScroll.viewport.gameObject;
            if (viewport.GetComponent<RectMask2D>() == null)
                viewport.AddComponent<RectMask2D>();

            // Keep the entire viewport scrollable even between article graphics.
            Graphic viewportGraphic = viewport.GetComponent<Graphic>();
            if (viewportGraphic == null)
            {
                Image hitArea = viewport.AddComponent<Image>();
                hitArea.color = Color.clear;
                hitArea.canvasRenderer.cullTransparentMesh = false;
                viewportGraphic = hitArea;
            }
            viewportGraphic.raycastTarget = true;
        }

        SpawnHeader();
        JournalRegister.JournalEntry featured = rewardsRegister.BigNews;
        if (featured != null && !HasNewsContent(featured))
            featured = null;
        JournalRegister.JournalEntry promoted = null;
        if (featured == null && bigNewsPrefab != null && rewardsRegister.SmallNews != null)
        {
            foreach (JournalRegister.JournalEntry entry in rewardsRegister.SmallNews)
            {
                if (HasNewsContent(entry) &&
                    (promoted == null || entry.Importance > promoted.Importance))
                    promoted = entry;
            }
            featured = promoted;
        }
        if (featured != null)
            SpawnFullWidthNews(bigNewsPrefab, featured, true);

        Transform incompleteRow = SpawnMissionRows(rewardsRegister.SmallNews, promoted);

        QuestData upcomingEmergency = GetUpcomingEmergency();
        if (upcomingEmergency != null && emergencyWarningPrefab != null)
        {
            Transform warningRow = incompleteRow != null ? incompleteRow : CreateNewsRow("EventRow");
            GameObject warningObject = Instantiate(emergencyWarningPrefab, warningRow);
            PrepareHalfWidth(warningObject);
            JournalNewsItem warning = GetOrAddNewsItem(warningObject);
            warning.LayoutChanged += RequestLayoutRefresh;
            warning.SetupWarning(upcomingEmergency, emergencyWarningText);
            spawnedNews.Add(warning);
        }

        SpawnResultsNews();
        CreateColumnDivider();
        resetScrollOnRefresh = true;
        RefreshLayout();
        if (newsScroll != null)
            newsScroll.verticalNormalizedPosition = 1f;
        if (newsSlider != null)
            newsSlider.SetValueWithoutNotify(0f);
        RequestLayoutRefresh();
    }

    private void SpawnHeader()
    {
        if (headerPrefab == null)
            return;
        GameObject container = new GameObject("HeaderContainer", typeof(RectTransform), typeof(LayoutElement));
        container.transform.SetParent(newsGrid.transform, false);
        GameObject instance = Instantiate(headerPrefab, container.transform);
        RectTransform header = instance.transform as RectTransform;
        if (header != null)
        {
            float height = header.rect.height;
            float bottom = header.rect.yMin;
            foreach (RectTransform child in header.GetComponentsInChildren<RectTransform>(false))
            {
                if (child == header)
                    continue;
                Vector3 lowerLeft = child.TransformPoint(new Vector3(child.rect.xMin, child.rect.yMin, 0f));
                Vector3 lowerRight = child.TransformPoint(new Vector3(child.rect.xMax, child.rect.yMin, 0f));
                bottom = Mathf.Min(bottom, header.InverseTransformPoint(lowerLeft).y,
                    header.InverseTransformPoint(lowerRight).y);
            }

            LayoutElement layout = GetOrAddLayoutElement(container);
            layout.preferredHeight = height + Mathf.Max(0f, header.rect.yMin - bottom);
            layout.flexibleWidth = 1f;
            header.anchorMin = new Vector2(0f, 1f);
            header.anchorMax = new Vector2(1f, 1f);
            header.pivot = new Vector2(0.5f, 1f);
            header.sizeDelta = new Vector2(0f, height);
            header.anchoredPosition = Vector2.zero;
        }
        spawnedRoots.Add(container);
    }

    private void SpawnResultsNews()
    {
        if (resultsNewsPrefab == null || journalReward == null)
            return;

        GameObject instance = Instantiate(resultsNewsPrefab, newsGrid.transform);
        PrepareFullWidth(instance);
        VerticalLayoutGroup resultsLayout = instance.transform.Find("Results")?.GetComponent<VerticalLayoutGroup>();
        if (resultsLayout != null)
            resultsLayout.spacing = -14f;
        JournalReward.RewardResult result = journalReward.CalculateRewards();
        bool hasResults = result.HasXP || result.HasMoney || result.HasCharacters;
        Transform results = instance.transform.Find("Results");
        Transform noRewards = instance.transform.Find("NoRewards");
        if (results != null)
            results.gameObject.SetActive(hasResults);
        if (noRewards != null)
            noRewards.gameObject.SetActive(!hasResults);
        SetResultsTab(instance.transform, "Results/XpTab", result.HasXP, result.XP);
        SetResultsTab(instance.transform, "Results/MoneyTab", result.HasMoney, result.Money);
        SetResultsTab(instance.transform, "Results/MonsterTab", result.HasCharacters, result.Characters);
        spawnedRoots.Add(instance);
    }

    private static void SetResultsTab(Transform root, string path, bool visible, float value)
    {
        Transform tab = root.Find(path);
        if (tab == null)
        {
            Debug.LogWarning($"Results news is missing '{path}'.", root);
            return;
        }

        tab.gameObject.SetActive(visible);
        TMP_Text label = tab.GetComponentInChildren<TMP_Text>(true);
        if (label != null)
        {
            string amount = value.ToString("+0.########;-0.########;0");
            label.text = label.text.Replace("xx", amount);
        }
    }

    private void SpawnFullWidthNews(GameObject prefab, JournalRegister.JournalEntry entry, bool showMonster)
    {
        if (prefab == null || entry == null)
            return;
        GameObject instance = Instantiate(prefab, newsGrid.transform);
        PrepareFullWidth(instance);
        JournalNewsItem item = GetOrAddNewsItem(instance);
        item.LayoutChanged += RequestLayoutRefresh;
        item.Setup(entry, showMonster);
        spawnedNews.Add(item);
        spawnedRoots.Add(instance);
    }

    public void RefreshLayout()
    {
        if (newsGrid == null)
            return;

        UpdateRowHeights();

        ContentSizeFitter fitter = newsGrid.GetComponent<ContentSizeFitter>();
        if (fitter == null)
            fitter = newsGrid.gameObject.AddComponent<ContentSizeFitter>();
        fitter.horizontalFit = ContentSizeFitter.FitMode.Unconstrained;
        fitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;

        LayoutRebuilder.ForceRebuildLayoutImmediate(newsGrid.transform as RectTransform);
        newsGrid.SetLayoutVertical();
        UpdateColumnDivider();
    }

    private void RequestLayoutRefresh()
    {
        if (!isActiveAndEnabled || newsGrid == null || layoutRefresh != null)
            return;
        layoutRefresh = StartCoroutine(RefreshScrollNextFrame());
    }

    private Transform SpawnMissionRows(List<JournalRegister.JournalEntry> entries,
        JournalRegister.JournalEntry promoted)
    {
        if (entries == null)
            return null;
        Transform row = null;
        int column = 0;
        foreach (JournalRegister.JournalEntry entry in entries)
        {
            if (entry == promoted || !HasNewsContent(entry))
                continue;
            if (column == 0)
                row = CreateNewsRow("NewsRow");
            SpawnSmallNews(row, entry);
            column = (column + 1) % 2;
        }
        return column == 1 ? row : null;
    }

    private static bool HasNewsContent(JournalRegister.JournalEntry entry)
    {
        if (entry == null || entry.Quest == null)
            return false;

        var title = entry.Quest.QuestTitle;
        var description = entry.PlayerWon
            ? entry.Quest.ResultDescriptionGood
            : entry.Quest.ResultDescriptionBad;
        if ((title != null && !title.IsEmpty) || (description != null && !description.IsEmpty))
            return true;

        Debug.LogWarning($"Quest '{entry.Quest.name}' has no title or result description configured; " +
            "its empty newspaper article was skipped.", entry.Quest);
        return false;
    }

    private Transform CreateNewsRow(string rowName)
    {
        GameObject row = new GameObject(rowName, typeof(RectTransform),
            typeof(HorizontalLayoutGroup), typeof(LayoutElement));
        row.transform.SetParent(newsGrid.transform, false);
        ConfigureNewsRow(row.GetComponent<HorizontalLayoutGroup>());
        newsRows.Add(row.GetComponent<RectTransform>());
        spawnedRoots.Add(row);
        return row.transform;
    }

    private GameObject CreateDivider(string objectName, Transform parent)
    {
        GameObject divider = new GameObject(objectName, typeof(RectTransform), typeof(Image));
        divider.transform.SetParent(parent, false);
        Image image = divider.GetComponent<Image>();
        Image headerImage = headerPrefab != null ? headerPrefab.GetComponent<Image>() : null;
        if (headerImage != null)
        {
            image.sprite = headerImage.sprite;
            image.color = headerImage.color;
        }
        image.raycastTarget = false;
        return divider;
    }

    private void CreateColumnDivider()
    {
        if (newsRows.Count == 0)
            return;

        GameObject divider = CreateDivider("ColumnDivider", newsGrid.transform);
        GetOrAddLayoutElement(divider).ignoreLayout = true;
        columnDivider = divider.GetComponent<RectTransform>();
        columnDivider.anchorMin = new Vector2(0.5f, 1f);
        columnDivider.anchorMax = new Vector2(0.5f, 1f);
        columnDivider.pivot = new Vector2(0.5f, 1f);
        spawnedRoots.Add(divider);
    }

    private void UpdateColumnDivider()
    {
        if (columnDivider == null || newsRows.Count == 0)
            return;

        RectTransform content = (RectTransform)newsGrid.transform;
        RectTransform first = newsRows[0];
        RectTransform last = newsRows[newsRows.Count - 1];
        float top = first.localPosition.y + first.rect.yMax;
        float bottom = last.localPosition.y + last.rect.yMin;
        columnDivider.anchoredPosition = new Vector2(
            (newsGrid.padding.left - newsGrid.padding.right) * 0.5f, top - content.rect.yMax);
        columnDivider.sizeDelta = new Vector2(2f, Mathf.Max(0f, top - bottom));
    }

    private void SpawnSmallNews(Transform parent, JournalRegister.JournalEntry entry)
    {
        GameObject prefab = UnityEngine.Random.value < 0.5f ? smallNewsPrefab1 : smallNewsPrefab2;
        if (prefab == null)
            prefab = smallNewsPrefab1 != null ? smallNewsPrefab1 : smallNewsPrefab2;
        if (prefab == null || entry == null)
            return;
        GameObject instanceObject = Instantiate(prefab, parent);
        PrepareHalfWidth(instanceObject);
        JournalNewsItem instance = GetOrAddNewsItem(instanceObject);
        instance.LayoutChanged += RequestLayoutRefresh;
        instance.Setup(entry, false);
        spawnedNews.Add(instance);
    }

    private static void PrepareFullWidth(GameObject instance)
    {
        LayoutElement element = GetOrAddLayoutElement(instance);
        RectTransform rect = instance.transform as RectTransform;
        if (rect != null && element.preferredHeight < 0f)
            element.preferredHeight = rect.rect.height;
        element.flexibleWidth = 1f;
    }

    private void PrepareHalfWidth(GameObject instance)
    {
        LayoutElement element = GetOrAddLayoutElement(instance);
        element.flexibleWidth = 0f;
        element.preferredWidth = GetColumnWidth();
    }

    private float GetColumnWidth()
    {
        RectTransform content = newsGrid.transform as RectTransform;
        if (content == null)
            return 0f;

        float availableWidth = content.rect.width - newsGrid.padding.horizontal;
        return Mathf.Max(0f, (availableWidth - rowSpacing) * 0.5f);
    }

    private void ConfigureNewsRow(HorizontalLayoutGroup layout)
    {
        layout.spacing = rowSpacing;
        layout.childAlignment = TextAnchor.UpperLeft;
        layout.childControlWidth = true;
        layout.childControlHeight = false;
        layout.childForceExpandWidth = false;
        layout.childForceExpandHeight = false;
    }

    private static LayoutElement GetOrAddLayoutElement(GameObject target)
    {
        LayoutElement element = target.GetComponent<LayoutElement>();
        return element != null ? element : target.AddComponent<LayoutElement>();
    }

    private static JournalNewsItem GetOrAddNewsItem(GameObject newsObject)
    {
        JournalNewsItem item = newsObject.GetComponent<JournalNewsItem>();
        return item != null ? item : newsObject.AddComponent<JournalNewsItem>();
    }

    private void UpdateRowHeights()
    {
        float halfWidth = GetColumnWidth();
        foreach (Transform child in newsGrid.transform)
        {
            if (!child.gameObject.activeSelf)
                continue;
            HorizontalLayoutGroup row = child.GetComponent<HorizontalLayoutGroup>();
            if (row == null)
                continue;
            float height = 0f;
            foreach (Transform article in row.transform)
            {
                LayoutElement articleLayout = article.GetComponent<LayoutElement>();
                if (articleLayout != null && articleLayout.ignoreLayout)
                    continue;
                RectTransform rect = article as RectTransform;
                if (articleLayout != null)
                    articleLayout.preferredWidth = halfWidth;
                height = Mathf.Max(height, articleLayout != null && articleLayout.preferredHeight >= 0f
                    ? articleLayout.preferredHeight : rect.rect.height);
            }
            row.GetComponent<LayoutElement>().preferredHeight = height;
        }
    }

    private QuestData GetUpcomingEmergency()
    {
        if (SingleEntryPoint.Instance == null ||
            SingleEntryPoint.Instance.ManagerManager == null ||
            SingleEntryPoint.Instance.ManagerManager.QuestSelector == null)
            return null;

        QuestSelector selector = SingleEntryPoint.Instance.ManagerManager.QuestSelector;
        DaySystem daySystem = SingleEntryPoint.Instance.ManagerManager.DaySystem;
        if (selector == null || daySystem == null)
            return null;

        return selector.GetEmergencyForDay(daySystem.CurrentDay + 1);
    }

    private IEnumerator RefreshScrollNextFrame()
    {
        yield return null;
        RefreshLayout();
        Canvas.ForceUpdateCanvases();
        // Width changes can reflow text during the first layout pass.
        RefreshLayout();

        RectTransform content = newsGrid != null ? newsGrid.transform as RectTransform : null;
        if (content != null)
        {
            LayoutRebuilder.ForceRebuildLayoutImmediate(content);
            UpdateColumnDivider();
        }

        // LocalizedString callbacks and dynamically-sized articles can finish
        // after the first rebuild. Wait one more frame before deciding whether
        // the page actually has content below the viewport.
        yield return null;
        Canvas.ForceUpdateCanvases();
        RefreshLayout();
        if (content != null)
        {
            LayoutRebuilder.ForceRebuildLayoutImmediate(content);
            UpdateColumnDivider();
        }

        if (resetScrollOnRefresh && newsScroll != null)
        {
            newsScroll.StopMovement();
            newsScroll.verticalNormalizedPosition = 1f;
            resetScrollOnRefresh = false;
        }

        if (newsScroll != null)
        {
            float contentHeight = content != null
                ? Mathf.Max(content.rect.height, LayoutUtility.GetPreferredHeight(content))
                : 0f;
            float viewportHeight = newsScroll.viewport != null
                ? newsScroll.viewport.rect.height
                : 0f;
            bool needsScroll = contentHeight > viewportHeight + 1f;
            newsScroll.vertical = needsScroll;
            if (newsSlider != null)
            {
                newsSlider.interactable = needsScroll;
                newsSlider.gameObject.SetActive(needsScroll);
                newsSlider.SetValueWithoutNotify(1f - newsScroll.verticalNormalizedPosition);
            }
        }
        layoutRefresh = null;
    }

    private void SetScrollPosition(float value)
    {
        if (newsScroll != null)
            newsScroll.verticalNormalizedPosition = 1f - value;
    }

    private void SyncSliderPosition(Vector2 position)
    {
        if (newsSlider != null)
            newsSlider.SetValueWithoutNotify(1f - position.y);
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
            {
                item.LayoutChanged -= RequestLayoutRefresh;
                item.gameObject.SetActive(false);
                Destroy(item.gameObject);
            }
        }

        spawnedNews.Clear();
        spawnedRoots.Clear();
        newsRows.Clear();
        columnDivider = null;

        if (newsGrid != null)
        {
            foreach (Transform child in newsGrid.transform)
            {
                child.gameObject.SetActive(false);
                Destroy(child.gameObject);
            }
        }
    }

    private void OnDestroy()
    {
        if (goToResultsButton != null)
            goToResultsButton.onClick.RemoveListener(ShowResultsPage);
        if (doneButton != null)
            doneButton.onClick.RemoveListener(FinishJournal);
        if (newsSlider != null)
            newsSlider.onValueChanged.RemoveListener(SetScrollPosition);
        if (newsScroll != null)
            newsScroll.onValueChanged.RemoveListener(SyncSliderPosition);
    }

    private void OnDisable()
    {
        layoutRefresh = null;
    }
}
