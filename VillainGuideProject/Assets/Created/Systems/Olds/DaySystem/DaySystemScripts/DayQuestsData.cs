using AYellowpaper.SerializedCollections;
using NaughtyAttributes;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "DayQuestsData")]
public class DayQuestsData : ScriptableObject {

    [Header("Emergency")]
    public bool IsEmergency = false;
    [ShowIf(nameof(IsEmergency))] public SerializedDictionary<QuestLine, QuestData> DictionaryOfMandatoryQuests = new();

    [Header("Random Quests")]
    public int totalAmountOfQuests;
    public int totalAmountOfMysterious;
    public SerializedDictionary<QuestType, List<QuestData>> DictionaryOfPossibleQuests = new();

    [Header("Settings")]
    public bool CanFindRescue;
    public bool ResetCharacter;

}
