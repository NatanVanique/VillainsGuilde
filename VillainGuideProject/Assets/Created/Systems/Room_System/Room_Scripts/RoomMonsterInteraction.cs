using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public sealed class RoomMonsterInteraction : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private GameObject hoverOutline;
    
    public void OnPointerEnter(PointerEventData eventData)
    {
        hoverOutline.SetActive(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        hoverOutline.SetActive(false);
    }

    private void OnDisable()
    {
        hoverOutline.SetActive(false);
    }
}
