using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class QuestValidator : MonoBehaviour
{
    #region Variables

    public static event Action<int> OnQuestWithResultFailed;

    #endregion

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
        JournalRegister jr = SingleEntryPoint.Instance?.ManagerManager?.JournalRegister;
        if (jr == null)
        {
            Debug.LogError("Cannot register mission results: JournalRegister is missing.", this);
            return;
        }

        if (selectedQuests == null)
            return;

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

            bool result = ReturnIfCardSucceded(card);

            jr.RegisterResult(result, data, listOfMonsterInfo);
            CheckResult(result, data);
        }
    }

    private void CheckResult(bool result, QuestData data) {
        if (!result) {
            if (!data.HasConsequence) return;

            int amountOfBadEvents = data.AmountOfConsequenceEvents;

            OnQuestWithResultFailed?.Invoke(amountOfBadEvents);
        }

    }

    bool ReturnIfCardSucceded(QuestCard card)
    {
        float chance = Mathf.Clamp(card.ReturnQuestData().BasePercentChanceOfWinning, 0f, 100f);
        return UnityEngine.Random.Range(0f, 100f) < chance;
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
