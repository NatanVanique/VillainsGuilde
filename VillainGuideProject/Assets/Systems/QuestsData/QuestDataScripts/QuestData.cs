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
    public LocalizedString QuestTitle;

    [Header("Emergency warning")]
    public LocalizedString EmergencyRequirements;

    [Header("Journal Results")]
    public LocalizedString ResultDescriptionGood;
    public LocalizedString ResultDescriptionBad;

    [Space(10)]

    [Header("Card")]
    public QuestTipData TipData;
    [Range(1, 3)] public int AmountOfMaxCharacters;

    [Space(10)]

    [Header("Rewards")]
    public bool HasMoney;
    [ShowIf(nameof(HasMoney))] public float MoneyReward;
    public bool HasXP;
    [ShowIf(nameof(HasXP))] public float XPReward;
    public bool HasCharacters;
    [ShowIf(nameof(HasCharacters))] public List<int> ListOfCharacters;

    [Header("Win or loose")]
    [Range(0, 100)] public float BasePercentChanceOfWinning = 0; 
    public bool HasConsequence;
    [ShowIf(nameof(HasConsequence))] public int AmountOfConsequenceEvents;

}

[Serializable]
public struct TipData {

    public LocalizedString TipText;
    public TipType Type;
    Color TipColor;

}

[Serializable]
public struct QuestTipData {
    public LocalizedString TipText;
    public List<TipType> Type;
}

public enum QuestType { Emergency, Mysterius, Classic, Rescue }
public enum TipType { Day, Night, Flying, Grounded, Charisma, Fear, Animal, Empathy, Blood, Strength, Contention, Discretion, Ilusion, Protection, Speed, Diplomacy }
public enum QuestRewardType { Character, XP, Money }
public enum QuestLine { Mayor, Second, Third }
