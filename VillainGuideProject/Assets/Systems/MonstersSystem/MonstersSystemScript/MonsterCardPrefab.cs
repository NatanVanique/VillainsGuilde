using AYellowpaper.SerializedCollections;
using DG.Tweening;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization.Components;
using UnityEngine.UI;

public class MonsterCardPrefab : MonoBehaviour {

    #region Variables

    [Header("Components")]
    [SerializeField] Transform container;
    [SerializeField] Image splashArt;
    [SerializeField] Image Background;
    [SerializeField] LocalizeStringEvent nameText;
    [SerializeField] Button infoButton;

    [Header("Animation")]
    [SerializeField] float scaleDuration;
    [SerializeField] float scaleHoover = 1.1f;
    [SerializeField] float scaleHooverDuration = 0.15f;

    [Header("Selection")]
    [SerializeField] Color selectedBackgroundColor;
    [SerializeField] Color originalBackgroundColor;

    [Header("Atributes")]
    public bool HasBeenUsed;

    ListOfTips _selectedMonsterTips;
    MonsterData _selectedData;

    public static event Action<MonsterData, ListOfTips> OnMonsterCardInfoButtonPressed;
    public static event Action<MonsterCardPrefab> OnCardSelected;

    Tween _scaleTween;

    bool _isSelected;
    bool _canSelect = false;

    #endregion

    #region Setup
    public void Setup(MonsterData data) {

        SubscribeToEvents();

        _selectedData = data;
        _selectedMonsterTips = ChooseRandomTips();
        _canSelect = false;

        SetVisual();

        SetButtons();
        
        Animate();
    }

    #endregion

    #region Handle Events

    void SubscribeToEvents() {
        OnCardSelected += HandleCardSelected;
    }

    void HandleCardSelected(MonsterCardPrefab cardSelected) {
        if (cardSelected != this) DeselectCard();
    }

    public void SelectCard() {
        if (_isSelected || !_canSelect) return;

        _isSelected = true;

        OnCardSelected?.Invoke(this);

        Background.color = selectedBackgroundColor;
    }

    void DeselectCard() {
        if (!_isSelected) return;

        _isSelected = false;

        Background.color = originalBackgroundColor;
    }

    public void OnPointerEnter() {
        if (!_canSelect) return;

        _scaleTween = container.DOScale(scaleHoover, scaleHooverDuration);

    }

    public void OnPointerExit() {
        if (!_canSelect) return;

        _scaleTween = container.DOScale(1, scaleHooverDuration);
    }

    #endregion

    #region Buttons

    void SetButtons() {
        infoButton.onClick.AddListener(InfoButton);
    }

    void InfoButton() {
        OnMonsterCardInfoButtonPressed?.Invoke(_selectedData, _selectedMonsterTips);
    }

    #endregion

    #region Main Logic
    ListOfTips ChooseRandomTips() {

        List<TipData> listOfTips = new();

        foreach (var pair in _selectedData.DictionaryOfTips) {
            List<TipData> listOsTips = pair.Value;
            int randomIndex = UnityEngine.Random.Range(0, listOsTips.Count);
            TipData newTip = listOsTips[randomIndex];
            switch (pair.Key) {
                case 1:
                    listOfTips.Insert(0, newTip);
                    break;
                case 2:
                    listOfTips.Insert(1, newTip);
                    break;
                case 3:
                    listOfTips.Insert(2, newTip);
                    break;
            }
        }

        ListOfTips monsterTips = new(listOfTips);

        return monsterTips;
    }

    #endregion

    #region Visuals

    void Animate() {
        _scaleTween = container.DOScale(1, scaleDuration).OnComplete(() => {
            FinishScale();
        });
    }

    private void FinishScale() {
        _canSelect = true;
    }

    void SetVisual() {
        if (_selectedData.SplashArt != null) splashArt.sprite = _selectedData.SplashArt;
        if (_selectedData.Name != null) nameText.StringReference = _selectedData.Name;
        if (_selectedData.Background != null) Background.sprite = _selectedData.Background;

        Background.color = originalBackgroundColor;
    }

    #endregion

    #region Select
    public MonsterInfo SelectThisMonter() {
        return new MonsterInfo(_selectedData, _selectedMonsterTips);
    }

    #endregion

    #region End

    private void OnDestroy() {
        KillTweens();
        UnsubscribeToEvents();
    }

    void UnsubscribeToEvents() {
        OnCardSelected -= HandleCardSelected;
    }

    void KillTweens() {
        DOTween.Kill(_scaleTween);
    }

    #endregion
}

[Serializable]
public struct ListOfTips {
    public SerializedDictionary<int, TipData> SelectedTips;

    public ListOfTips(List<TipData> listOfTips) {
        SelectedTips = new() {
            [1] = listOfTips[0],
            [2] = listOfTips[1],
            [3] = listOfTips[2]
        };
    }
}

[Serializable]
public struct MonsterInfo {
    public MonsterData SelectedData;
    public ListOfTips TipList;

    public MonsterInfo(MonsterData selectedData, ListOfTips tipList) {
        SelectedData = selectedData;
        TipList = tipList;
    }
}
