using JetBrains.Annotations;
using System;
using UnityEngine;

public class HealthManager : MonoBehaviour
{

    #region Variables

    [SerializeField] int maxHealth;
    int _currentHealth;

    public static event Action OnHealthZero;
    public static event Action<int> OnHealthChanged;

    #endregion

    #region Setup

    public void Setup() {
        SetMaxHealth();
        SubscribeToEvents();
    }

    #endregion

    #region Handle Events

    void SubscribeToEvents() {
        QuestValidator.OnQuestWithResultFailed += HandleQuestsFailed;
    }

    private void HandleQuestsFailed(int obj) {
        LooseHealth(obj);
    }

    #endregion

    #region Handle Variables

    public void SetMaxHealth() {
        _currentHealth = maxHealth;
    }

    void LooseHealth(int amountToLoose) {
        _currentHealth = Mathf.Max(0, _currentHealth - amountToLoose);

        if (_currentHealth <= 0) OnHealthZero?.Invoke();
        else OnHealthChanged?.Invoke(_currentHealth);
    }

    #endregion

    #region End

    private void OnDestroy() {
        UnsubscribeToEvents();
    }

    void UnsubscribeToEvents() {

    }

    #endregion
}
