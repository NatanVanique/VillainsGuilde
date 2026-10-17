using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using DG.Tweening;

public class PhoneController : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private GameObject exclamationIcon;
    private Button phoneButton;
    private float animationDuration = 0.5f;
    private bool phoneAnswered = false;

    private void Start()
    {
        phoneButton = GetComponent<Button>();

        exclamationIcon.SetActive(false);

        StartCoroutine(PhoneRing());
    }

    private void AnswerPhone()
    {
        if (phoneAnswered)
            return;

        phoneAnswered = true;

        phoneButton.onClick.RemoveListener(AnswerPhone);

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
        yield return new WaitForSeconds(10f);

        if (phoneAnswered)
            yield break;

        phoneButton.onClick.AddListener(AnswerPhone);
        exclamationIcon.transform.localScale = Vector3.zero;
        exclamationIcon.SetActive(true);

        exclamationIcon.transform
            .DOScale(Vector3.one, animationDuration)
            .SetEase(Ease.OutBack);
    }
}