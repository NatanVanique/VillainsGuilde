using UnityEngine;

public class JournalReward : MonoBehaviour
{
    [SerializeField] private JournalRegister rewardsRegister;

    public float TotalXP { get; private set; }
    public float TotalMoney { get; private set; }
    public int TotalCharacters { get; private set; }

    private void Awake()
    {
        CalculateRewards();
    }

    public void CalculateRewards()
    {
        TotalXP = 0f;
        TotalMoney = 0f;
        TotalCharacters = 0;

        if (rewardsRegister == null)
            return;

        AddRewards(rewardsRegister.QuestsWinned, 1);
        AddRewards(rewardsRegister.QuestsLosed, -1);
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
