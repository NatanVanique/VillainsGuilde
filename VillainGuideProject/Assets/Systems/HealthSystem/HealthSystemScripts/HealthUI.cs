using AYellowpaper.SerializedCollections;
using System;
using UnityEngine;
using UnityEngine.UI;

public class HealthUI : MonoBehaviour
{
    #region Variables

    [SerializedDictionary("index", "image"), SerializeField] SerializedDictionary<int, Image> dictionaryOfHealthIcons = new();
    [SerializeField] Sprite regularHealthSprite;
    [SerializeField] Sprite damagedHealthSprite;

    #endregion

    #region Setup
    public void Setup() {
        SubscribeToEvents();
    }

    #endregion

    #region HandleEvents

    void SubscribeToEvents() {
        HealthManager.OnHealthChanged += HandleHealthChanged;
    }

    private void HandleHealthChanged(int obj) {
        dictionaryOfHealthIcons[obj].sprite = damagedHealthSprite;
    }

    #endregion

    #region End

    private void OnDestroy() {
        UnSubscribeToEvents();
    }

    void UnSubscribeToEvents() {
        HealthManager.OnHealthChanged -= HandleHealthChanged;
    }

    #endregion
}
