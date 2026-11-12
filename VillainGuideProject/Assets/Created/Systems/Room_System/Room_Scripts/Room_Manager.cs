using System;
using UnityEngine;
using UnityEngine.UI;

public sealed class RoomManager : MonoBehaviour
{
    private void Update()
    {
        UpdateDayColor();
    }

    private void UpdateDayColor()
    {
        float normalizedHour = Main_Referencer.Instance.value.currentHour_HourValues / 24f;
        Main_Referencer.Instance.visual.colorImage_Room.color = Main_Referencer.Instance.value.dayGradient_HourValues.Evaluate(normalizedHour);
    }
}
