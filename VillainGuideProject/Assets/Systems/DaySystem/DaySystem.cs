using System;
using UnityEngine;

public class DaySystem : MonoBehaviour
{
    #region Variables

    [Header("Atributes")]
    [SerializeField] int maxAmountOfDays;

    // private variables
    int _currentDay = 0;

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
        _currentDay++;
        OnCurrentDayChanged?.Invoke(_currentDay);
    }

    #endregion
}
