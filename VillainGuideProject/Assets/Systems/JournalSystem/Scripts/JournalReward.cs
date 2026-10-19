using UnityEngine;

public class JournalReward : MonoBehaviour
{
    private JournalRegister rewardsRegister;

    public struct RewardResult
    {
        public int Characters;
        public float Money;
        public float XP;
        public bool HasCharacters;
        public bool HasMoney;
        public bool HasXP;

        public RewardResult(int characters, float money, float xp,
            bool hasCharacters, bool hasMoney, bool hasXP)
        {
            Characters = characters;
            Money = money;
            XP = xp;
            HasCharacters = hasCharacters;
            HasMoney = hasMoney;
            HasXP = hasXP;
        }
    }

    public float TotalXP { get; private set; }
    public float TotalMoney { get; private set; }
    public int TotalCharacters { get; private set; }
    private bool hasCharacters;
    private bool hasMoney;
    private bool hasXP;

    public void Setup()
    {
        if (SingleEntryPoint.Instance != null && SingleEntryPoint.Instance.ManagerManager != null)
            rewardsRegister = SingleEntryPoint.Instance.ManagerManager.JournalRegister;
    }

    public void CollectRewards()
    {
        RewardResult result = CalculateRewards();

        if (result.Characters != 0)
            UpdateEmployees(result.Characters);
    }

    public RewardResult CalculateRewards()
    {
        TotalMoney = 0f;
        TotalXP = 0f;
        TotalCharacters = 0;
        hasCharacters = false;
        hasMoney = false;
        hasXP = false;

        if (rewardsRegister == null)
            Setup();

        if (rewardsRegister == null)
            return new RewardResult(0, 0f, 0f, false, false, false);

        AddRewards(rewardsRegister.QuestsWinned);

        return new RewardResult(TotalCharacters, TotalMoney, TotalXP, hasCharacters, hasMoney, hasXP);
    }

    private void AddRewards(System.Collections.Generic.List<QuestData> quests)
    {
        if (quests == null)
            return;

        foreach (QuestData quest in quests)
        {
            if (quest == null)
                continue;

            if (quest.HasXP)
            {
                TotalXP += quest.XPReward;
                hasXP |= quest.XPReward != 0f;
            }

            if (quest.HasMoney)
            {
                TotalMoney += quest.MoneyReward;
                hasMoney |= quest.MoneyReward != 0f;
            }

            if (quest.HasCharacters && quest.ListOfCharacters != null)
            {
                TotalCharacters += quest.ListOfCharacters.Count;
                hasCharacters |= quest.ListOfCharacters.Count > 0;
            }
        }
    }

    public void UpdateEmployees(int characterChange)
    {
        // Será implementado quando o sistema de funcionários estiver definido.
    }
}
