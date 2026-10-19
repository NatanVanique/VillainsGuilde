using DG.Tweening;
using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class MonsterUiDisplay : MonoBehaviour {
    #region Variables

    [Header("Components")]
    [SerializeField] GameObject container;
    [SerializeField] CanvasGroup mask;
    [SerializeField] Button confirmButton;

    [Header("Prefabs")]
    [SerializeField] MonsterCardPrefab monsterCardPrefab;

    [Header("Animation")]
    [SerializeField] float scaleDuration;
    [SerializeField] float fadeDuration;
    [SerializeField] float fadeFinalValue;

    [Header("Instantiate Cards")]
    [SerializeField] float instantiateInterval;

    [Header("Monsters")]
    [SerializeField] List<MonsterData> listOfAllMonsters = new();

    // private
    [HideInInspector] public List<MonsterCardPrefab> _listOfExistingCards = new();
    [HideInInspector] public MonsterCardPrefab _selectedCard;

    Tween _scaleTween, _fadeTween, _confirmButtonTween;

    public bool _canTurnScreenOn = false;

    public static event Action<MonsterCardPrefab> OnMonsterCardSelected;
    #endregion

    #region Setup

    private void Awake() {
        Setup();
    }

    public void Setup() {
        SubscribeToEvents();
        SetButtons();
    }

    #endregion

    #region Buttons

    void SetButtons() {
        confirmButton.onClick.AddListener(ConfirmButtonMethod);
    }

    private void ConfirmButtonMethod() {
        if (_selectedCard == null) return;

        OnMonsterCardSelected?.Invoke(_selectedCard);
        MonsterUIDisplayOff();

        //PhoneController.Instance.ResetPhone();
    }

    #endregion

    #region Handle Events

    void SubscribeToEvents() {
        MonsterCardPrefab.OnCardSelected += HandleCardSelected;
        JournalCloseButton.OnCloseJournalButtonPreesed += MonsterUIDisplayOn;
        QuestValidator.OnRescueQuestWon += HandleRescueQuestWon;
    }

    private void HandleRescueQuestWon() {
        _canTurnScreenOn = true;
    }

    private void HandleCardSelected(MonsterCardPrefab card) {
        _selectedCard = card;
    }

    #endregion

    #region Main Logic

    void MonsterUIDisplayOn() {

        if (!_canTurnScreenOn) return;

        _canTurnScreenOn = false;

        List<MonsterData> unregisteredMonster = ReturnUnRegisteredMonsters().OrderBy(x => UnityEngine.Random.value).ToList();

        if (unregisteredMonster == null || unregisteredMonster.Count == 0) return;

        while (unregisteredMonster.Count > 3) {
            unregisteredMonster.RemoveAt(unregisteredMonster.Count - 1);
        }

        mask.blocksRaycasts = true;

        _fadeTween = mask.DOFade(fadeFinalValue, fadeDuration);

        _scaleTween = container.transform.DOScale(1, scaleDuration).OnComplete(() => {
            StartCoroutine(InstantiateMonsterCards(unregisteredMonster));
        });
    }

    List<MonsterData> ReturnUnRegisteredMonsters() {
        List<MonsterData> listOfMosters = new(listOfAllMonsters);

        List<MonsterInfo> listOfRegisteredMonsters = SingleEntryPoint.Instance.ManagerManager.MonsterMan.ListOfRegisteredMonsters;

        foreach (var monster in listOfAllMonsters) {
            if (CheckIfHasMonster(monster, listOfRegisteredMonsters)) {
                listOfMosters.Remove(monster);
            }
        }

        return listOfMosters;
    }

    bool CheckIfHasMonster(MonsterData monster, List<MonsterInfo> listOfRegisteredMonster) {

        foreach (var monsterInfo in listOfRegisteredMonster) {
            if (monsterInfo.SelectedData == monster) {
                return true;
            }
        }

        return false;
    }

    IEnumerator InstantiateMonsterCards(List<MonsterData> listOfMonsters) {

        WaitForSeconds wait = new(instantiateInterval);
        List<MonsterCardPrefab> listOfMonsterCard = new();

        for (int i = 0; i < listOfMonsters.Count; i++) {
            listOfMonsterCard.Add(InstantiateCard());
        }

        for (int i = 0; i < listOfMonsterCard.Count; i++) {
            listOfMonsterCard[i].Setup(listOfMonsters[i]);
            yield return wait;
        }

        _confirmButtonTween = confirmButton.transform.DOScale(1, scaleDuration);
    }

    MonsterCardPrefab InstantiateCard() {

        foreach (var card in _listOfExistingCards) {
            if (!card.HasBeenUsed) {
                card.HasBeenUsed = true;
                return card;
            }
        }

        MonsterCardPrefab newCard = Instantiate(monsterCardPrefab, container.transform);
        if (!_listOfExistingCards.Contains(newCard)) _listOfExistingCards.Add(newCard);
        newCard.HasBeenUsed = true;
        return newCard;
    }

    #endregion

    #region End

    void MonsterUIDisplayOff() {

        _confirmButtonTween = confirmButton.transform.DOScale(0, scaleDuration);
        _fadeTween = mask.DOFade(0, fadeDuration);
        _scaleTween = container.transform.DOScale(0, scaleDuration).OnComplete(() => {
            foreach (var card in _listOfExistingCards) {
                card.DisableCard();
            }

            mask.blocksRaycasts = false;
        });
    }

    private void OnDestroy() {
        UnsubscribeToEvents();
        KillTweens();
    }

    void UnsubscribeToEvents() {
        MonsterCardPrefab.OnCardSelected -= HandleCardSelected;
        JournalCloseButton.OnCloseJournalButtonPreesed -= MonsterUIDisplayOn;
        QuestValidator.OnRescueQuestWon -= HandleRescueQuestWon;
    }

    void KillTweens() {
        DOTween.Kill(_fadeTween);
        DOTween.Kill(_scaleTween);
        DOTween.Kill(_confirmButtonTween);
    }

    #endregion
}
