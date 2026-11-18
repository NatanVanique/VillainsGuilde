using UnityEngine;
using UnityEngine.EventSystems;

public sealed class TelephoneHover : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public void OnPointerEnter(PointerEventData eventData)
    {
        Main_Referencer.Instance.manager.telephoneManager.OnPointerEnter(eventData);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        Main_Referencer.Instance.manager.telephoneManager.OnPointerExit(eventData);
    }
}
