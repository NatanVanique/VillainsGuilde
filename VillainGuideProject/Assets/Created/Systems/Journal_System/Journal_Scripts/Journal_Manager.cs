using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public sealed class JournalManager : MonoBehaviour
{
    private readonly List<GameObject> instantiatedNews = new();
    
    private bool rewardsOpen = false;

    public void RegisterNew(CreateQuest_Data data, CreateMonster_Data monster, bool isWinned )
    {
        ValuesReferencer values = Main_Referencer.Instance.value;
        int day = values.currentDay_HourValues;
        while (values.realizedDayRegister.Count <= day)
        {
            values.realizedDayRegister.Add(new());
        }

        values.realizedDayRegister[day].Add((data, monster, isWinned));
        InstanceNew(day);
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

        foreach (var news  in Main_Referencer.Instance.value.realizedDayRegister[day])
        {
            if (news.data.type == CreateQuest_Data.QuestType.Single)
            {
                curNew = Main_Referencer.Instance.visual.singleNews_Journal
                    [Random.Range(0, Main_Referencer.Instance.visual.singleNews_Journal.Length)];

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
                info.newDescription.OnUpdateString.Invoke(news.isWinned ? "Mission succeeded." : "Mission failed.");
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
        
        Main_Referencer.Instance.visual.tabText_Journal.StringReference = Main_Referencer.Instance.value.NewsTabText_JournalValues;
        
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
        
        DoTweenEffects_Commands.DoTweenSpin(buttonTransform, 0.3f, -0);
        
        Main_Referencer.Instance.visual.tabText_Journal.StringReference = Main_Referencer.Instance.value.RewardsTabText_JournalValues;
        
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
        int totalCharacterXp = 0;
        int totalPopularityXp = 0;
        int totalCoins = 0;
        int totalMonsters = 0;
        
        foreach (var news in Main_Referencer.Instance.value.realizedDayRegister[day])
        {
            if (!news.isWinned)
            {
                continue;
            }

            totalCharacterXp += news.data.characterXpEarned;
            totalPopularityXp += news.data.popularityXpEarned;
            totalCoins += news.data.coinsEarned;

            if (news.data.hasCharacterReward)
            {
                totalMonsters++;
            }
        }
        
        //CharactersXP
        Main_Referencer.Instance.visual.rewardsNewComponents_Journal[0].progressSlider.value = totalCharacterXp;
        
        //adicionar verificação de se xp atingiu o maximo aumenta o nivel e continua somando xp
        
        //Tem que pegar o nivel do monstro pra mostrar, mas como?
        
        //PopularityXp
        Main_Referencer.Instance.visual.rewardsNewComponents_Journal[1].progressSlider.value = totalPopularityXp;
        
        //adicionar verificação de se xp atingiu o maximo aumenta o nivel e continua somando xp
        
        Main_Referencer.Instance.visual.rewardsNewComponents_Journal[1].LevelValueText.text =
            Main_Referencer.Instance.value.popularityLevel_Popularity.ToString();
        
        //Coins
        Main_Referencer.Instance.visual.rewardsNewComponents_Journal[2].numbersValueText.text = totalCoins.ToString();
        
        //Characters
        Main_Referencer.Instance.visual.rewardsNewComponents_Journal[3].numbersValueText.text = totalMonsters.ToString();
    }
}
