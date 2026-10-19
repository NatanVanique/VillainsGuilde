using AYellowpaper.SerializedCollections;
using System.Collections.Generic;
using UnityEngine;

public class QuestSelector : MonoBehaviour {

    public List<QuestData> SelectedQuests => _selectedQuests;

    #region Variables

    [SerializeField, SerializedDictionary("day", "day info")] SerializedDictionary<int, DayQuestsData> dictionaryOfDayInfo = new();

    List<QuestData> _selectedQuests = new();
    QuestLine _currentLine;
    bool _hasFoundCharacter;
    bool _isInitialized;
    int _selectedDay = -1;

    #endregion

    #region Test

    private void Start() {
        Setup();
    }

    #endregion

    #region Setup

    public void Setup() {
        Setup(SingleEntryPoint.Instance.ManagerManager.DaySystem.CurrentDay);
    }

    public void Setup(int currentDay) {
        if (_isInitialized) return;

        _isInitialized = true;
        SubscribeToEvents();
        HandleDayChanged(Mathf.Max(1, currentDay));
    }

    #endregion

    #region Handle Events

    void SubscribeToEvents() {
        DaySystem.OnCurrentDayChanged += HandleDayChanged;
    }

    void HandleDayChanged(int newDay) {
        if (_selectedDay == newDay) return;
        if (!dictionaryOfDayInfo.ContainsKey(newDay)) {
            Debug.LogError($"No quest configuration for day {newDay}.", this);
            _selectedQuests.Clear();
            return;
        }

        _selectedQuests.Clear();

        DayQuestsData newDayQuestData = dictionaryOfDayInfo[newDay];

        int totalAmountOfQuestsLefts = newDayQuestData.totalAmountOfQuests;
        int amountOfMysteriusLeft = newDayQuestData.totalAmountOfMysterious;

        if (newDayQuestData.IsEmergency) { // VERIFICANDO SE � UMA EMERG�NCIA
            if (newDayQuestData.DictionaryOfMandatoryQuests.TryGetValue(_currentLine, out QuestData mandatory) && mandatory != null) _selectedQuests.Add(mandatory);
            else totalAmountOfQuestsLefts++;
            totalAmountOfQuestsLefts--;
        }

        if (newDayQuestData.CanFindRescue && !_hasFoundCharacter && newDayQuestData.ResetCharacter && totalAmountOfQuestsLefts > 0) {

            // PRECISA ENCONTRAR UM PERSONAGEM
            newDayQuestData.DictionaryOfPossibleQuests.TryGetValue(QuestType.Rescue, out List<QuestData> listOsRescueQuests);
            if (AddRandomQuestFromList(listOsRescueQuests)) {
                totalAmountOfQuestsLefts--;
            }
            _hasFoundCharacter = true;
        }

        for (int i = 0; i < totalAmountOfQuestsLefts; i++) {

            if (newDayQuestData.CanFindRescue && !_hasFoundCharacter) { // Quer encontrar personagem
                int random = Random.Range(0, 2);

                if (random == 0) { // Encontrou
                    newDayQuestData.DictionaryOfPossibleQuests.TryGetValue(QuestType.Rescue, out List<QuestData> listOsRescueQuests);
                    if (AddRandomQuestFromList(listOsRescueQuests)) {
                        _hasFoundCharacter = true;
                        continue;
                    }
                }
            }

            List<QuestData> listOfQuestsToChoose;

            if (amountOfMysteriusLeft > 0) { // Precisa ser uma misteriosa
                newDayQuestData.DictionaryOfPossibleQuests.TryGetValue(QuestType.Mysterius, out listOfQuestsToChoose);
                amountOfMysteriusLeft--;
            }
            else { // Cl�ssica
                newDayQuestData.DictionaryOfPossibleQuests.TryGetValue(QuestType.Classic, out listOfQuestsToChoose);
            }

            if (!AddRandomQuestFromList(listOfQuestsToChoose)) {
                newDayQuestData.DictionaryOfPossibleQuests.TryGetValue(QuestType.Classic, out List<QuestData> fallback);
                if (!AddRandomQuestFromList(fallback))
                    Debug.LogError($"No valid quests available for day {newDay}.", newDayQuestData);
            }

        }

        if (newDayQuestData.ResetCharacter) _hasFoundCharacter = false;

        _selectedDay = newDay;
    }
    #endregion

    public void EnsureCurrentDay(int currentDay) {
        HandleDayChanged(currentDay);
    }

    public QuestData GetEmergencyForDay(int day)
    {
        if (!dictionaryOfDayInfo.TryGetValue(day, out DayQuestsData dayData) ||
            !dayData.IsEmergency || dayData.DictionaryOfMandatoryQuests == null)
            return null;

        dayData.DictionaryOfMandatoryQuests.TryGetValue(_currentLine, out QuestData emergency);
        return emergency;
    }

    #region Sup

    bool AddRandomQuestFromList(List<QuestData> listOfDataToAdd) {
        if (listOfDataToAdd == null) return false;
        List<QuestData> validQuests = listOfDataToAdd.FindAll(quest => quest != null);
        if (validQuests.Count == 0) return false;
        _selectedQuests.Add(validQuests[Random.Range(0, validQuests.Count)]);
        return true;
    }
    #endregion

    #region End

    private void OnDestroy() {
        UnsubscribeToEvents();
    }

    void UnsubscribeToEvents() {
        DaySystem.OnCurrentDayChanged -= HandleDayChanged;
    }

    #endregion

}