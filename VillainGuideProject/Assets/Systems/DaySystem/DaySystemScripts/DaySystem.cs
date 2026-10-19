using System;
using UnityEngine;

public class DaySystem : MonoBehaviour
{
    #region Variables

    [Header("Atributes")]
    [SerializeField] int maxAmountOfDays;

    // private variables
    int _currentDay = 1;
    public int CurrentDay => _currentDay;
    public int CompletedDays { get; private set; }
    public bool HasCompletedAllDays => maxAmountOfDays > 0 && CompletedDays >= maxAmountOfDays;

    public void CompleteCurrentDay() {
        CompletedDays = _currentDay;
    }

    // events
    /// <summary>
    /// Chamado quando um dia muda. Passa o index do novo dia;
    /// </summary>
    public static event Action<int> OnCurrentDayChanged;

    #endregion

    #region Setup

    public void Setup() {

    }

    #endregion

    #region Main Logic

    public void AdvanceDay() {
        if (_currentDay >= maxAmountOfDays) return;

        _currentDay++;
        OnCurrentDayChanged?.Invoke(_currentDay);
    }

    #endregion
}
