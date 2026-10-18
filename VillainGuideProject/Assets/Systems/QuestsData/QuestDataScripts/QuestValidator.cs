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
        MissionsScreenController.OnQuestsAccepted += HandleQuestsSelected;
    }

    void HandleQuestsSelected(List<QuestCardSelection> selectedQuests)
    {
        foreach (var cardSelection in selectedQuests) // Para cada carta
        {
            QuestCard card = cardSelection.QuestCard;
            List<MonsterInfo> listOfMonsterInfo = card._listOfMonsters;
            QuestData data = card.ReturnQuestData();
            JournalRegister jr = SingleEntryPoint.Instance.ManagerManager.JournalRegister;

            bool result = ReturnIfCardSucceded(card, listOfMonsterInfo);
            Debug.Log("Quest processed, result: " + result);
            jr.RegisterResult(result, data);
        }
    }

    bool ReturnIfCardSucceded(QuestCard card, List<MonsterInfo> listOfMonsterInfo)
    {
        int amountOfTipsSucceded = 0;

        foreach (var tip in card.ReturnListOfTips().SelectedTips.Values) // Para cada dica da carta
        {
            if (ReturnIfAnyMonsterSucceded(listOfMonsterInfo, tip)) amountOfTipsSucceded++; // Verifica se algum monstro possui um match
        }

        float percent = amountOfTipsSucceded / card.ReturnListOfTips().SelectedTips.Count;
        float basePercent = card.ReturnQuestData().BasePercentChanceOfWinning;
        float restPercent = 100 - basePercent;

        float realPercent = (percent * restPercent) + basePercent;

        int rng = Random.Range(0, 100);

        if (rng <= realPercent) return true;

        return false;
    }

    bool ReturnIfAnyMonsterSucceded(List<MonsterInfo> listOfMonsterInfo, TipData tip)
    {
        foreach (var monsterInfo in listOfMonsterInfo) // Para cada monstro
        {
            if(ReturnIfMonsterSucceded(monsterInfo, tip)) return true; // Verifica se aquele monstro deu match
        }

        return false;
    }

    bool ReturnIfMonsterSucceded(MonsterInfo monster, TipData tip)
    {
        foreach (var monsterTip in monster.TipList.SelectedTips) // Para cada dica do monstro
        {
            if (monsterTip.Value.Type == tip.Type) return true; // Verifica se deu match
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
