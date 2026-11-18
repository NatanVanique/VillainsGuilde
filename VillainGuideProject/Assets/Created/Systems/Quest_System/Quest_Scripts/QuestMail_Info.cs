using System;
using UnityEngine;
using UnityEngine.Localization.Components;
using UnityEngine.Localization;
using UnityEngine.UI;
using UnityEngine.ResourceManagement.AsyncOperations;
using DG.Tweening;

public class QuestMail_Info : MonoBehaviour
{
    private const float InfoTweenDuration = 0.35f;
    private const float MailDefaultY = 165.2499f;
    private const float MailDefaultHeight = 750f;
    private const float MailInfoY = -45.90763f;
    private const float MailInfoHeight = 1170f;

    [SerializeField] private LocalizeStringEvent mailName;
    [SerializeField] Button infoButton;
    [SerializeField] Button monstersButton;
    [SerializeField] private Button acceptButton;
    [SerializeField] private Button declineButton;
    [SerializeField] private Image timerBar;
    [SerializeField] private Gradient timerBarGradient;
    [SerializeField] private RewardIcon_Element rewardPrefab;
    [SerializeField] private Requirement_Element requirementPrefab;

    private CreateQuest_Data questData;
    private LocalizedString questName;
    private LocalizedString questDescription;
    private int infoRequestId;
    private Transform requirementContent;
    private Transform rewardContent;
    private bool resolved;
    private bool viewingInfo;
    private float responseDeadline;

    public CreateMonster_Data AssignedMonster { get; private set; }

    public void Start()
    {
        VisualReferencer visual = Main_Referencer.Instance.visual;
        requirementContent = visual.requirementsContent;
        rewardContent = visual.rewardsIconsContent;
    }

    public void setInfo(CreateQuest_Data data)
    {
        infoButton.onClick.AddListener(openInfo);
        monstersButton.onClick.AddListener(openMonsters);
        acceptButton.onClick.AddListener(AcceptQuest);
        declineButton.onClick.AddListener(DeclineQuest);
        acceptButton.interactable = false;
        
        questData = data;
        questName = questData.questName;
        questDescription = questData.questDescription;
        mailName.StringReference = questData.questName;
        mailName.RefreshString();
        responseDeadline = Time.time + data.timeToAcceptSeconds;
        Main_Referencer.Instance.manager.questManager.StartResponseTimer(this);
    }

    internal bool UpdateResponseTimer()
    {
        if (resolved)
        {
            return false;
        }

        float remaining = responseDeadline - Time.time;
        float fraction = remaining > 0f ? Mathf.Clamp01(remaining / questData.timeToAcceptSeconds) : 0f;
        RectTransform barTransform = timerBar.rectTransform;
        barTransform.anchorMax = new Vector2(fraction, barTransform.anchorMax.y);
        timerBar.color = timerBarGradient.Evaluate(1f - fraction);

        if (remaining <= 0f)
        {
            DeclineQuest();
            return false;
        }

        return true;
    }

    public void openInfo()
    {
        viewingInfo = true;
        Main_Referencer.Instance.visual.returnInfoButton.onClick.AddListener(closeInfo);
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

        if (questData.haspopularityXpReward)
        {
            AddReward(rewardContent, RewardIcon_Element.RewardType.ReputationXp, questData.popularityXpEarned.ToString());
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
        viewingInfo = false;
        Main_Referencer.Instance.visual.returnInfoButton.onClick.RemoveListener(closeInfo);
        infoRequestId++;
        SetInfoState(isInfoOpen: false);
    }

    public void openMonsters()
    {
        Main_Referencer.Instance.manager.monsterManager.OpenSelection(this);
    }

    public void AssignMonster(CreateMonster_Data monster)
    {
        AssignedMonster = monster;
        acceptButton.interactable = true;
        Image icon = monstersButton.image;
        icon.sprite = monster.icon;
        icon.color = Color.white;
        icon.preserveAspect = true;
    }

    private void AcceptQuest()
    {
        if (AssignedMonster != null)
        {
            ResolveQuest(accepted: true);
        }
    }

    private void DeclineQuest()
    {
        ResolveQuest(accepted: false);
    }

    private void ResolveQuest(bool accepted)
    {
        if (resolved)
        {
            return;
        }

        resolved = true;
        Main_Referencer references = Main_Referencer.Instance;
        if (viewingInfo)
        {
            closeInfo();
        }
        CreateMonster_Data monster = AssignedMonster;
        AssignedMonster = null;
        gameObject.SetActive(false);
        Destroy(gameObject);
        references.manager.monsterManager.OnQuestMailResolved(this);
        references.manager.questManager.ResolveQuest(questData, monster, accepted && Time.time < responseDeadline);
    }
    
    public void SetInfoState(bool isInfoOpen)
    {
        VisualReferencer visual = Main_Referencer.Instance.visual;
        RectTransform background = visual.voiceMailBackground.rectTransform;
        DOTween.Kill(background);

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
            targetY = MailInfoY;
            targetHeight = MailInfoHeight;
        }
        else
        {
            targetY = MailDefaultY;
            targetHeight = MailDefaultHeight;
        }

        background.DOAnchorPosY(targetY, InfoTweenDuration).SetEase(Ease.OutCubic);
        background.DOSizeDelta(new Vector2(background.sizeDelta.x, targetHeight), InfoTweenDuration)
            .SetEase(Ease.OutCubic);
    }
}
