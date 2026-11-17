using TMPro;
using UnityEngine;
using UnityEngine.UI;

public sealed class RewardIcon_Element : MonoBehaviour
{
    [SerializeField] private Image iconImage;
    [SerializeField] private TMP_Text rewardText;
    [SerializeField] private Sprite reputationXpIcon;
    [SerializeField] private Sprite characterXpIcon;
    [SerializeField] private Sprite coinsIcon;
    [SerializeField] private Sprite characterIcon;

    public enum RewardType
    {
        ReputationXp,
        CharacterXp,
        Coins,
        Character
    }

    public void SetReward(RewardType type, string text)
    {
        switch (type)
        {
            case RewardType.ReputationXp:
                iconImage.sprite = reputationXpIcon;
                break;
            case RewardType.CharacterXp:
                iconImage.sprite = characterXpIcon;
                break;
            case RewardType.Coins:
                iconImage.sprite = coinsIcon;
                break;
            case RewardType.Character:
                iconImage.sprite = characterIcon;
                break;
        }

        rewardText.text = text;
    }
}
