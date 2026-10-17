using UnityEngine;
using UnityEngine.UI;
using AYellowpaper.SerializedCollections;
using TMPro;
using UnityEngine.Localization.Components;

public class QuestCard : MonoBehaviour
{
    #region Variables

    [Header("Components")]
    [SerializeField] Image questIcon;
    [SerializeField, SerializedDictionary("index", "text")] SerializedDictionary<int, LocalizeStringEvent> dictionaryOfTips;
    [SerializeField, SerializedDictionary("amount", "container")] SerializedDictionary<int, GameObject> dictionaryOfAmountOfCharacters;

    QuestData _data;

    #endregion

    #region Setup

    public void Setup(QuestData data) {
        _data = data;
    }

    #endregion

    void ChangeIcon() {
        questIcon.sprite = _data.QuestIcon;
    }

    void ChangeCharactersAmountIcons() {
        foreach(var pair in dictionaryOfAmountOfCharacters) {
            if (pair.Key == _data.AmountOfMaxCharacters) pair.Value.SetActive(true);
            else pair.Value.SetActive(false);
        }
    }

    void ChangeTips() {

    }
}
