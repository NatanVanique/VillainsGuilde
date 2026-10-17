using AYellowpaper.SerializedCollections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization.Components;
using UnityEngine.UI;

public class MonsterCardPrefab : MonoBehaviour {

    [Header("Components")]
    [SerializeField] Image SplashArt;
    [SerializeField] Image Background;
    [SerializeField] LocalizeStringEvent Name;
    //[SerializeField, SerializedDictionary("index", "localize string event")] SerializedDictionary<int, LocalizeStringEvent> dictionaryOfTipTexts = new();

    MonsterTips _selectedMonsterTips;

    [Header("Test")]
    [SerializeField] MonsterData data;

    private void Start() {
        Setup(data);
    }

    public void Setup(MonsterData data) {
        _selectedMonsterTips = ChooseRandomTips(data);
        SetVisual(data);
    }

    MonsterTips ChooseRandomTips(MonsterData data) {

        List<TipData> listOfTips = new();

        foreach (var pair in data.DictionaryOfTips) {
            List<TipData> listOsTips = pair.Value;
            int randomIndex = Random.Range(0, listOsTips.Count);
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

        MonsterTips monsterTips = new(listOfTips);

        return monsterTips;
    }

    void SetVisual(MonsterData data) {
        if (data.SplashArt != null) SplashArt.sprite = data.SplashArt;
        if (data.Name != null) Name.StringReference = data.Name;
        if (data.Background != null) Background.sprite = data.Background;

        //foreach (var pair in dictionaryOfTipTexts) {
        //    pair.Value.StringReference = _selectedMonsterTips.SelectedTips[pair.Key].TipText;
        //}
    }
}

public struct MonsterTips {
    public Dictionary<int, TipData> SelectedTips;

    public MonsterTips(List<TipData> listOfTips) {
        SelectedTips = new() {
            [1] = listOfTips[0],
            [2] = listOfTips[1],
            [3] = listOfTips[2]
        };
    }
}
