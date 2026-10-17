using UnityEngine;

public class JournalReward : MonoBehaviour
{
    private JournalRegister rewardsRegistred;

        using UnityEngine;

    public class JournalReward : MonoBehaviour
    {
        [SerializeField] private JournalRegister rewardsRegistred;

        private int totalXP;
        private int totalMoney;
        private int totalCharacters;

        private void CalculeRewards()
        {
            totalXp == rewardsRegistred.QuestsWinned.xp - rewardsRegistred.QuestsLosed.xp;
            totalCoins == rewardsRegistred.QuestsWinned.coins - rewardsRegistred.QuestsLosed.coins;
            finalChars == rewardsRegistred.QuestsWinned.chars - rewardsRegistred.QuestsLosed.chars;
        }
    }
}
