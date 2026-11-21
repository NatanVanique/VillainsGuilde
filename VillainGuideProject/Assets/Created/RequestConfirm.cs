using UnityEngine;

public class RequestConfirm : MonoBehaviour
{
    public void OpenConfirm(GameObject confirmObject)
    {
        gameObject.SetActive(false);
        confirmObject.SetActive(true);   
    }
    
    public void CloseConfirm(GameObject button)
    {
        gameObject.SetActive(false);
        button.SetActive(true);   
    }
}
