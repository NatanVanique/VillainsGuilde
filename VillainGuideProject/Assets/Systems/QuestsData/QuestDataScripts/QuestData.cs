using AYellowpaper.SerializedCollections;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization;
using NaughtyAttributes;

[CreateAssetMenu(menuName = "QuestData")]
public class QuestData : ScriptableObject {

    [Header("Quest")]
    public QuestType Type;
    public Sprite QuestIcon;
    public LocalizedString QuestTitle;

    [Space(10)]

    [Header("Card")]
    [SerializedDictionary("index", "list of tips")] public SerializedDictionary<int, List<TipData>> DictionaryOfTipData = new();
    [Range(1, 3)] public int AmountOfMaxCharacters;

    [Space(10)]

    [Header("Rewards")]
    public bool HasMoney;
    [ShowIf(nameof(HasMoney))] public float MoneyReward;
    public bool HasXP;
    [ShowIf(nameof(HasXP))] public float XPReward;
    public bool HasCharacters;
    [ShowIf(nameof(HasCharacters))] public List<int> ListOfCharacters;
    public bool HasConsequence;
    [ShowIf(nameof(HasConsequence))] public int AmountOfConsequenceEvents;

}

[Serializable]
public struct TipData {

    public LocalizedString TipText;
    public TipType Type;

}

public enum QuestType { Emergency, Mysterius, Classic, Rescue }
public enum TipType { Day, Night, Flying }
public enum QuestRewardType { Character, XP, Money }
public enum QuestLine { Mayor, Second, Third }
