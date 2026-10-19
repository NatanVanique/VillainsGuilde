using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class QuestValidator : MonoBehaviour
{
    #region Variables

    public static event Action<int> OnQuestWithResultFailed;
    public static event Action OnRescueQuestWon;

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

        // Keep track of each offered occurrence, including repeated quest assets.
        QuestSelector selector = SingleEntryPoint.Instance.ManagerManager.QuestSelector;
        List<QuestData> notSelected = selector != null && selector.SelectedQuests != null
            ? new List<QuestData>(selector.SelectedQuests)
            : new List<QuestData>();

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
            notSelected.Remove(data);

            bool result = ReturnIfCardSucceded(data, listOfMonsterInfo);

            jr.RegisterResult(result, data, listOfMonsterInfo);
            CheckResult(result, data);
        }

        // Offered missions that were not selected still appear as failed news.
        foreach (QuestData quest in notSelected)
        {
            if (quest == null)
                continue;
            jr.RegisterResult(false, quest);
            CheckResult(false, quest);
        }
    }

    private void CheckResult(bool result, QuestData data) {

        if (data.Type == QuestType.Rescue && result == true) OnRescueQuestWon?.Invoke();

        if (!result) {
            if (!data.HasConsequence) return;

            OnQuestWithResultFailed?.Invoke(Mathf.Max(0, data.AmountOfConsequenceEvents));
        }

    }

    bool ReturnIfCardSucceded(QuestData quest, List<MonsterInfo> monsters)
    {
        float chance = CalculateWinChance(quest, monsters);
        return chance >= 100f || (chance > 0f && UnityEngine.Random.Range(0f, 100f) < chance);
    }

    internal static float CalculateWinChance(QuestData quest, List<MonsterInfo> monsters)
    {
        float baseChance = Mathf.Clamp(quest.BasePercentChanceOfWinning, 0f, 100f);
        List<TipType> tips = quest.TipData.Type;
        if (tips == null || tips.Count == 0 || monsters == null || monsters.Count == 0)
            return baseChance;

        int matchedTips = 0;
        foreach (TipType tip in tips)
        {
            foreach (MonsterInfo monster in monsters)
            {
                if (monster.TipList.SelectedTips == null)
                    continue;

                bool matched = false;
                foreach (var monsterTip in monster.TipList.SelectedTips)
                {
                    if (monsterTip.Value.Type != tip)
                        continue;
                    matched = true;
                    break;
                }

                if (matched)
                {
                    matchedTips++;
                    break;
                }
            }
        }

        return baseChance + (100f - baseChance) * matchedTips / tips.Count;
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
