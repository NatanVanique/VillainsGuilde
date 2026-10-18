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
        Debug.Log(newDay);
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

        if (newDayQuestData.IsEmergency) { // VERIFICANDO SE É UMA EMERGÊNCIA
            _selectedQuests.Add(newDayQuestData.DictionaryOfMandatoryQuests[_currentLine]);
            totalAmountOfQuestsLefts--;
        }

        if (newDayQuestData.CanFindRescue && !_hasFoundCharacter && newDayQuestData.ResetCharacter && totalAmountOfQuestsLefts > 0) {

            // PRECISA ENCONTRAR UM PERSONAGEM
            List<QuestData> listOsRescueQuests = newDayQuestData.DictionaryOfPossibleQuests[QuestType.Rescue];
            AddRandomQuestFromList(listOsRescueQuests);

            _hasFoundCharacter = false;
            totalAmountOfQuestsLefts--;
        }

        for (int i = 0; i < totalAmountOfQuestsLefts; i++) {

            if (newDayQuestData.CanFindRescue && !_hasFoundCharacter) { // Quer encontrar personagem
                int random = Random.Range(0, 2);

                if (random == 0) { // Encontrou
                    List<QuestData> listOsRescueQuests = newDayQuestData.DictionaryOfPossibleQuests[QuestType.Rescue];
                    AddRandomQuestFromList(listOsRescueQuests);
                    _hasFoundCharacter = true;
                    continue;
                }
            }

            List<QuestData> listOfQuestsToChoose;

            if (amountOfMysteriusLeft > 0) { // Precisa ser uma misteriosa
                listOfQuestsToChoose = newDayQuestData.DictionaryOfPossibleQuests[QuestType.Mysterius];
                amountOfMysteriusLeft--;
            }
            else { // Clássica
                listOfQuestsToChoose = newDayQuestData.DictionaryOfPossibleQuests[QuestType.Classic];
            }

            AddRandomQuestFromList(listOfQuestsToChoose);

        }

        _selectedDay = newDay;
    }
    #endregion

    public void EnsureCurrentDay(int currentDay) {
        HandleDayChanged(currentDay);
    }

    #region Sup

    void AddRandomQuestFromList(List<QuestData> listOfDataToAdd) {
        int randomQuestId = Random.Range(0, listOfDataToAdd.Count);
        QuestData selectedQuest = listOfDataToAdd[randomQuestId];
        _selectedQuests.Add(selectedQuest);
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