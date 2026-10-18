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

    #endregion

    #region Test

    private void Start() {
        Setup();
        HandleDayChanged(Mathf.Max(1, SingleEntryPoint.Instance.ManagerManager.DaySystem.CurrentDay));
    }

    #endregion

    #region Setup

    public void Setup() {
        SubscribeToEvents();
    }

    #endregion

    #region Handle Events

    void SubscribeToEvents() {
        DaySystem.OnCurrentDayChanged += HandleDayChanged;
    }

    void HandleDayChanged(int newDay) {
        Debug.Log(newDay);
        if (!dictionaryOfDayInfo.ContainsKey(newDay)) return;

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
            if (AddRandomQuestFromList(listOsRescueQuests)) {
                _hasFoundCharacter = false;
                totalAmountOfQuestsLefts--;
            }
        }

        for (int i = 0; i < totalAmountOfQuestsLefts; i++) {

            if (newDayQuestData.CanFindRescue && !_hasFoundCharacter) { // Quer encontrar personagem
                int random = Random.Range(0, 2);

                if (random == 0) { // Encontrou
                    List<QuestData> listOsRescueQuests = newDayQuestData.DictionaryOfPossibleQuests[QuestType.Rescue];
                    if (AddRandomQuestFromList(listOsRescueQuests)) {
                        _hasFoundCharacter = true;
                        continue;
                    }
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

    }
    #endregion

    #region Sup

    bool AddRandomQuestFromList(List<QuestData> listOfDataToAdd) {
        List<QuestData> validQuests = new();
        if (listOfDataToAdd != null) {
            foreach (QuestData quest in listOfDataToAdd)
                if (quest != null) validQuests.Add(quest);
        }
        if (validQuests.Count == 0) {
            Debug.LogWarning("A quest list has no valid QuestData references; check the day configuration.", this);
            return false;
        }
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
