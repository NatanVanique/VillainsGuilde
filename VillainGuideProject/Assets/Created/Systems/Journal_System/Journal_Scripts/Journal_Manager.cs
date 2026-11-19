
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.Localization;

public sealed class JournalManager : MonoBehaviour
{
    [SerializeField] private GameObject journalCanvas;
    [SerializeField] private TMPro.TextMeshProUGUI dayLabel;

    private readonly List<GameObject> instantiatedNews = new();
    private readonly List<GameObject> instantiatedCharacterXp = new();
    private readonly List<(MonsterReferencer.Monsters monster, int startLevel, int startXp, int endLevel, int endXp)> monsterRewards = new();
    private readonly LocalizedString emptyEditionTitle = new("Journal", "NoMissionResults_Journal");
    private readonly LocalizedString missingWinArticle = new("Journal", "MissingWinArticle_Journal");
    private readonly LocalizedString missingLossArticle = new("Journal", "MissingLossArticle_Journal");

    private readonly List<(CreateQuest_Data data, MonsterReferencer.Monsters monster, bool isWinned, bool attempted)> pendingNews = new();

    private DaycicleManager daycicle;
    private VisualReferencer visual;
    private UnityEngine.UI.ScrollRect journalScroll;

    private int publishedDay = -1;
    private float? timeScaleBeforeJournal;
    private bool rewardsOpen;
    private int totalPopularityXp;
    private int totalCoins;
    private int totalRecruits;

    private void Start()
    {
        Main_Referencer references = Main_Referencer.Instance;
        visual = references.visual;
        journalScroll = visual.rewardsNew_Journal.GetComponentInParent<UnityEngine.UI.ScrollRect>(true);
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
            if (journalCanvas != null)
            {
                journalCanvas.SetActive(false);
            }

            ResumeGame();
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
        dayLabel.text = $"Day {day - 1:000}";
        CollectRewards(reports);
        ClearRewardRows();
        ShowNewsPage(reports.Count > 0);
        InstanceNew(day);
        timeScaleBeforeJournal ??= Time.timeScale;
        Time.timeScale = 0f;
        journalCanvas.SetActive(true);
        ResetPageScroll();

        Main_Referencer.Instance.manager.questManager.RefreshDiscoveredResults();
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
        CleanJournal();

        if (day < 0 || day >= Main_Referencer.Instance.value.realizedDayRegister.Count)
        {
            return;
        }

        var reports = Main_Referencer.Instance.value.realizedDayRegister[day];
        int featured = -1;
        if (!reports.Exists(report => report.data.type == CreateQuest_Data.QuestType.Route))
        {
            featured = reports.FindIndex(report => report.attempted && report.monster != null);
        }

        for (int index = 0; index < reports.Count; index++)
        {
            var news = reports[index];
            bool isHeadline = index == featured || news.data.type == CreateQuest_Data.QuestType.Route;
            GameObject prefab = isHeadline
                ? visual.routeNew_Journal
                : visual.singleNews_Journal[Random.Range(0, visual.singleNews_Journal.Length)];
            Transform container = isHeadline
                ? visual.routeNewContainer_Journal.transform
                : visual.singleNewContainer_Journal.transform;
            GameObject newObject = Instantiate(prefab, container);
            instantiatedNews.Add(newObject);

            JournalNew_Info info = newObject.GetComponent<JournalNew_Info>();

            var result = news.isWinned ? news.data.goodResult : news.data.badResult;

            if (result == null || result.IsEmpty)
            {
                result = news.isWinned ? missingWinArticle : missingLossArticle;
            }

            info.SetMonsterImage(news.attempted ? news.monster?.monstersProfile : null);
            info.setInfo(news.data, result);
        }
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
        if (rewardsOpen || !journalCanvas.activeSelf)
        {
            return;
        }

        Transform buttonTransform = visual.tabButton_Journal.transform;
        buttonTransform.DOKill();

        DoTweenEffects_Commands.DoTweenSpin(buttonTransform, 0.3f, -180);

        Main_Referencer.Instance.visual.tabText_Journal.StringReference =
            Main_Referencer.Instance.value.NewsTabText_JournalValues;

        Main_Referencer.Instance.visual.titleText_Journal.StringReference =
            Main_Referencer.Instance.value.RewardsText_JournalValues;

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
        journalCanvas.SetActive(false);
        rewardsOpen = false;
        ResumeGame();
    }

    private void ResumeGame()
    {
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

        Main_Referencer.Instance.visual.tabText_Journal.StringReference =
            Main_Referencer.Instance.value.RewardsTabText_JournalValues;

        Main_Referencer.Instance.visual.titleText_Journal.StringReference =
            hasNews ? Main_Referencer.Instance.value.DailyNewsText_JournalValues : emptyEditionTitle;

        Main_Referencer.Instance.visual.rewardsNew_Journal.SetActive(false);
        Main_Referencer.Instance.visual.routeNewContainer_Journal.SetActive(true);
        Main_Referencer.Instance.visual.singleNewContainer_Journal.SetActive(true);

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
        Dictionary<MonsterReferencer.Monsters, int> experienceByMonster = new();

        foreach (var news in reports)
        {
            if (!news.isWinned)
            {
                continue;
            }

            totalPopularityXp += news.data.popularityXpEarned;
            totalCoins += news.data.coinsEarned;

            if (news.data.hasCharacterReward)
            {
                totalRecruits += news.data.characterEarned?.Length ?? 0;
            }

            if (news.monster == null || news.data.characterXpEarned <= 0)
            {
                continue;
            }

            experienceByMonster.TryGetValue(news.monster, out int experience);
            experienceByMonster[news.monster] = experience + news.data.characterXpEarned;
        }

        foreach (var reward in experienceByMonster)
        {
            MonsterReferencer.Monsters monster = reward.Key;
            int startLevel = monster.monsterLevel;
            int startXp = monster.monsterExperience;
            monster.GainExperience(reward.Value);
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

        var rewards = visual.rewardsNewComponents_Journal;
        rewards[1].progressSlider.SetValueWithoutNotify(Mathf.Clamp01(totalPopularityXp / 100f));
        rewards[1].LevelValueText.text = Main_Referencer.Instance.value.popularityLevel_Popularity.ToString();
        rewards[2].numbersValueText.text = totalCoins.ToString();
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
        Canvas.ForceUpdateCanvases();
        journalScroll.StopMovement();
        journalScroll.verticalNormalizedPosition = 1f;
    }
}
