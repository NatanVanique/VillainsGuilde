using AYellowpaper.SerializedCollections;
using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization.Components;
using UnityEngine.UI;

public class MonsterInfoCard : MonoBehaviour {
    #region Variables

    [Header("Components")]
    [SerializeField] GameObject container;
    [SerializeField] Image splashArt;
    [SerializeField] Image icon;
    [SerializeField] LocalizeStringEvent nameText;
    [SerializeField] List<Button> xButton;
    [SerializeField] CanvasGroup maskCanvasGroup;
    [SerializeField, SerializedDictionary("index", "localize string event")] SerializedDictionary<int, LocalizeStringEvent> dictionaryOfTipTexts = new();

    [Header("Animation")]
    [SerializeField] float fadeInDuration;
    [SerializeField] float fadeInFinalValue;
    [SerializeField] float scaleDuration;

    // Tween
    Tween _fadeTween, _scaleTween;
    #endregion

    #region Setup

    private void Start() {
        Setup();

        //XButton();
    }

    public void Setup() {
        SubscribeToEvents();
        SetButtons();
    }

    #endregion

    #region Buttons

    void SetButtons() {
        foreach (var button in xButton) {
            button.onClick.AddListener(XButton);
        }
    }

    void XButton() {
        FadeOutAnimation();
    }

    #endregion

    #region Handle Events

    void SubscribeToEvents() {
        MonsterCardPrefab.OnMonsterCardInfoButtonPressed += HandleCardInfoClicked;
    }

    void HandleCardInfoClicked(MonsterData data, MonsterTips selectedTips) {

        SetImagesAndName(data);

        SetTips(selectedTips);

        FadeInAnimation();
    }

    #endregion

    #region Animation

    void FadeInAnimation() {
        maskCanvasGroup.blocksRaycasts = true;

        maskCanvasGroup.alpha = 0;
        _fadeTween = maskCanvasGroup.DOFade(fadeInFinalValue, fadeInDuration);

        container.transform.localScale = Vector3.zero;

        container.SetActive(true);

        _scaleTween = container.transform.DOScale(1, scaleDuration).OnComplete(() => {
            FinishScale();
        });
    }

    void FinishScale() {
        foreach (var button in xButton) {
            button.enabled = true;
        }
    }

    void FadeOutAnimation() {
        _fadeTween = maskCanvasGroup.DOFade(0, fadeInDuration);
        _scaleTween = container.transform.DOScale(0, scaleDuration).OnComplete(() => {
            FinishFadeOutAnimation();
        });
    }

    void FinishFadeOutAnimation() {

        maskCanvasGroup.blocksRaycasts = false;

        foreach (var button in xButton) {
            button.enabled = false;
        }

        container.SetActive(false);
    }

    #endregion

    #region Visuals

    void SetImagesAndName(MonsterData data) {
        if (data.SplashArt != null) splashArt.sprite = data.SplashArt;
        if (data.Icon != null) icon.sprite = data.Icon;

        if (data.Name != null) nameText.StringReference = data.Name;
    }

    void SetTips(MonsterTips selectedTips) {
        foreach (var tip in dictionaryOfTipTexts) {
            tip.Value.StringReference = selectedTips.SelectedTips[tip.Key].TipText;
        }
    }

    #endregion

    #region End

    private void OnDestroy() {
        UnSubscribeToEvents();
        KillTweens();
    }

    void UnSubscribeToEvents() {
        MonsterCardPrefab.OnMonsterCardInfoButtonPressed -= HandleCardInfoClicked;
    }

    void KillTweens() {
        DOTween.Kill(_fadeTween);
        DOTween.Kill(_scaleTween);
    }

    #endregion
}
