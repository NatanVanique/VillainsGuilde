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
    
    private List<QuestData> QuestsDone; // Isso calcula o que vocÊ recebe
    [SerializeField] private JournalPageInfo[] JournalPage; //Isso calcula a página inicial
    
    public void RegisterResult(bool playerWins, QuestData questScriptable)
    {
        if (playerWins)
        {
            if (questScriptable.QuestType == Emergency)
            {
                dailyNews.Add(questScriptable);
            }
            else if (questScriptable.QuestType == Mistery)
            {
                
            }
        }
        else
        {
            if (questScriptable.QuestType != Classic)
            {
                dailyNews.Add(questScriptable);
            }
        }
    }
}
