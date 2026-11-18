
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public sealed class JournalManager : MonoBehaviour
{
    private readonly List<GameObject> instantiatedNews = new();
    private readonly List<GameObject> instantiatedCharacterXp = new();

    private readonly List<(CreateQuest_Data data, MonsterReferencer.Monsters monster, bool isWinned, bool attempted)> pendingNews = new();

    private DaycicleManager daycicle;

    private bool rewardsOpen = false;

    private void Start()
    {
        daycicle = Main_Referencer.Instance.manager.dayCicleManager;
        daycicle.BeforeOfficeDayStart += PublishResults;
    }

    private void OnDestroy()
    {
        if (daycicle != null)
        {
            daycicle.BeforeOfficeDayStart -= PublishResults;
        }
    }

    public void RegisterNew(CreateQuest_Data data, MonsterReferencer.Monsters monster, bool isWinned, bool attempted = true)
    {
        pendingNews.Add((data, monster, isWinned, attempted));
    }

    private void PublishResults()
    {
        if (pendingNews.Count == 0)
        {
            return;
        }

        ValuesReferencer values = Main_Referencer.Instance.value;
        int day = values.currentDay_HourValues;

        while (values.realizedDayRegister.Count <= day)
        {
            values.realizedDayRegister.Add(new());
        }

        values.realizedDayRegister[day].AddRange(pendingNews);
        pendingNews.Clear();

        InstanceNew(day);

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

        GameObject curNew;
        Transform curContainer;

        foreach (var news in Main_Referencer.Instance.value.realizedDayRegister[day])
        {
            if (news.data.type == CreateQuest_Data.QuestType.Single)
            {
                curNew = Main_Referencer.Instance.visual.singleNews_Journal[
                    Random.Range(0, Main_Referencer.Instance.visual.singleNews_Journal.Length)];

                curContainer = Main_Referencer.Instance.visual.singleNewContainer_Journal.transform;
            }
            else if (news.data.type == CreateQuest_Data.QuestType.Route)
            {
                curNew = Main_Referencer.Instance.visual.routeNew_Journal;
                curContainer = Main_Referencer.Instance.visual.routeNewContainer_Journal.transform;
            }
            else
            {
                return;
            }

            GameObject newObject = Instantiate(curNew, curContainer);
            instantiatedNews.Add(newObject);

            JournalNew_Info info = newObject.GetComponent<JournalNew_Info>();

            var result = news.isWinned ? news.data.goodResult : news.data.badResult;

            info.setInfo(news.data, result);

            if (result == null || result.IsEmpty)
            {
                info.newDescription.OnUpdateString.Invoke(
                    news.isWinned ? "Mission succeeded." : "Mission failed.");
            }
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
        Transform buttonTransform = Main_Referencer.Instance.visual.tabButton_Journal.transform;
        buttonTransform.DOKill();

        DoTweenEffects_Commands.DoTweenSpin(buttonTransform, 0.3f, -180);

        Main_Referencer.Instance.visual.tabText_Journal.StringReference =
            Main_Referencer.Instance.value.NewsTabText_JournalValues;

        Main_Referencer.Instance.visual.titleText_Journal.StringReference =
            Main_Referencer.Instance.value.RewardsText_JournalValues;

        Main_Referencer.Instance.visual.rewardsNew_Journal.SetActive(true);
        Main_Referencer.Instance.visual.routeNewContainer_Journal.SetActive(false);
        Main_Referencer.Instance.visual.singleNewContainer_Journal.SetActive(false);

        rewardsOpen = true;
    }

    public void CloseRewards()
    {
        Transform buttonTransform = Main_Referencer.Instance.visual.tabButton_Journal.transform;
        buttonTransform.DOKill();

        DoTweenEffects_Commands.DoTweenSpin(buttonTransform, 0.3f, 0);

        Main_Referencer.Instance.visual.tabText_Journal.StringReference =
            Main_Referencer.Instance.value.RewardsTabText_JournalValues;

        Main_Referencer.Instance.visual.titleText_Journal.StringReference =
            Main_Referencer.Instance.value.DailyNewsText_JournalValues;

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

    public void UpdateRewards(int day)
    {
        int totalPopularityXp = 0;
        int totalCoins = 0;
        int totalMonsters = 0;

        Dictionary<MonsterReferencer.Monsters, int> monstersUsed = new();

        foreach (var news in Main_Referencer.Instance.value.realizedDayRegister[day])
        {
            if (!news.isWinned)
            {
                continue;
            }

            totalPopularityXp += news.data.popularityXpEarned;
            totalCoins += news.data.coinsEarned;

            if (news.data.hasCharacterReward)
            {
                totalMonsters++;
            }

            if (news.monster == null)
            {
                continue;
            }

            monstersUsed.TryAdd(news.monster, 0);
            monstersUsed[news.monster] += news.data.characterXpEarned;
        }

        foreach (var obj in instantiatedCharacterXp)
        {
            Destroy(obj);
        }

        instantiatedCharacterXp.Clear();

        foreach (var monster in monstersUsed)
        {
            GameObject obj = Instantiate(
                Main_Referencer.Instance.visual.characterXpElement_Journal,
                Main_Referencer.Instance.visual.rewardsNew_Journal.transform.parent
            );

            instantiatedCharacterXp.Add(obj);

            obj.GetComponentInChildren<Slider>().value = monster.Value;
        }

        //PopularityXp
        Main_Referencer.Instance.visual.rewardsNewComponents_Journal[1].progressSlider.value =
            totalPopularityXp;

        Main_Referencer.Instance.visual.rewardsNewComponents_Journal[1].LevelValueText.text =
            Main_Referencer.Instance.value.popularityLevel_Popularity.ToString();

        //Coins
        Main_Referencer.Instance.visual.rewardsNewComponents_Journal[2].numbersValueText.text =
            totalCoins.ToString();

        //Characters
        Main_Referencer.Instance.visual.rewardsNewComponents_Journal[3].numbersValueText.text =
            totalMonsters.ToString();
    }
}
