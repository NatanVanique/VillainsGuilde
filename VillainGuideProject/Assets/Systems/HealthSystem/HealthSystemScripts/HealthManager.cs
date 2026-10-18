using System;
using UnityEngine;

public class HealthManager : MonoBehaviour
{

    #region Variables

    [SerializeField, Min(1)] int maxHealth = 3;
    int _currentHealth;
    bool initialized;
    public int CurrentHealth => _currentHealth;
    public int MaxHealth => maxHealth;
    public int PendingHealthLoss { get; private set; }

    public int ConsumePendingHealthLoss() {
        int lost = PendingHealthLoss;
        PendingHealthLoss = 0;
        return lost;
    }

    public static event Action OnHealthZero;
    public static event Action<int> OnHealthChanged;

    #endregion

    #region Setup

    public void Setup() {
        if (initialized) return;
        initialized = true;
        SetMaxHealth();
        SubscribeToEvents();
    }

    #endregion

    #region Handle Events

    void SubscribeToEvents() {
        QuestValidator.OnQuestWithResultFailed -= HandleQuestsFailed;
        QuestValidator.OnQuestWithResultFailed += HandleQuestsFailed;
    }

    private void HandleQuestsFailed(int obj) {
        LooseHealth(obj);
    }

    #endregion

    #region Handle Variables

    public void SetMaxHealth() {
        PendingHealthLoss = 0;
        _currentHealth = Mathf.Max(1, maxHealth);
        OnHealthChanged?.Invoke(_currentHealth);
    }

    void LooseHealth(int amountToLoose) {
        if (_currentHealth <= 0 || amountToLoose <= 0) return;
        int previousHealth = _currentHealth;
        _currentHealth = Mathf.Max(0, _currentHealth - amountToLoose);
        PendingHealthLoss += previousHealth - _currentHealth;
        OnHealthChanged?.Invoke(_currentHealth);
        if (_currentHealth <= 0) OnHealthZero?.Invoke();
    }

    #endregion

    #region End

    private void OnDestroy() {
        UnsubscribeToEvents();
    }

    void UnsubscribeToEvents() {
        QuestValidator.OnQuestWithResultFailed -= HandleQuestsFailed;
    }

    #endregion
}
