using UnityEngine;
using UnityEngine.UI;
using AYellowpaper.SerializedCollections;
using UnityEngine.Localization.Components;
using System.Collections.Generic;

public class QuestCard : MonoBehaviour
{
    #region Variables

    [Header("Components")]
    [SerializeField] Image questIcon;
    [SerializeField] LocalizeStringEvent title;
    [SerializeField, SerializedDictionary("index", "text")] SerializedDictionary<int, LocalizeStringEvent> dictionaryOfTips;
    [SerializeField, SerializedDictionary("amount", "container")] SerializedDictionary<int, GameObject> dictionaryOfAmountOfCharacters;
    [SerializeField, SerializedDictionary("type", "icons")] SerializedDictionary<QuestRewardType, GameObject> dictionaryOfRewardIcons;

    [Header("Test")]
    [SerializeField] QuestData dataTest;

    QuestData _data;

    #endregion

    #region Setup

    private void Start() {
        Setup(dataTest);
    }

    public void Setup(QuestData data) {
        _data = data;

        SetVisuals();
    }

    #endregion

    #region Visuals

    void SetVisuals() {
        ChangeTitle();
        ChangeIcon();
        ChangeCharactersAmountIcons();
        ChangeTips();
        SetRewardsIcons();
    }

    void ChangeTitle() {
        title.StringReference = _data.QuestTitle;
    }

    void ChangeIcon() {
        questIcon.sprite = _data.QuestIcon;
    }

    void ChangeCharactersAmountIcons() {
        foreach(var pair in dictionaryOfAmountOfCharacters) {
            if (pair.Key == _data.AmountOfMaxCharacters) pair.Value.SetActive(true);
            else pair.Value.SetActive(false);
        }
    }

    void ChangeTips() {
        foreach (var pair in dictionaryOfTips) {
            if (!_data.DictionaryOfTipData.ContainsKey(pair.Key)) continue;

            List<TipData> listOfData = _data.DictionaryOfTipData[pair.Key];
            int randomTip = Random.Range(0, listOfData.Count);
            pair.Value.StringReference = listOfData[randomTip].TipText;
        }
    }

    void SetRewardsIcons() {
        dictionaryOfRewardIcons[QuestRewardType.Money].SetActive(_data.HasMoney);
        dictionaryOfRewardIcons[QuestRewardType.XP].SetActive(_data.HasXP);
        dictionaryOfRewardIcons[QuestRewardType.Character].SetActive(_data.HasCharacters);
    }

    #endregion
}
