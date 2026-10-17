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
    
    public List<QuestData> QuestsWinned; // Isso calcula o que vocÊ recebe
    public List<QuestData> QuestsLosed; // Isso calcula o que vocÊ perde
    private JournalPageInfo JournalPage; //Isso calcula a página inicial
    
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
        else if (//(questScriptable.xp + questScriptable.money) >=(JournalPage.SmallNew.xp + JournalPage.SmallNew.money))
            JournalPage.SmallNew = questScriptable;
    }
}
