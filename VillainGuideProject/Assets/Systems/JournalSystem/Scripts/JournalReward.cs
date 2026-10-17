using System;
using UnityEngine;

public class JournalReward : MonoBehaviour
{
    private JournalRegister rewardsRegister;

    private void Awake()
    {
        Setup();
    }

    public struct RewardResult
    {
        public int Characters;
        public float Money;
        public float XP;

        public RewardResult(int characters, float money, float xp)
        {
            Characters = characters;
            Money = money;
            XP = xp;
        }
    }

    public float TotalXP { get; private set; }
    public float TotalMoney { get; private set; }
    public int TotalCharacters { get; private set; }

    public void Setup()
    {
        rewardsRegister = gameObject.GetComponent<JournalRegister>();
    }

    public void CollectRewards()
    {
    }

    public RewardResult CalculateRewards()
    {
        TotalMoney = 0f;
        TotalXP = 0f;
        TotalCharacters = 0;

        if (rewardsRegister == null)
            return new RewardResult();

        AddRewards(rewardsRegister.QuestsWinned, 1);
        AddRewards(rewardsRegister.QuestsLosed, -1);

        return new RewardResult(TotalCharacters,TotalMoney,TotalXP);
    }

    private void AddRewards(System.Collections.Generic.List<QuestData> quests, int multiplier)
    {
        if (quests == null)
            return;

        foreach (QuestData quest in quests)
        {
            if (quest == null)
                continue;

            if (quest.HasXP)
                TotalXP += quest.XPReward * multiplier;

            if (quest.HasMoney)
                TotalMoney += quest.MoneyReward * multiplier;

            if (quest.HasCharacters && quest.ListOfCharacters != null)
                TotalCharacters += quest.ListOfCharacters.Count * multiplier;
        }
    }
}
