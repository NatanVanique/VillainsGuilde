using JetBrains.Annotations;
using UnityEngine;

public class HealthManager : MonoBehaviour
{

    #region Variables

    [SerializeField] int maxHealth;
    int _currentHealth;

    #endregion

    #region Setup

    public void Setup() {
        SetMaxHealth();
        SubscribeToEvents();
    }

    #endregion

    #region Handle Events

    void SubscribeToEvents() {

    }

    #endregion

    #region Handle Variables

    public void SetMaxHealth() {
        _currentHealth = maxHealth;
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
