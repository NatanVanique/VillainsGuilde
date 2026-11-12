using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Localization.Components;
using UnityEngine.Localization;

public class VisualReferencer : MonoBehaviour
{
    [Header("Room")]
    public Image colorImage_Room;
    
    [Header("Bars")]
    public GameObject top_Bars;
    public GameObject bottom_Bars;
    
    [System.Serializable]
    public class StatSlider
    {
        public CreateMonster_Data.StatsTypes statType;
        public Slider slider;
    }
    
    [Header("LookingForJob")]
    public Canvas lookingForJobCanvas_LookingForJob;
    
    [Header("ContractTolltip")]
    public Canvas contractTooltipCanvas_ContractTolltip;
    public Button infoButton_ContractTolltip;
    public Button statsButton_ContractTolltip;
    public StatSlider[] statsSlider_ContractTolltip;
    public Image bigImage_ContractTolltip;
    public Image iconImage__ContractTolltip;
    public LocalizeStringEvent  nameText_ContractTolltip;
    public LocalizeStringEvent descriptionText_ContractTolltip;
}
