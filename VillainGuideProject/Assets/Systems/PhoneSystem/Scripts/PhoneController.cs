using UnityEngine;
using System.Collections;
using UnityEngine.UI;
public class PhoneController : MonoBehaviour
{
    [SerializeField] GameObject exclamationIcon;
    void Start()
    {
        gameObject.GetComponent<Button>().onClick.AddListener(AnswerPhone);
        StartCoroutine(PhoneRing());
    }

    void AnswerPhone()
    {
        exclamationIcon.SetActive(false);
    }

    IEnumerator PhoneRing()
    {
        yield return new WaitForSeconds(10f);
        exclamationIcon.SetActive(true);
    }

}