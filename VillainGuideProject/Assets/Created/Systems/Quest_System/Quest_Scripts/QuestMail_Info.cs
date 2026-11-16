using UnityEngine;
using UnityEngine.Localization.Components;
using UnityEngine.Localization;
using UnityEngine.UI;
using UnityEngine.ResourceManagement.AsyncOperations;

public class QuestMail_Info : MonoBehaviour
{
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
        Main_Referencer.Instance.visual.infoVision.SetActive(isInfoOpen);
        Main_Referencer.Instance.visual.firstVision.SetActive(!isInfoOpen);
    }
}