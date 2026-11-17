using System.Collections.Generic;
using UnityEngine;

public sealed class JournalManager : MonoBehaviour
{
    private List<GameObject> instantiatedNews = new List<GameObject>();

    public void InstanceNew(CreateQuest_Data data)
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
        instantiatedNews.Add(newObject);
    }

    public void CleanJournal()
    {
        foreach (GameObject news in instantiatedNews)
        {
            Destroy(news);
        }

        instantiatedNews.Clear();
    }
}