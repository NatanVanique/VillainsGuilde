using UnityEngine;
using UnityEngine.Localization.Components;
using UnityEngine.UI;

public class QuestMail_Info : MonoBehaviour
{
    [SerializeField] private LocalizeStringEvent mailName;
    [SerializeField] Button infoButton;
    [SerializeField] Button monstersButton;

    public void setInfo(CreateQuest_Data data)
    {
        Main_Referencer.Instance.visual.returnInfoButton.onClick.AddListener(closeInfo);
        infoButton.onClick.AddListener(openInfo);
        monstersButton.onClick.AddListener(openMonsters);
        
        mailName.StringReference = data.questName;
        mailName.RefreshString();
    }

    public void openInfo()
    {
        SetInfoState(isInfoOpen: true);
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