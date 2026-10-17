using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using DG.Tweening;

public class PhoneController : MonoBehaviour
{
    [SerializeField] private GameObject exclamationIcon;
    private float animationDuration = 0.5f;

    private void Start()
    {
        GetComponent<Button>().onClick.AddListener(AnswerPhone);
        exclamationIcon.SetActive(false);
        StartCoroutine(PhoneRing());
    }

    private void AnswerPhone()
    {
        exclamationIcon.transform
            .DOScale(Vector3.zero, animationDuration)
            .SetEase(Ease.InBack)
            .OnComplete(() =>
            {
                exclamationIcon.SetActive(false);
            });

        MissionsScreenController.Instance.ShowMissionsScreen();
    }

    private IEnumerator PhoneRing()
    {
        yield return new WaitForSeconds(3f);

        exclamationIcon.transform.localScale = Vector3.zero;
        exclamationIcon.SetActive(true);

        exclamationIcon.transform
            .DOScale(Vector3.one, animationDuration)
            .SetEase(Ease.OutBack);
    }
}