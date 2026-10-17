using AYellowpaper.SerializedCollections;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization;

[CreateAssetMenu(menuName = "QuestData")]
public class QuestData : ScriptableObject {

    [Header("Quest")]
    public QuestType Type;
    public Sprite QuestIcon;

    [Header("Card")]
    [SerializedDictionary("index", "list of tips")] public SerializedDictionary<int, List<TipData>> DictionaryOfTipData = new();
    [Range(1, 3)] public int AmountOfMaxCharacters;
}

[Serializable]
public struct TipData {

    public LocalizedString TipText;
    public TipType Type;

}

public enum QuestType { Emergency, Mistery, Classic }
public enum TipType { Day, Night, Flying }
