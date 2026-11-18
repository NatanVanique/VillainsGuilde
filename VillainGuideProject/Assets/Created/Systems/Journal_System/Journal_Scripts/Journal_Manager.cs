using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEditor.Localization.Plugins.XLIFF.V20;
using UnityEngine.UI;

public sealed class JournalManager : MonoBehaviour
{
    private List<GameObject> instantiatedNews;
    
    private bool rewardsOpen = false;

    public void RegisterNew(CreateQuest_Data data, MonsterReferencer monster, bool isWinned )
    {
        Main_Referencer.Instance.value.realizedDayRegister.Add(new ());
        Main_Referencer.Instance.value.realizedDayRegister[^1].Add((data, monster, isWinned));
    }
    
    public void InstanceNew(int day)
    {
        CleanJournal();
        
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

            if (news.isWinned)
            {
                curNew.GetComponent<JournalNew_Info>().setInfo(news.data , news.data.goodResult);
            }
            else
            {
                curNew.GetComponent<JournalNew_Info>().setInfo(news.data, news.data.badResult);
            }
        }
    }
    
    public void CleanJournal()
    {
        foreach (var news in instantiatedNews)
        {
            Destroy(news);
        }
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
        int totalPopularityXp = 0;
        int totalCoins = 0;
        int totalMonsters = 0;
        Dictionary<MonsterReferencer, int> monstersUsed = new();
        
        
        //CharactersXP
        foreach (var news in Main_Referencer.Instance.value.realizedDayRegister[day])
        {
            if (news.monster == null) continue;

            monstersUsed.TryAdd(news.monster, 0);
            monstersUsed[news.monster] += news.isWinned ? news.data.characterXpEarned : 0;
        }
        
        foreach (var monster in monstersUsed)
        {
            GameObject obj = Instantiate(
                Main_Referencer.Instance.visual.characterXpElement_Journal,
                Main_Referencer.Instance.visual.rewardsNew_Journal.transform.parent
            );

            obj.GetComponentInChildren<Slider>().value = monster.Value;
        }
        
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
