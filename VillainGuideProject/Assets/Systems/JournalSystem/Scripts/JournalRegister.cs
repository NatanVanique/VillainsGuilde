using System;
using System.Collections.Generic;
using UnityEngine;

public class JournalRegister : MonoBehaviour
{
    [Serializable]
    public class JournalEntry
    {
        public QuestData Quest;
        public bool PlayerWon;
        public float SignedValue;
        public List<MonsterInfo> Monsters;

        public float Importance => Mathf.Abs(SignedValue);

        public JournalEntry(QuestData quest, bool playerWon, float signedValue, List<MonsterInfo> monsters)
        {
            Quest = quest;
            PlayerWon = playerWon;
            SignedValue = signedValue;
            Monsters = monsters != null
                ? new List<MonsterInfo>(monsters)
                : new List<MonsterInfo>();
        }
    }

    public List<QuestData> QuestsWinned = new List<QuestData>();
    public List<QuestData> QuestsLosed = new List<QuestData>();
    public List<JournalEntry> SmallNews = new List<JournalEntry>();
    public JournalEntry BigNews { get; private set; }

    public void RegisterResult(bool playerWins, QuestData quest, List<MonsterInfo> monsters = null)
    {
        if (quest == null)
            return;

        (playerWins ? QuestsWinned : QuestsLosed).Add(quest);

        float signedValue = CalculateQuestValue(quest) * (playerWins ? 1f : -1f);
        JournalEntry entry = new JournalEntry(quest, playerWins, signedValue, monsters);

        if (quest.Type == QuestType.Emergency)
        {
            if (BigNews == null)
                BigNews = entry;
            else if (entry.Importance > BigNews.Importance)
            {
                SmallNews.Add(BigNews);
                BigNews = entry;
            }
            else
                SmallNews.Add(entry);
            SmallNews.Sort(CompareByImportance);
            return;
        }

        SmallNews.Add(entry);
        SmallNews.Sort(CompareByImportance);
    }

    public void ClearResults()
    {
        QuestsWinned.Clear();
        QuestsLosed.Clear();
        SmallNews.Clear();
        BigNews = null;
    }

    private static int CompareByImportance(JournalEntry first, JournalEntry second)
    {
        int importance = second.Importance.CompareTo(first.Importance);
        if (importance != 0)
            return importance;

        return second.SignedValue.CompareTo(first.SignedValue);
    }

    private static float CalculateQuestValue(QuestData quest)
    {
        float value = 0f;

        if (quest.HasMoney)
            value += Mathf.Abs(quest.MoneyReward);
        if (quest.HasXP)
            value += Mathf.Abs(quest.XPReward);
        if (quest.HasCharacters && quest.ListOfCharacters != null)
            value += quest.ListOfCharacters.Count;

        return value;
    }
}
