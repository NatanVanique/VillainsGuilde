using System.Collections.Generic;
using UnityEngine;
using System;

public class JournalRegister : MonoBehaviour
{
    [Serializable]
    public class JournalPageInfo
    {
        public QuestData BigNew;
        public QuestData EventNew;
    }
    
    public List<QuestData> QuestsWinned = new List<QuestData>(); // Isso calcula o que você recebe
    public List<QuestData> QuestsLosed = new List<QuestData>(); // Isso calcula o que você perde
    public List<QuestData> SmallNews = new List<QuestData>();
    public JournalPageInfo JournalPage { get; private set; } = new JournalPageInfo();
    
    public void RegisterResult(bool playerWins, QuestData questScriptable)
    {
        if (questScriptable == null)
            return;

        List<QuestData> resultList = playerWins ? QuestsWinned : QuestsLosed;
        resultList.Add(questScriptable);

        if (playerWins)
        {
            if (questScriptable.Type == QuestType.Emergency)
            {
                JournalPage.BigNew = questScriptable;
            }
            else if (questScriptable.Type == QuestType.Mistery)
            {
                RegisterSmallNews(questScriptable);
            }
            else if (questScriptable.Type == QuestType.Classic)
            {
                JournalPage.EventNew = questScriptable;
            }
        }
        else
        {
            if (questScriptable.Type == QuestType.Emergency)
            {
                JournalPage.BigNew = questScriptable;
            }
            else if (questScriptable.Type == QuestType.Mistery)
            {
                RegisterSmallNews(questScriptable);
            }
            else if (questScriptable.Type == QuestType.Classic)
            {
                JournalPage.EventNew = questScriptable;
            }
        }
    }

    private void RegisterSmallNews(QuestData questScriptable)
    {
        SmallNews.Add(questScriptable);
        SmallNews.Sort((first, second) =>
            GetImportance(second).CompareTo(GetImportance(first)));
    }

    private float GetImportance(QuestData quest)
    {
        if (quest == null)
            return 0f;

        float value = 0f;
        if (quest.HasMoney)
            value += quest.MoneyReward;
        if (quest.HasXP)
            value += quest.XPReward;
        if (quest.HasCharacters && quest.ListOfCharacters != null)
            value += quest.ListOfCharacters.Count;

        return Mathf.Abs(value);
    }
}
