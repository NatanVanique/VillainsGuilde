using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class QuestValidator : MonoBehaviour
{
    #region Setup

    public void Setup()
    {
        SubscribeToEvents();
    }

    #endregion

    #region HandleEvents

    void SubscribeToEvents()
    {
        MissionsScreenController.OnQuestsAccepted -= HandleQuestsSelected;
        MissionsScreenController.OnQuestsAccepted += HandleQuestsSelected;
    }

    void HandleQuestsSelected(List<QuestCardSelection> selectedQuests)
    {
        foreach (var cardSelection in selectedQuests) // Para cada carta
        {
            if (cardSelection == null || cardSelection.QuestCard == null || cardSelection.QuestCard.ReturnQuestData() == null)
            {
                Debug.LogWarning("A selected mission has no quest data and could not be registered.", this);
                continue;
            }
            QuestCard card = cardSelection.QuestCard;
            List<MonsterInfo> listOfMonsterInfo = card._listOfMonsters;
            QuestData data = card.ReturnQuestData();
            JournalRegister jr = SingleEntryPoint.Instance.ManagerManager.JournalRegister;

            bool result = ReturnIfCardSucceded(card, listOfMonsterInfo);
            Debug.Log("Quest processed, result: " + result);
            jr.RegisterResult(result, data, listOfMonsterInfo);
        }
    }

    bool ReturnIfCardSucceded(QuestCard card, List<MonsterInfo> listOfMonsterInfo)
    {
        int amountOfTipsSucceded = 0;

        List<TipType> tips = card.ReturnQuestData().TipData.Type;
        if (tips == null)
            tips = new List<TipType>();
        foreach (var tip in tips) // Para cada dica da carta
        {
            if (ReturnIfAnyMonsterSucceded(listOfMonsterInfo, tip)) amountOfTipsSucceded++; // Verifica se algum monstro possui um match
        }

        int amountOfTips = tips.Count;
        float percent = amountOfTips > 0
            ? (float)amountOfTipsSucceded / amountOfTips
            : 0f;
        float basePercent = card.ReturnQuestData().BasePercentChanceOfWinning;
        float restPercent = 100 - basePercent;

        float realPercent = (percent * restPercent) + basePercent;

        int rng = Random.Range(0, 100);

        if (rng <= realPercent) return true;

        return false;
    }

    bool ReturnIfAnyMonsterSucceded(List<MonsterInfo> listOfMonsterInfo, TipType tip)
    {
        foreach (var monsterInfo in listOfMonsterInfo) // Para cada monstro
        {
            if(ReturnIfMonsterSucceded(monsterInfo, tip)) return true; // Verifica se aquele monstro deu match
        }

        return false;
    }

    bool ReturnIfMonsterSucceded(MonsterInfo monster, TipType tip)
    {
        if (monster.TipList.SelectedTips == null)
            return false;
        foreach (var monsterTip in monster.TipList.SelectedTips) // Para cada dica do monstro
        {
            if (monsterTip.Value.Type == tip) return true; // Verifica se deu match
        }

        return false;
    }
    #endregion

    #region End

    private void OnDestroy()
    {
        UnsubscribeToEvents();
    }

    void UnsubscribeToEvents()
    {
        MissionsScreenController.OnQuestsAccepted -= HandleQuestsSelected;
    }

    #endregion
}
