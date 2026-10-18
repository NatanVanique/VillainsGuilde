using AYellowpaper.SerializedCollections;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Localization.Components;
using UnityEngine.UI;

public class QuestCard : MonoBehaviour {
    #region Variables

    [Header("Components")]
    [SerializeField] LocalizeStringEvent title;
    [SerializeField] LocalizeStringEvent tipText;
    [SerializeField, SerializedDictionary("amount", "container")] SerializedDictionary<int, GameObject> dictionaryOfAmountOfCharacters;
    [SerializeField, SerializedDictionary("type", "icons")] SerializedDictionary<QuestRewardType, GameObject> dictionaryOfRewardIcons;


    [Header("Test")]
    [SerializeField] QuestData dataTest;

    public List<MonsterInfo> _listOfMonsters = new();
    QuestData _data;

    #endregion

    #region Setup

    public void Setup(QuestData data) {
        _data = data;

        SetVisuals();
    }

    #endregion

    #region Visuals

    void SetVisuals() {
        ChangeTitle();
        ChangeCharactersAmountIcons();
        ChangeTipText();
        SetRewardsIcons();
    }

    void ChangeTitle() {
        title.StringReference = _data.QuestTitle;
    }

    void ChangeCharactersAmountIcons() {
        foreach (var pair in dictionaryOfAmountOfCharacters) {
            if (pair.Key == _data.AmountOfMaxCharacters) pair.Value.SetActive(true);
            else pair.Value.SetActive(false);
        }
    }

    void ChangeTipText() {
        tipText.StringReference = _data.TipData.TipText;
    }

    void SetRewardsIcons() {
        dictionaryOfRewardIcons[QuestRewardType.Money].SetActive(_data.HasMoney);
        dictionaryOfRewardIcons[QuestRewardType.XP].SetActive(_data.HasXP);
        dictionaryOfRewardIcons[QuestRewardType.Character].SetActive(_data.HasCharacters);
    }

    #endregion

    #region Add Monster

    public void AddMonster(MonsterInfo info) {
        if (_listOfMonsters.Contains(info)) return;

        _listOfMonsters.Add(info);
    }

    public void RemoveMonster(MonsterInfo info) {
        if (!_listOfMonsters.Contains(info)) return;
        _listOfMonsters.Remove(info);
    }

    #endregion

    #region Getters

    public QuestData ReturnQuestData() => _data;

    #endregion
}
