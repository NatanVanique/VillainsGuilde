using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

[Serializable]
public sealed class QuestSaveData
{
    [Serializable]
    public sealed class Stat
    {
        public CreateMonster_Data.StatsTypes type;
        public float value;
    }

    [Serializable]
    public sealed class Monster
    {
        public string id;
        public bool contracted;
        public int level;
        public int experience;
        public float happiness;
        public List<Stat> stats = new();
    }

    [Serializable]
    public sealed class Mail
    {
        public string questId;
        public string assignedMonsterId;
        public float remaining;
        public bool infoOpen;
        public string selectedMonsterId;
    }

    [Serializable]
    public sealed class Mission
    {
        public string questId;
        public string monsterId;
        public bool accepted;
        public float remaining;
    }

    [Serializable]
    public sealed class Report
    {
        public string questId;
        public string monsterId;
        public bool won;
        public bool attempted;
    }

    [Serializable]
    public sealed class Edition
    {
        public List<Report> reports = new();
    }

    [Serializable]
    public sealed class MonsterReward
    {
        public string monsterId;
        public int startLevel, startXp, endLevel, endXp;
    }

    public int nextRouteIndex;
    public List<string> completedQuests = new();
    public List<Monster> monsters = new();
    public List<Mail> mails = new();
    public List<Mission> missions = new();
    public bool voiceMailOpen;
    public List<Edition> editions = new();
    public List<Report> pendingReports = new();
    public int publishedDay = -1;
    public bool waitingForFailedPanel;
    public bool newspaperOpen, rewardsOpen;
    public List<int> newspaperLayouts = new();
    public float journalScroll, timeScaleBeforeJournal;
    public long popularityReward, coinsReward;
    public int recruitReward;
    public List<MonsterReward> monsterRewards = new();

    public static QuestSaveData Capture(Main_Referencer refs)
    {
        var data = new QuestSaveData();
        foreach (var reference in GetQuestReferences(refs.quest))
            if (reference.questCompleted)
                data.completedQuests.Add(reference.quest.SaveId);

        foreach (var monster in refs.monster.monsters)
        {
            if (monster?.monstersProfile == null)
                continue;

            var record = new Monster
            {
                id = monster.monstersProfile.SaveId,
                contracted = monster.contracted,
                level = monster.monsterLevel,
                experience = monster.monsterExperience,
                happiness = monster.happiness
            };
            foreach (var stat in monster.statusUpgrade ?? Array.Empty<MonsterReferencer.statusUpgradeValues>())
                if (stat != null)
                    record.stats.Add(new Stat { type = stat.upgradeStatType, value = stat.upgradeValue });
            data.monsters.Add(record);
        }

        refs.manager.questManager.CaptureProgress(data);
        refs.manager.journalManager.CaptureProgress(data);
        return data;
    }

    public void Restore(Main_Referencer refs)
    {
        var catalog = new Catalog(refs);
        var completed = new HashSet<string>(completedQuests);
        foreach (var reference in GetQuestReferences(refs.quest))
            reference.questCompleted = completed.Contains(reference.quest.SaveId);

        foreach (var record in monsters)
        {
            var monster = catalog.Monster(record.id);
            monster.contracted = record.contracted;
            monster.monsterLevel = record.level;
            monster.monsterExperience = record.experience;
            monster.happiness = record.happiness;
            foreach (var stat in record.stats)
            {
                var upgrade = Array.Find(monster.statusUpgrade ?? Array.Empty<MonsterReferencer.statusUpgradeValues>(),
                    entry => entry != null && entry.upgradeStatType == stat.type);
                if (upgrade != null)
                    upgrade.upgradeValue = stat.value;
            }
        }

        // Mail cards display discoveries from published editions. Restore the historical records first.
        refs.manager.journalManager.RestoreProgress(this, catalog);
        refs.manager.questManager.RestoreProgress(this, catalog);
    }

    private static IEnumerable<QuestReferencer.QuestReference> GetQuestReferences(QuestReferencer referencer)
    {
        foreach (var reference in referencer.singleQuests ?? Array.Empty<QuestReferencer.QuestReference>())
            if (reference?.quest != null)
                yield return reference;

        foreach (var route in referencer.routeQuests ?? Array.Empty<QuestReferencer.QuestRoute>())
            foreach (var reference in route?.quests ?? Array.Empty<QuestReferencer.QuestReference>())
                if (reference?.quest != null)
                    yield return reference;
    }

    internal sealed class Catalog
    {
        private readonly Dictionary<string, CreateQuest_Data> quests = new();
        private readonly Dictionary<string, MonsterReferencer.Monsters> monsters = new();

        internal Catalog(Main_Referencer refs)
        {
            foreach (var reference in GetQuestReferences(refs.quest))
                quests[reference.quest.SaveId] = reference.quest;
            foreach (var monster in refs.monster.monsters)
                if (monster?.monstersProfile != null)
                    monsters.Add(monster.monstersProfile.SaveId, monster);
        }

        internal CreateQuest_Data Quest(string id)
        {
            if (!string.IsNullOrEmpty(id) && quests.TryGetValue(id, out var quest))
                return quest;
            throw new InvalidDataException($"Saved quest '{id}' is missing from the game catalog.");
        }

        internal MonsterReferencer.Monsters Monster(string id)
        {
            if (string.IsNullOrEmpty(id))
                return null;
            if (monsters.TryGetValue(id, out var monster))
                return monster;
            throw new InvalidDataException($"Saved monster '{id}' is missing from the game catalog.");
        }
    }
}
