using TriInspector;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Localization.Components;
using UnityEngine.Localization;

[DeclareFoldoutGroup("Room", Expanded = true)]
[DeclareFoldoutGroup("Bars", Expanded = true)]
[DeclareFoldoutGroup("Voice Mail", Expanded = true)]
[DeclareFoldoutGroup("Looking For Job", Expanded = true)]
[DeclareFoldoutGroup("Contract Tooltip", Expanded = true)]
public class VisualReferencer : MonoBehaviour
{
    [Group("Room")]
    public Image colorImage_Room;
    
    [Group("Bars")]
    public GameObject top_Bars;

    [Group("Bars")]
    public GameObject bottom_Bars;
    
    [Group("Voice Mail")]
    public Canvas voiceMailCanvas_voiceMail;

    [Group("Voice Mail")]
    public GameObject questMail_VoiceMail;

    [Group("Voice Mail")]
    public GameObject questMailContainer_VoiceMail;
    
    [Group("Looking For Job")]
    public Canvas lookingForJobCanvas_LookingForJob;

    [Group("Looking For Job")]
    public GameObject contractCard_LookingForJob;

    [Group("Looking For Job")]
    public GameObject contractCardContainer_LookingForJob;
    
    [Group("Contract Tooltip")]
    public Canvas contractTooltipCanvas_ContractTolltip;

    [Group("Contract Tooltip")]
    public Button infoButton_ContractTolltip;

    [Group("Contract Tooltip")]
    public Button statsButton_ContractTolltip;

    [Group("Contract Tooltip")]
    public StatSlider[] statsSlider_ContractTolltip;

    [Group("Contract Tooltip")]
    public Image bigImage_ContractTolltip;

    [Group("Contract Tooltip")]
    public Image iconImage__ContractTolltip;

    [Group("Contract Tooltip")]
    public LocalizeStringEvent  nameText_ContractTolltip;

    [Group("Contract Tooltip")]
    public LocalizeStringEvent descriptionText_ContractTolltip;
    
    [System.Serializable]
    [DeclareBoxGroup("References", Title = "Referências")]
    public class StatSlider
    {
        [Group("References")]
        public CreateMonster_Data.StatsTypes statType;

        [Group("References")]
        public Slider slider;
    }
}
