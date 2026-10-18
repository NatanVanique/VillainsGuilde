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

            bool result = ReturnIfCardSucceded(card, listOfMonsterInfo);
            QuestData data = card.ReturnQuestData();
            SingleEntryPoint.Instance.ManagerManager.JournalRegister.RegisterResult(result, data);
        }



    }

    bool ReturnIfCardSucceded(QuestCard card, List<MonsterInfo> listOfMonsterInfo)
    {
        int amountOfTipsSucceded = 0;

        foreach (var tip in card.ReturnListOfTips().SelectedTips.Values) // Para cada dica da carta
        {
            if (ReturnIfAnyMonsterSucceded(listOfMonsterInfo, tip)) amountOfTipsSucceded++; // Verifica se algum monstro possui um match
        }

        float percent = amountOfTipsSucceded / card.ReturnListOfTips().SelectedTips.Count * 100;

        int rng = Random.Range(0, 100);

        if (rng <= percent) return true;

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
