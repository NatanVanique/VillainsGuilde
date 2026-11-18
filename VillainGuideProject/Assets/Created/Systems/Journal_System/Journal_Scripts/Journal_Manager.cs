using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public sealed class JournalManager : MonoBehaviour
{
    private List<GameObject> instantiatedNews = new List<GameObject>();
    private List<CreateQuest_Data> winnedNews = new List<CreateQuest_Data>();
    private bool rewardsOpen = false;

    public void InstanceNew(CreateQuest_Data data, bool isWinned)
    {
        GameObject curNew;
        Transform curContainer;

        if (data.type == CreateQuest_Data.QuestType.Single)
        {
            curNew = Main_Referencer.Instance.visual.singleNews_Journal
                [Random.Range(0, Main_Referencer.Instance.visual.singleNews_Journal.Length)];

            curContainer = Main_Referencer.Instance.visual.singleNewContainer_Journal.transform;
        }
        else if (data.type == CreateQuest_Data.QuestType.Route)
        {
            curNew = Main_Referencer.Instance.visual.routeNew_Journal;
            curContainer = Main_Referencer.Instance.visual.routeNewContainer_Journal.transform;
        }
        else
        {
            return;
        }

        GameObject newObject = Instantiate(curNew, curContainer);

        if (isWinned)
        {
            curNew.GetComponent<JournalNew_Info>().setInfo(data, data.goodResult);
            RegisterWinned(data);
        }
        else
        {
            curNew.GetComponent<JournalNew_Info>().setInfo(data, data.badResult);
        }
        
        instantiatedNews.Add(newObject);
    }

    public void RegisterWinned(CreateQuest_Data data)
    {
        winnedNews.Add(data);
    }
    
    public void CleanJournal()
    {
        foreach (GameObject news in instantiatedNews)
        {
            Destroy(news);
        }

        instantiatedNews.Clear();
        winnedNews.Clear();
    }

    public void OpenRewards()
    {
        Transform buttonTransform = Main_Referencer.Instance.visual.tabButton_Journal.transform;
        buttonTransform.DOKill();
        
        DoTweenEffects_Commands.DoTweenSpin(buttonTransform, 0.3f, -180);
        
        Main_Referencer.Instance.visual.tabText_Journal.StringReference = Main_Referencer.Instance.value.NewsTabText_JournalValues;
        
        Main_Referencer.Instance.visual.titleText_Journal.StringReference =
            Main_Referencer.Instance.value.RewardsText_JournalValues;
            
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


}
