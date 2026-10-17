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

    public void Start()
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

        SingleEntryPoint.Instance.UiManager.MissionsScreenController.ShowMissionsScreen();
        SingleEntryPoint.Instance.UiManager.MissionsScreenController.SetupQuests();
    }

    private IEnumerator PhoneRing()
    {
        yield return new WaitForSeconds(3f);

        if (phoneAnswered)
            yield break;

        phoneButton.onClick.AddListener(AnswerPhone);
        exclamationIcon.transform.localScale = Vector3.zero;
        exclamationIcon.SetActive(true);

        exclamationIcon.transform
            .DOScale(Vector3.one, animationDuration)
            .SetEase(Ease.OutBack);
    }

    //PARA TOCAR O TELEFONE SEM A CORROUTINA, DEPOIS DE LER O JORNAL

    //public void PhoneRing()
    //{
    //    if (phoneAnswered)
    //        return;

    //    phoneButton.onClick.AddListener(AnswerPhone);

    //    exclamationIcon.transform.localScale = Vector3.zero;
    //    exclamationIcon.SetActive(true);

    //    exclamationIcon.transform
    //        .DOScale(Vector3.one, animationDuration)
    //        .SetEase(Ease.OutBack);
    //}

    //private IEnumerator PhoneRingDelay()
    //{
    //    yield return new WaitForSeconds(3f);

    //    PhoneRing();
    //}
}