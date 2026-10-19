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
    private bool started;

    #endregion

    #region Setup
    private void Start() {
        started = true;
        Setup();
    }
    public void Setup() {
        SubscribeToEvents();
        HealthManager health = SingleEntryPoint.Instance.ManagerManager.HealthMan;
        if (health != null) HandleHealthChanged(health.CurrentHealth);
    }

    private void OnEnable() {
        SubscribeToEvents();
        if (started) Setup();
    }

    private void OnDisable() {
        UnSubscribeToEvents();
    }

    #endregion

    #region HandleEvents

    void SubscribeToEvents() {
        HealthManager.OnHealthChanged -= HandleHealthChanged;
        HealthManager.OnHealthChanged += HandleHealthChanged;
    }

    private void HandleHealthChanged(int currentHealth) {
        foreach (var icon in dictionaryOfHealthIcons) {
            if (icon.Value != null)
                icon.Value.sprite = icon.Key < currentHealth ? regularHealthSprite : damagedHealthSprite;
        }
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
