using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using DG.Tweening;

[RequireComponent(typeof(Button), typeof(Image))]
public class PhoneController : MonoBehaviour
{
    public static PhoneController Instance { get; private set; }

    [Header("Components")]
    [SerializeField] private GameObject exclamationIcon;
    [SerializeField] private Sprite idleSprite;
    [SerializeField] private Sprite answeredSprite;
    private Button phoneButton;
    private Image phoneImage;
    private float animationDuration = 0.5f;
    private enum PhoneState { Idle, Ringing, Answered }
    private PhoneState state;

    public void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
            return;
        }
        Instance = this;

        phoneButton = GetComponent<Button>();
        phoneImage = GetComponent<Image>();
        idleSprite = phoneImage.sprite;
        phoneButton.onClick.AddListener(AnswerPhone);
        ResetPhone();
    }
    private IEnumerator Start()
    {
        yield return new WaitForSeconds(3f);
        PhoneRing();
    }

    private void AnswerPhone()
    {
        if (state != PhoneState.Ringing)
            return;

        state = PhoneState.Answered;
        phoneButton.interactable = false;
        phoneImage.overrideSprite = null;
        phoneImage.sprite = answeredSprite;
        exclamationIcon.transform.DOKill();
        exclamationIcon.SetActive(false);

        SingleEntryPoint.Instance.UiManager.MissionsScreenController.ShowMissionsScreen();
        SingleEntryPoint.Instance.UiManager.MissionsScreenController.SetupQuests();
        SingleEntryPoint.Instance.UiManager.MissionsScreenController.SetupIcons();
    }

    public void ResetPhone()
    {
        StopAllCoroutines();
        state = PhoneState.Idle;
        phoneButton.interactable = false;
        phoneImage.overrideSprite = null;
        phoneImage.sprite = idleSprite;
        exclamationIcon.transform.DOKill();
        exclamationIcon.SetActive(false);
    }

    public void PhoneRing()
    {
        if (state != PhoneState.Idle)
            return;

        StopAllCoroutines();
        state = PhoneState.Ringing;
        phoneButton.interactable = true;

        exclamationIcon.transform.DOKill();
        exclamationIcon.transform.localScale = Vector3.zero;
        exclamationIcon.SetActive(true);

        exclamationIcon.transform
            .DOScale(Vector3.one, animationDuration)
            .SetEase(Ease.OutBack);
    }

}