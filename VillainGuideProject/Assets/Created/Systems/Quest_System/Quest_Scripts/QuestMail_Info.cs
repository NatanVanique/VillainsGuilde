using System;
using UnityEngine;
using UnityEngine.Localization.Components;
using UnityEngine.Localization;
using UnityEngine.UI;
using UnityEngine.ResourceManagement.AsyncOperations;
using DG.Tweening;

public class QuestMail_Info : MonoBehaviour
{
    [Header("Info Animation")]
    [SerializeField, Min(0f)] private float infoTweenDuration = 0.35f;
    [SerializeField] private Ease infoTweenEase = Ease.OutCubic;
    [SerializeField] private float mailDefaultY = 165.2499f;
    [SerializeField] private float mailDefaultHeight = 750f;
    [SerializeField] private float mailInfoY = -45.90763f;
    [SerializeField] private float mailInfoHeight = 1170f;

    [SerializeField] private LocalizeStringEvent mailName;
    [SerializeField] Button infoButton;
    [SerializeField] Button monstersButton;
    [SerializeField] private RewardIcon_Element rewardPrefab;
    [SerializeField] private Requirement_Element requirementPrefab;

    private CreateQuest_Data questData;
    private LocalizedString questName;
    private LocalizedString questDescription;
    private int infoRequestId;
    private Transform requirementContent;
    private Transform rewardContent;

    public void Start()
    {
        VisualReferencer visual = Main_Referencer.Instance.visual;
        requirementContent = visual.requirementsContent;
        rewardContent = visual.rewardsIconsContent;
    }

    public void setInfo(CreateQuest_Data data)
    {
        Main_Referencer.Instance.visual.returnInfoButton.onClick.AddListener(closeInfo);
        infoButton.onClick.AddListener(openInfo);
        monstersButton.onClick.AddListener(openMonsters);
        
        questData = data;
        questName = questData.questName;
        questDescription = questData.questDescription;
        mailName.StringReference = questData.questName;
        mailName.RefreshString();
    }

    public void openInfo()
    {
        SetInfoState(isInfoOpen: true);
        PopulateRequirementsAndRewards();
        RefreshMissionInfo();
    }

    private void PopulateRequirementsAndRewards()
    {
        ClearContent(requirementContent);
        ClearContent(rewardContent);

        if (questData.timeToDoneSeconds > 0f)
        {
            int totalSeconds = Mathf.CeilToInt(questData.timeToDoneSeconds);
            string time = $"{totalSeconds / 60:00}:{totalSeconds % 60:00}";
            Requirement_Element requirement = Instantiate(requirementPrefab, requirementContent);
            requirement.SetRequirement(Requirement_Element.RequirementType.TimeToDone, time);
        }

        if (questData.hasReputationXpReward)
        {
            AddReward(rewardContent, RewardIcon_Element.RewardType.ReputationXp, questData.reputationXpEarned.ToString());
        }

        if (questData.hasCharacterXpReward)
        {
            AddReward(rewardContent, RewardIcon_Element.RewardType.CharacterXp, questData.characterXpEarned.ToString());
        }

        if (questData.hasCoinsReward)
        {
            AddReward(rewardContent, RewardIcon_Element.RewardType.Coins, questData.coinsEarned.ToString());
        }

        if (questData.hasCharacterReward)
        {
            AddReward(
                rewardContent,
                RewardIcon_Element.RewardType.Character,
                questData.characterEarned.Length.ToString()
            );
        }

        LayoutRebuilder.ForceRebuildLayoutImmediate((RectTransform)rewardContent);
    }

    private void AddReward(Transform content, RewardIcon_Element.RewardType type, string text)
    {
        RewardIcon_Element reward = Instantiate(rewardPrefab, content);
        reward.SetReward(type, text);
    }

    private static void ClearContent(Transform content)
    {
        for (int i = content.childCount - 1; i >= 0; i--)
        {
            GameObject item = content.GetChild(i).gameObject;
            item.SetActive(false);
            Destroy(item);
        }
    }

    private void RefreshMissionInfo()
    {
        VisualReferencer visual = Main_Referencer.Instance.visual;
        int requestId = ++infoRequestId;
        AsyncOperationHandle<string> titleOperation = questName.GetLocalizedStringAsync();
        AsyncOperationHandle<string> descriptionOperation = questDescription.GetLocalizedStringAsync();

        titleOperation.Completed += operation =>
        {
            if (requestId == infoRequestId)
            {
                visual.missionTitleText.text = operation.Result;
            }
        };

        descriptionOperation.Completed += operation =>
        {
            if (requestId == infoRequestId)
            {
                visual.missionDescriptionText.text = operation.Result;
            }
        };
    }

    public void closeInfo()
    {
        SetInfoState(isInfoOpen: false);
    }

    public void openMonsters()
    {
        
    }
    
    public void SetInfoState(bool isInfoOpen)
    {
        VisualReferencer visual = Main_Referencer.Instance.visual;
        RectTransform background = visual.voiceMailBackground.rectTransform;

        if (isInfoOpen)
        {
            visual.infoVision.SetActive(true);
            visual.firstVision.SetActive(false);
        }
        else
        {
            visual.infoVision.SetActive(false);
            visual.firstVision.SetActive(true);
        }

        float targetY;
        float targetHeight;

        if (isInfoOpen)
        {
            targetY = mailInfoY;
            targetHeight = mailInfoHeight;
        }
        else
        {
            targetY = mailDefaultY;
            targetHeight = mailDefaultHeight;
        }

        DoTweenEffects_Commands.DoTweenAnchorPosYAndHeight(
            background, targetY, targetHeight, infoTweenDuration, infoTweenEase);
    }
}
