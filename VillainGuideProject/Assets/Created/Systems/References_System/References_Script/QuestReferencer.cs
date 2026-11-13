using System;
using TriInspector;
using UnityEngine;

[DeclareBoxGroup("Single Quests", Title = "Single Quests")]
[DeclareBoxGroup("Routes", Title = "Routes")]
public class QuestReferencer : MonoBehaviour
{
    [Serializable]
    [DeclareBoxGroup("Quest", Title = "Quest")]
    [DeclareBoxGroup("Good Monsters", Title = "Good Monsters")]
    [DeclareBoxGroup("Bad Monsters", Title = "Bad Monsters")]
    public class QuestReference
    {
        [ReadOnly]
        [LabelText("Name")]
        [Group("Quest")]
        public string displayName;

        [Group("Quest")]
        public CreateQuest_Data quest;

        [Group("Quest")]
        public bool questCompleted;

        [Group("Good Monsters")]
        public string[] goodMonsters;

        [Group("Bad Monsters")]
        public string[] badMonsters;
    }

    [Serializable]
    [DeclareBoxGroup("Route", Title = "Route")]
    [DeclareBoxGroup("Quests", Title = "Quests")]
    public class QuestRoute
    {
        [Group("Route")]
        public string routeName;

        [Group("Quests")]
        [ListDrawerSettings(ShowElementLabels = true)]
        public QuestReference[] quests;
    }

    [Group("Single Quests")]
    [ListDrawerSettings(ShowElementLabels = true)]
    public QuestReference[] singleQuests;

    [Group("Routes")]
    [ListDrawerSettings(ShowElementLabels = true)]
    public QuestRoute[] routeQuests;

    private void OnValidate()
    {
        UpdateQuestDisplayNames(singleQuests);

        if (routeQuests == null) return;

        foreach (var route in routeQuests)
        {
            if (route == null) continue;

            UpdateQuestDisplayNames(route.quests);
        }
    }

    private static void UpdateQuestDisplayNames(QuestReference[] quests)
    {
        if (quests == null) return;

        foreach (var questReference in quests)
        {
            if (questReference == null) continue;

            questReference.displayName = questReference.quest != null
                ? questReference.quest.name
                : "Quest não configurada";
        }
    }
}
