using System.Collections.Generic;
using UnityEngine;
using System;

public class JournalRegister : MonoBehaviour
{
    [Serializable]
    public class JournalPageInfo
    {
        public QuestData BigNew;
        public QuestData SmallNew;
        public QuestData EventNew;
    }
    
    public List<QuestData> QuestsWinned = new List<QuestData>(); // Isso calcula o que você recebe
    public List<QuestData> QuestsLosed = new List<QuestData>(); // Isso calcula o que você perde
    private JournalPageInfo JournalPage = new JournalPageInfo(); // Isso calcula a página inicial
    
    public void RegisterResult(bool playerWins, QuestData questScriptable)
    {
        if (playerWins)
        {
            if (questScriptable.Type == QuestType.Emergency)
            {
                QuestsWinned.Add(questScriptable);
                JournalPage.BigNew = questScriptable;
            }
            else if (questScriptable.Type == QuestType.Mistery)
            {
                QuestsWinned.Add(questScriptable);
                SmallNewReplace(questScriptable);
            }
        }
        else
        {
            if (questScriptable.Type == QuestType.Emergency)
            {
                QuestsLosed.Add(questScriptable);
                JournalPage.BigNew = questScriptable;
            }
            else if (questScriptable.Type == QuestType.Mistery)
            {
                QuestsLosed.Add(questScriptable);
                SmallNewReplace(questScriptable);
            }
        }
    }

    public void SmallNewReplace(QuestData questScriptable)
    {
        if (JournalPage.SmallNew == null) 
            JournalPage.SmallNew = questScriptable;
        else
            JournalPage.SmallNew = questScriptable;
    }
}
