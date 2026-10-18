using AYellowpaper.SerializedCollections;
using UnityEngine;
using UnityEngine.UI;

public class MonsterLayoutScene : MonoBehaviour
{
    #region Variables

    [SerializeField, SerializedDictionary("Monster", "Image")] SerializedDictionary<MonsterData, GameObject> dictionaryOfMonstersImage;

    #endregion

    #region Setup

    private void Awake() {
        SubscribeToEvents();
    }

    void Setup() {
        SubscribeToEvents();
    }

    #endregion

    #region Handle Events

    void SubscribeToEvents() {
        MonstersManager.OnMonsterRegistered += HandleMonsterRegistered;
    }

    void HandleMonsterRegistered(MonsterInfo info) {
        MonsterData data = info.SelectedData;
        if (!dictionaryOfMonstersImage.ContainsKey(data)) return;

        dictionaryOfMonstersImage[data].SetActive(true);
    }

    #endregion

    #region End

    private void OnDestroy() {
        UnsubscribeToEvents();
    }

    void UnsubscribeToEvents() {
        MonstersManager.OnMonsterRegistered -= HandleMonsterRegistered;
    }

    #endregion
}
