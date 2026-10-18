using DG.Tweening;
using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
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

    [Header("Test")]
    [SerializeField] List<MonsterData> listOfData = new();

    // private
    public List<MonsterCardPrefab> _listOfExistingCards = new();

    Tween _scaleTween, _fadeTween, _confirmButtonTween;
    #endregion

    #region Setup

    private void Start() {
        MonsterUIDisplayOn(listOfData);
    }

    public void Setup() {
        SubscribeToEvents();
    }

    #endregion

    #region Handle Events

    void SubscribeToEvents() {

    }

    #endregion

    #region Main Logic

    void MonsterUIDisplayOn(List<MonsterData> listOfMonsters) {

        mask.blocksRaycasts = true;

        _fadeTween = mask.DOFade(fadeFinalValue, fadeDuration);
        _scaleTween = container.transform.DOScale(1, scaleDuration).OnComplete(() => {
            StartCoroutine(InstantiateMonsterCards(listOfMonsters));
            _confirmButtonTween = confirmButton.transform.DOScale(1, scaleDuration);
        });
    }

    IEnumerator InstantiateMonsterCards(List<MonsterData> listOfMonsters) {

        WaitForSeconds wait = new(instantiateInterval);
        List<MonsterCardPrefab> listOfMonsterCard = new();

        for (int i = 0; i < listOfMonsters.Count; i++) {
            listOfMonsterCard.Add(InstantiateCard());
        }

        for (int i = 0; i < listOfMonsterCard.Count; i++) {
            yield return wait;
            listOfMonsterCard[i].Setup(listOfMonsters[i]);
        }

    }

    MonsterCardPrefab InstantiateCard() {

        foreach(var card in _listOfExistingCards) {
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

    private void OnDestroy() {
        UnsubscribeToEvents();
        KillTweens();
    }

    void UnsubscribeToEvents() {

    }

    void KillTweens() {
        DOTween.Kill(_fadeTween);
        DOTween.Kill(_scaleTween);
    }

    #endregion
}
