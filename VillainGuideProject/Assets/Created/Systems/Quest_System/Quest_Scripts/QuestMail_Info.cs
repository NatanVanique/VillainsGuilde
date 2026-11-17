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
    private const float MailDefaultHeight = 749.5f;
    private const float MailInfoY = 55.1375f;
    private const float MailInfoHeight = 969.7247f;

    [SerializeField] private LocalizeStringEvent mailName;
    [SerializeField] Button infoButton;
    [SerializeField] Button monstersButton;

    private LocalizedString questName;
    private LocalizedString questDescription;
    private int infoRequestId;

    public void setInfo(CreateQuest_Data data)
    {
        Main_Referencer.Instance.visual.returnInfoButton.onClick.AddListener(closeInfo);
        infoButton.onClick.AddListener(openInfo);
        monstersButton.onClick.AddListener(openMonsters);
        
        questName = data.questName;
        questDescription = data.questDescription;
        mailName.StringReference = data.questName;
        mailName.RefreshString();
    }

    public void openInfo()
    {
        RefreshMissionInfo();
        SetInfoState(isInfoOpen: true);
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

        float targetY = isInfoOpen ? MailInfoY : MailDefaultY;
        float targetHeight = isInfoOpen ? MailInfoHeight : MailDefaultHeight;
        background.DOAnchorPosY(targetY, InfoTweenDuration).SetEase(Ease.OutCubic);
        background.DOSizeDelta(new Vector2(background.sizeDelta.x, targetHeight), InfoTweenDuration)
            .SetEase(Ease.OutCubic);
    }
}
