using AYellowpaper.SerializedCollections;
using System.Collections.Generic;
using UnityEngine;

public class QuestSelector : MonoBehaviour {

    #region Variables

    [SerializeField, SerializedDictionary("day", "day info")] SerializedDictionary<int, DayQuestsData> dictionaryOfDayInfo = new();

    List<QuestData> _selectedQuests = new();
    QuestLine _currentLine;
    bool _hasFoundCharacter;

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
        if (!dictionaryOfDayInfo.ContainsKey(newDay)) return;

        _selectedQuests.Clear();

        DayQuestsData newDayQuestData = dictionaryOfDayInfo[newDay];

        int totalAmountOfQuestsLefts = newDayQuestData.totalAmountOfQuests;

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
                }

                else { // Não encontrou
                    List<QuestData> listOfClassicQuests = newDayQuestData.DictionaryOfPossibleQuests[QuestType.Classic];
                    AddRandomQuestFromList(listOfClassicQuests);
                }
            }

        }

    }
    #endregion

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

