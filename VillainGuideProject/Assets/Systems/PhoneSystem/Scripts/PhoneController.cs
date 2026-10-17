using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using DG.Tweening;

public class PhoneController : MonoBehaviour
{
    public static PhoneController Instance { get; private set; }

    [Header("Components")]
    [SerializeField] private GameObject exclamationIcon;
    private Button phoneButton;
    private float animationDuration = 0.5f;
    private bool phoneAnswered = false;

    public void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
            return;
        }
        Instance = this;

        phoneButton = GetComponent<Button>();
        exclamationIcon.SetActive(false);
    }
    public void Start()
    {
        new WaitForSeconds(3f);
        PhoneRing();
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
        SingleEntryPoint.Instance.UiManager.MissionsScreenController.SetupIcons();
    }

    public void ResetPhone()
    {
        StopAllCoroutines();
        phoneButton.onClick.RemoveListener(AnswerPhone);
        phoneAnswered = false;
        exclamationIcon.transform.DOKill();
        exclamationIcon.SetActive(false);
    }

    public void PhoneRing()
    {
        if (phoneAnswered)
            return;

        phoneButton.onClick.AddListener(AnswerPhone);

        exclamationIcon.transform.localScale = Vector3.zero;
        exclamationIcon.SetActive(true);

        exclamationIcon.transform
            .DOScale(Vector3.one, animationDuration)
            .SetEase(Ease.OutBack);
    }

}