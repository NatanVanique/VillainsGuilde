using AYellowpaper.SerializedCollections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization;

[CreateAssetMenu(menuName = "MonsterData")]
public class MonsterData : ScriptableObject {

    [Header("General Atributes")]
    public LocalizedString Name;
    public Sprite SplashArt;
    public Sprite MonsterCardSprite;
    public Sprite Icon;
    public Sprite Background;

    [Header("Tips")]
    [SerializedDictionary("tip index", "list of possible tiles")] public SerializedDictionary<int, List<TipData>> DictionaryOfTips = new();

}
