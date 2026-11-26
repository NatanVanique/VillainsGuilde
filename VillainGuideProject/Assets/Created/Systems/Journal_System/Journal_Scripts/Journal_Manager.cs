
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public sealed class JournalManager : MonoBehaviour
{
    [SerializeField] private RectTransform journalPanel;
    [SerializeField, Min(0f)] private float entranceDuration = 0.5f;

    private readonly List<GameObject> instantiatedNews = new();
    private readonly List<int> smallNewsLayouts = new();
    private readonly List<GameObject> instantiatedCharacterXp = new();
    private readonly List<(MonsterReferencer.Monsters monster, int startLevel, int startXp, int endLevel, int endXp)> monsterRewards = new();

    private readonly List<(CreateQuest_Data data, MonsterReferencer.Monsters monster, bool isWinned, bool attempted)> pendingNews = new();

    private DaycicleManager daycicle;
    private VisualReferencer visual;

    private int publishedDay = -1;
    private float? timeScaleBeforeJournal;
    private Tween entranceTween;
    private bool rewardsOpen;
    private long totalPopularityXp;
    private long totalCoins;
    private int totalRecruits;

    private void Start()
    {
        Main_Referencer references = Main_Referencer.Instance;
        visual = references.visual;
        daycicle = references.manager.dayCicleManager;
        daycicle.BeforeOfficeDayStart += PublishResults;
    }

    private void OnEnable()
    {
        if (daycicle != null)
        {
            daycicle.BeforeOfficeDayStart += PublishResults;
        }
    }

    private void OnDisable()
    {
        if (daycicle != null)
        {
            daycicle.BeforeOfficeDayStart -= PublishResults;
        }

        if (timeScaleBeforeJournal.HasValue)
        {
            HideEdition();
        }
    }

    public void RegisterNew(CreateQuest_Data data, MonsterReferencer.Monsters monster, bool isWinned, bool attempted = true)
    {
        pendingNews.Add((data, monster, isWinned, attempted));
    }

    private void PublishResults()
    {
        ValuesReferencer values = Main_Referencer.Instance.value;
        int day = values.currentDay_HourValues;
        if (day <= 1 || day == publishedDay)
        {
            return;
        }

        while (values.realizedDayRegister.Count <= day)
        {
            values.realizedDayRegister.Add(new());
        }

        var reports = values.realizedDayRegister[day];
        reports.AddRange(pendingNews);
        pendingNews.Clear();

        publishedDay = day;
        CollectRewards(reports);
        ShowEdition(reports, day - 1);

        Main_Referencer.Instance.manager.questManager.RefreshDiscoveredResults();
        GameSaveSystem.RequestAutosave();
    }

    internal void CaptureProgress(QuestSaveData data)
    {
        data.publishedDay = publishedDay;
        data.newspaperOpen = timeScaleBeforeJournal.HasValue;
        data.timeScaleBeforeJournal = timeScaleBeforeJournal.GetValueOrDefault(1f);
        data.rewardsOpen = rewardsOpen;
        if (data.newspaperOpen)
        {
            data.journalScroll = visual.scrollRect_Journal.verticalNormalizedPosition;
            data.newspaperLayouts.AddRange(smallNewsLayouts);
        }
        data.coinsReward = totalCoins;
        data.popularityReward = totalPopularityXp;
        data.recruitReward = totalRecruits;
        foreach (var day in Main_Referencer.Instance.value.realizedDayRegister)
        {
            var edition = new QuestSaveData.Edition();
            foreach (var report in day)
                edition.reports.Add(SaveReport(report));
            data.editions.Add(edition);
        }
        foreach (var report in pendingNews)
            data.pendingReports.Add(SaveReport(report));
        foreach (var reward in monsterRewards)
        {
            data.monsterRewards.Add(new QuestSaveData.MonsterReward
            {
                monsterId = reward.monster.monstersProfile.SaveId,
                startLevel = reward.startLevel, startXp = reward.startXp,
                endLevel = reward.endLevel, endXp = reward.endXp
            });
        }
    }

    private static QuestSaveData.Report SaveReport(
        (CreateQuest_Data data, MonsterReferencer.Monsters monster, bool isWinned, bool attempted) report)
    {
        return new QuestSaveData.Report
        {
            questId = report.data.SaveId, monsterId = report.monster?.monstersProfile.SaveId,
            won = report.isWinned, attempted = report.attempted
        };
    }

    internal void RestoreProgress(QuestSaveData data, QuestSaveData.Catalog catalog)
    {
        var editions = Main_Referencer.Instance.value.realizedDayRegister;
        editions.Clear();
        foreach (var edition in data.editions)
            editions.Add(edition.reports.ConvertAll(report =>
                (catalog.Quest(report.questId), catalog.Monster(report.monsterId), report.won, report.attempted)));
        pendingNews.Clear();
        foreach (var report in data.pendingReports)
            pendingNews.Add((catalog.Quest(report.questId), catalog.Monster(report.monsterId), report.won, report.attempted));

        publishedDay = data.publishedDay;
        totalCoins = data.coinsReward;
        totalPopularityXp = data.popularityReward;
        totalRecruits = data.recruitReward;
        monsterRewards.Clear();
        foreach (var reward in data.monsterRewards)
            monsterRewards.Add((catalog.Monster(reward.monsterId), reward.startLevel, reward.startXp, reward.endLevel, reward.endXp));

        if (data.newspaperOpen)
        {
            // The stored edition has already credited its rewards. Only rebuild its presentation.
            timeScaleBeforeJournal = data.timeScaleBeforeJournal;
            ShowEdition(editions[publishedDay], publishedDay - 1, data.newspaperLayouts);
            if (data.rewardsOpen)
                OpenRewards();
            visual.scrollRect_Journal.verticalNormalizedPosition = data.journalScroll;
        }
    }

    private void ShowEdition(
        List<(CreateQuest_Data data, MonsterReferencer.Monsters monster, bool isWinned, bool attempted)> reports,
        int editionDay, List<int> layouts = null)
    {
        visual.dayLabel_Journal.text = $"Day {editionDay:000}";
        ClearRewardRows();
        ShowNewsPage(reports.Count > 0);
        RenderNews(reports, layouts);
        timeScaleBeforeJournal ??= Time.timeScale;
        Time.timeScale = 0f;
        OpenJournalCanvas();
        ResetPageScroll();
    }

    private void OpenJournalCanvas()
    {
        GameObject journalCanvas = visual.journalCanvas_Journal;
        entranceTween?.Kill();

        if (journalPanel == null || entranceDuration <= 0f)
        {
            journalCanvas.SetActive(true);
            return;
        }

        Vector2 restingPosition = journalPanel.anchoredPosition;
        float hiddenY = -journalPanel.rect.yMax * journalPanel.localScale.y - 8f;
        journalPanel.anchoredPosition = new Vector2(restingPosition.x, hiddenY);
        journalCanvas.SetActive(true);

        entranceTween = journalPanel.DOAnchorPosY(restingPosition.y, entranceDuration)
            .SetEase(Ease.OutCubic)
            .SetUpdate(true)
            .OnKill(() =>
            {
                entranceTween = null;
                if (journalPanel != null)
                {
                    journalPanel.anchoredPosition = restingPosition;
                }
            });
    }

    public bool? GetLatestDiscoveredResult(CreateQuest_Data quest, MonsterReferencer.Monsters monster)
    {
        var results = Main_Referencer.Instance.value.realizedDayRegister;

        for (int day = results.Count - 1; day >= 0; day--)
        {
            var reports = results[day];

            for (int index = reports.Count - 1; index >= 0; index--)
            {
                var report = reports[index];

                if (report.attempted && report.data == quest && report.monster == monster)
                {
                    return report.isWinned;
                }
            }
        }

        return null;
    }

    public void GetDiscoveredMonsters(
        CreateQuest_Data quest,
        HashSet<MonsterReferencer.Monsters> good,
        HashSet<MonsterReferencer.Monsters> bad)
    {
        good.Clear();
        bad.Clear();

        foreach (var reports in Main_Referencer.Instance.value.realizedDayRegister)
        {
            foreach (var report in reports)
            {
                if (report.attempted && report.data == quest && report.monster != null)
                {
                    (report.isWinned ? good : bad).Add(report.monster);
                }
            }
        }
    }

    public void InstanceNew(int day)
    {
        var editions = Main_Referencer.Instance.value.realizedDayRegister;
        RenderNews(day >= 0 && day < editions.Count ? editions[day] : null);
    }

    private void RenderNews(
        List<(CreateQuest_Data data, MonsterReferencer.Monsters monster, bool isWinned, bool attempted)> reports,
        List<int> layouts = null)
    {
        CleanJournal();
        visual.smallNewsDivider_Journal.SetActive(false);
        if (reports == null)
        {
            return;
        }

        ValuesReferencer values = Main_Referencer.Instance.value;
        int featured = -1;
        if (!reports.Exists(report => report.data.type == CreateQuest_Data.QuestType.Route))
        {
            featured = reports.FindIndex(report => report.attempted && report.monster != null);
        }

        smallNewsLayouts.Clear();
        for (int index = 0; index < reports.Count; index++)
        {
            var news = reports[index];
            bool isHeadline = index == featured || news.data.type == CreateQuest_Data.QuestType.Route;
            GameObject prefab = visual.routeNew_Journal;
            if (!isHeadline)
            {
                int indexInEdition = smallNewsLayouts.Count;
                int layout = layouts != null && indexInEdition < layouts.Count
                    ? layouts[indexInEdition]
                    : Random.Range(0, visual.singleNews_Journal.Length);
                smallNewsLayouts.Add(layout);
                prefab = visual.singleNews_Journal[layout];
            }
            Transform container = isHeadline
                ? visual.routeNewContainer_Journal.transform
                : visual.singleNewContainer_Journal.transform;
            GameObject newObject = Instantiate(prefab, container);
            instantiatedNews.Add(newObject);

            JournalNew_Info info = newObject.GetComponent<JournalNew_Info>();

            var result = news.isWinned ? news.data.goodResult : news.data.badResult;

            if (result == null || result.IsEmpty)
            {
                result = news.isWinned ? values.MissingWinArticle_JournalValues : values.MissingLossArticle_JournalValues;
            }

            info.SetMonsterImage(news.attempted ? news.monster?.monstersProfile : null);
            info.setInfo(news.data, result);
        }

        visual.smallNewsDivider_Journal.SetActive(smallNewsLayouts.Count > 1);
    }

    public void CleanJournal()
    {
        foreach (var news in instantiatedNews)
        {
            news.SetActive(false);
            Destroy(news);
        }

        instantiatedNews.Clear();
    }

    public void OpenRewards()
    {
        if (rewardsOpen || !visual.journalCanvas_Journal.activeSelf)
        {
            return;
        }

        Transform buttonTransform = visual.tabButton_Journal.transform;
        buttonTransform.DOKill();

        DoTweenEffects_Commands.DoTweenSpin(buttonTransform, 0.3f, -180);

        ValuesReferencer values = Main_Referencer.Instance.value;
        visual.tabText_Journal.StringReference = values.NewsTabText_JournalValues;
        visual.titleText_Journal.StringReference = values.RewardsText_JournalValues;

        visual.routeNewContainer_Journal.SetActive(false);
        visual.singleNewContainer_Journal.SetActive(false);
        visual.rewardsNew_Journal.SetActive(true);

        rewardsOpen = true;
        UpdateRewards(publishedDay);
        ResetPageScroll();
    }

    public void CloseRewards()
    {
        visual.tabButton_Journal.transform.DOKill();
        ClearRewardRows();
        HideEdition();
        GameSaveSystem.RequestAutosave();
    }

    private void HideEdition()
    {
        if (visual != null && visual.journalCanvas_Journal != null)
        {
            visual.journalCanvas_Journal.SetActive(false);
        }

        entranceTween?.Kill();
        rewardsOpen = false;
        if (timeScaleBeforeJournal.HasValue)
        {
            Time.timeScale = timeScaleBeforeJournal.Value;
            timeScaleBeforeJournal = null;
        }
    }

    private void ShowNewsPage(bool hasNews)
    {
        Transform buttonTransform = visual.tabButton_Journal.transform;
        buttonTransform.DOKill();
        buttonTransform.localRotation = Quaternion.identity;

        ValuesReferencer values = Main_Referencer.Instance.value;
        visual.tabText_Journal.StringReference = values.RewardsTabText_JournalValues;
        visual.titleText_Journal.StringReference = hasNews ? values.DailyNewsText_JournalValues : values.NoMissionResults_JournalValues;

        visual.rewardsNew_Journal.SetActive(false);
        visual.routeNewContainer_Journal.SetActive(true);
        visual.singleNewContainer_Journal.SetActive(true);

        rewardsOpen = false;
    }

    public void ToggleRewards()
    {
        if (rewardsOpen)
        {
            CloseRewards();
            return;
        }

        OpenRewards();
    }

    private void CollectRewards(
        List<(CreateQuest_Data data, MonsterReferencer.Monsters monster, bool isWinned, bool attempted)> reports)
    {
        totalPopularityXp = 0;
        totalCoins = 0;
        totalRecruits = 0;
        monsterRewards.Clear();
        Dictionary<MonsterReferencer.Monsters, long> experienceByMonster = new();

        foreach (var news in reports)
        {
            if (!news.isWinned)
            {
                continue;
            }

            if (news.data.haspopularityXpReward)
            {
                totalPopularityXp += Mathf.Max(0, news.data.popularityXpEarned);
            }

            if (news.data.hasCoinsReward)
            {
                totalCoins += Mathf.Max(0, news.data.coinsEarned);
            }

            if (news.data.hasCharacterReward)
            {
                totalRecruits += news.data.characterEarned?.Length ?? 0;
            }

            if (news.monster == null || news.data.characterXpEarned <= 0)
            {
                continue;
            }

            experienceByMonster.TryGetValue(news.monster, out long experience);
            experienceByMonster[news.monster] = experience + news.data.characterXpEarned;
        }

        ManagerReferencer managers = Main_Referencer.Instance.manager;
        managers.popularityManager.GrantExperience(totalPopularityXp);
        managers.moneyManager.Credit(totalCoins);

        foreach (var reward in experienceByMonster)
        {
            MonsterReferencer.Monsters monster = reward.Key;
            int startLevel = monster.monsterLevel;
            int startXp = monster.monsterExperience;
            monster.GainExperience((int)System.Math.Min(reward.Value, int.MaxValue));
            monsterRewards.Add((monster, startLevel, startXp, monster.monsterLevel, monster.monsterExperience));
        }
    }

    public void UpdateRewards(int day)
    {
        if (!rewardsOpen || day != publishedDay)
        {
            return;
        }

        ClearRewardRows();
        foreach (var reward in monsterRewards)
        {
            GameObject row = Instantiate(visual.characterXpElement_Journal, visual.rewardsNew_Journal.transform);
            row.transform.SetSiblingIndex(instantiatedCharacterXp.Count);
            instantiatedCharacterXp.Add(row);
            row.GetComponent<JournalMonsterReward>().Play(
                reward.monster.monstersProfile.icon, reward.startLevel, reward.startXp, reward.endLevel, reward.endXp);
        }

        Main_Referencer references = Main_Referencer.Instance;
        ValuesReferencer values = references.value;
        var rewards = visual.rewardsNewComponents_Journal;
        float progress = values.popularityLevel_Popularity >= values.maxPopularityLevel_Popularity
            ? 1f
            : Mathf.Clamp01((float)values.popularityXp_Popularity / PopularityManager.ExperiencePerLevel);
        rewards[1].progressSlider.SetValueWithoutNotify(progress);
        rewards[1].LevelValueText.text = values.popularityLevel_Popularity.ToString();
        rewards[1].numbersValueText.gameObject.SetActive(true);
        rewards[1].numbersValueText.text = $"+{totalPopularityXp:N0} XP";
        rewards[2].numbersValueText.text = $"+{totalCoins:N0}  |  Total {references.manager.moneyManager.Balance:N0}";
        rewards[3].numbersValueText.text = totalRecruits.ToString();
    }

    private void ClearRewardRows()
    {
        foreach (GameObject row in instantiatedCharacterXp)
        {
            row.SetActive(false);
            Destroy(row);
        }

        instantiatedCharacterXp.Clear();
    }

    private void ResetPageScroll()
    {
        // Size the active sections before their parent reads their heights.
        if (rewardsOpen)
        {
            UnityEngine.UI.LayoutRebuilder.ForceRebuildLayoutImmediate((RectTransform)visual.rewardsNew_Journal.transform);
        }
        else
        {
            UnityEngine.UI.LayoutRebuilder.ForceRebuildLayoutImmediate((RectTransform)visual.routeNewContainer_Journal.transform);
            UnityEngine.UI.LayoutRebuilder.ForceRebuildLayoutImmediate((RectTransform)visual.singleNewContainer_Journal.transform);
        }

        UnityEngine.UI.LayoutRebuilder.ForceRebuildLayoutImmediate((RectTransform)visual.rewardsNew_Journal.transform.parent);
        UnityEngine.UI.ScrollRect journalScroll = visual.scrollRect_Journal;
        UnityEngine.UI.LayoutRebuilder.ForceRebuildLayoutImmediate(journalScroll.content);
        Canvas.ForceUpdateCanvases();
        journalScroll.StopMovement();
        journalScroll.verticalNormalizedPosition = 1f;
    }
}
