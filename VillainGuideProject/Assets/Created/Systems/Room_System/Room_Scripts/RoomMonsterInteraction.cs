using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public sealed class RoomMonsterInteraction : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private GameObject hoverOutline;
    [SerializeField] private CreateMonster_Data profile;

    internal CreateMonster_Data Profile => profile;

    private void Awake()
    {
        GetComponent<Button>().onClick.AddListener(OpenProfile);
    }

    private void OpenProfile()
    {
        Main_Referencer.Instance.manager.monsterManager.OpenRoomProfile(profile);
    }

    internal void SetAvailable(bool available)
    {
        transform.parent.gameObject.SetActive(available);
    }
    
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
