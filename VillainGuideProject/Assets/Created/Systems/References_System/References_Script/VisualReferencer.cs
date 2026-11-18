using System.Collections.Generic;
using TriInspector;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Localization.Components;
using UnityEngine.Localization;
using TMPro;

[DeclareFoldoutGroup("Room", Expanded = true)]
[DeclareFoldoutGroup("Bars", Expanded = true)]
[DeclareFoldoutGroup("Voice Mail", Expanded = true)]
[DeclareFoldoutGroup("Monster Selection", Expanded = true)]
[DeclareFoldoutGroup("Looking For Job", Expanded = true)]
[DeclareFoldoutGroup("Contract Tooltip", Expanded = true)]
[DeclareFoldoutGroup("Telephone", Expanded = true)]
[DeclareFoldoutGroup("Journal", Expanded = true)]

public class VisualReferencer : MonoBehaviour
{
    [System.Serializable]
    public struct RoomScreenReference
    {
        public int position;
        public RectTransform screen;
    }

    [Group("Room")] public Image colorImage_Room;
    [Group("Room")] public Button previousScreenButton_Room;
    [Group("Room")] public Button nextScreenButton_Room;
    [Group("Room")] public List<RoomScreenReference> screensRoom = new List<RoomScreenReference>();

    [Group("Bars")] public GameObject top_Bars;
    [Group("Bars")] public GameObject bottom_Bars;

    [Group("Voice Mail")] public GameObject voiceMailCanvas_voiceMail;
    [Group("Voice Mail")] public GameObject questMail_VoiceMail;
    [Group("Voice Mail")] public GameObject questMailContainer_VoiceMail;
    [Group("Voice Mail")] public GameObject firstVision;
    [Group("Voice Mail")] public GameObject infoVision;
    [Group("Voice Mail")] public Transform requirementsContent;
    [Group("Voice Mail")] public Transform rewardsIconsContent;
    [Group("Voice Mail")] public Button returnInfoButton;
    [Group("Voice Mail")] public Button closeVoiceMailButton;
    [Group("Voice Mail")] public Image voiceMailBackground;
    [Group("Voice Mail")] public TMP_Text missionTitleText;
    [Group("Voice Mail")] public TMP_Text missionDescriptionText;
    [Group("Voice Mail")] public RectTransform goodDiscoveriesContent;
    [Group("Voice Mail")] public RectTransform badDiscoveriesContent;

    [Group("Monster Selection")] public GameObject selectionCanvas_MonsterSelection;
    [Group("Monster Selection")] public Image monsterImage_MonsterSelection;
    [Group("Monster Selection")] public TMP_Text monsterNameText_MonsterSelection;
    [Group("Monster Selection")] public TMP_Text specialAbilityText_MonsterSelection;
    [Group("Monster Selection")] public TMP_Text levelText_MonsterSelection;
    [Group("Monster Selection")] public Slider levelSlider_MonsterSelection;
    [Group("Monster Selection")] public Slider happinessSlider_MonsterSelection;
    [Group("Monster Selection")] public StatSlider[] statSliders_MonsterSelection;
    [Group("Monster Selection")] public Button previousButton_MonsterSelection;
    [Group("Monster Selection")] public Button nextButton_MonsterSelection;
    [Group("Monster Selection")] public Button sendButton_MonsterSelection;
    [Group("Monster Selection")] public TMP_Text sendButtonText_MonsterSelection;
    [Group("Monster Selection")] public Button backButton_MonsterSelection;

    [Group("Looking For Job")] public Canvas lookingForJobCanvas_LookingForJob;
    [Group("Looking For Job")] public GameObject contractCard_LookingForJob;
    [Group("Looking For Job")] public GameObject contractCardContainer_LookingForJob;

    [Group("Contract Tooltip")] public Canvas contractTooltipCanvas_ContractTolltip;
    [Group("Contract Tooltip")] public Button infoButton_ContractTolltip;
    [Group("Contract Tooltip")] public Button statsButton_ContractTolltip;
    [Group("Contract Tooltip")] public StatSlider[] statsSlider_ContractTolltip;
    [Group("Contract Tooltip")] public Image bigImage_ContractTolltip;
    [Group("Contract Tooltip")] public Image iconImage__ContractTolltip;
    [Group("Contract Tooltip")] public LocalizeStringEvent nameText_ContractTolltip;
    [Group("Contract Tooltip")] public LocalizeStringEvent descriptionText_ContractTolltip;

    [Group("Telephone")] public Animator telephoneAnimator_Telephone;
    [Group("Telephone")] public GameObject closedPhone;
    [Group("Telephone")] public GameObject openPhone;
    [Group("Telephone")] public GameObject[] closedPhoneOutlines;
    [Group("Telephone")] public GameObject openPhoneOutline;
    [Group("Telephone")] public Button phoneButton;

    [Group("Journal")] public GameObject singleNewContainer_Journal;
    [Group("Journal")] public GameObject[] singleNews_Journal;
    [Group("Journal")] public GameObject routeNewContainer_Journal;
    [Group("Journal")] public GameObject routeNew_Journal;
    [Group("Journal")] public LocalizeStringEvent titleText_Journal;
    [Group("Journal")] public Button tabButton_Journal;
    [Group("Journal")] public LocalizeStringEvent tabText_Journal;
    [Group("Journal")] public GameObject rewardsNew_Journal;
    [Group("Journal")] [ListDrawerSettings(ShowElementLabels = true)]
    public RewardComponenet[] rewardsNewComponents_Journal;


    [System.Serializable]
    [DeclareFoldoutGroup("References", Expanded = true)]
    public class RewardComponenet
    {
        public string displayName;
        public Slider progressSlider;
        public TextMeshProUGUI numbersValueText;
        public TextMeshProUGUI LevelValueText;
        public GameObject levelUpObject;
    }
    
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