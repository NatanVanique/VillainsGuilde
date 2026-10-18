using UnityEngine;
using UnityEngine.UI;
using AYellowpaper.SerializedCollections;
using UnityEngine.Localization.Components;
using System.Collections.Generic;
using System;

public class QuestCard : MonoBehaviour
{
    #region Variables

    [Header("Components")]
    [SerializeField] Image questIcon;
    [SerializeField] LocalizeStringEvent title;
    [SerializeField] Button moreInfoButton;
    [SerializeField, SerializedDictionary("index", "text")] SerializedDictionary<int, LocalizeStringEvent> dictionaryOfTips;
    [SerializeField, SerializedDictionary("amount", "container")] SerializedDictionary<int, GameObject> dictionaryOfAmountOfCharacters;
    [SerializeField, SerializedDictionary("type", "icons")] SerializedDictionary<QuestRewardType, GameObject> dictionaryOfRewardIcons;


    [Header("Test")]
    [SerializeField] QuestData dataTest;

    public List<MonsterInfo> _listOfMonsters = new();
    QuestData _data;
    ListOfTips _listOfTips;

    public static event Action<QuestData, ListOfTips> OnQuestCardInfoPressed;

    #endregion

    #region Setup

    public void Setup(QuestData data) {
        _data = data;

        SetVisuals();
        SetButtons();
    }

    #endregion

    #region Buttons

    void SetButtons() {
        moreInfoButton.onClick.AddListener(MoreInfoButton);
    }

    void MoreInfoButton() {
        OnQuestCardInfoPressed?.Invoke(_data, _listOfTips);
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
        List<TipData> listOfTips = new();

        foreach (var pair in dictionaryOfTips) {
            if (!_data.DictionaryOfTipData.ContainsKey(pair.Key)) continue;

            List<TipData> listOfData = _data.DictionaryOfTipData[pair.Key];
            int randomTip = UnityEngine.Random.Range(0, listOfData.Count);
            TipData data = listOfData[randomTip];
            pair.Value.StringReference = data.TipText;

            listOfTips.Add(data);
        }

        _listOfTips = new(listOfTips);
    }

    void SetRewardsIcons() {
        dictionaryOfRewardIcons[QuestRewardType.Money].SetActive(_data.HasMoney);
        dictionaryOfRewardIcons[QuestRewardType.XP].SetActive(_data.HasXP);
        dictionaryOfRewardIcons[QuestRewardType.Character].SetActive(_data.HasCharacters);
    }

    #endregion

    #region Add Monster

    public void AddMonster(MonsterInfo info)
    {
        if (_listOfMonsters.Contains(info)) return;

        _listOfMonsters.Add(info);
    }

    public void RemoveMonster(MonsterInfo info)
    {
        if (!_listOfMonsters.Contains(info)) return;
        _listOfMonsters.Remove(info);
    }

    #endregion

    #region Getters

    public ListOfTips ReturnListOfTips() => _listOfTips;

    public QuestData ReturnQuestData() => _data;

    #endregion
}
