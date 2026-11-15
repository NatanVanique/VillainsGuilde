using UnityEngine;

public sealed class RoomManager : MonoBehaviour
{

    private void Update()
    {
        UpdateDayColor();
    }

    private void UpdateDayColor()
    {
        Main_Referencer.Instance.visual.colorImage_Room.color =
            Main_Referencer.Instance.value.dayGradient_HourValues.Evaluate(
                Main_Referencer.Instance.manager.dayCicleManager.NormalizedDayProgress);
    }
}
