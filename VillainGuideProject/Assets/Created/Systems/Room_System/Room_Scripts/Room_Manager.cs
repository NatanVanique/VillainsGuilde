using System;
using UnityEngine;
using UnityEngine.UI;

public sealed class RoomManager : MonoBehaviour
{
    [Header("Hour Changes")]
    [SerializeField] DaycicleManager daycicleManager;
    [SerializeField] private Gradient dayColorGradient;
    [SerializeField] private Image colorImage;

    private void Update()
    {
        UpdateDayColor();
    }

    private void UpdateDayColor()
    {
        float normalizedHour = daycicleManager.currentHour / 24f;
        colorImage.color = dayColorGradient.Evaluate(normalizedHour);
    }
}
