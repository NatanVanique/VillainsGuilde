using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using DG.Tweening;

public class PhoneController : MonoBehaviour
{
    public static PhoneController Instance { get; private set; }

    [Header("Components")]
    [SerializeField] private GameObject notificationObject;
    [SerializeField] private GameObject exclamationIcon;
    [SerializeField] private Sprite idleSprite;
    [SerializeField] private Sprite answeredSprite;
    [SerializeField] private Button phoneButton;
    [SerializeField] private Image phoneImage;

    private float animationDuration = 0.5f;
    private Sequence ringSequence;

    private enum PhoneState { Idle, Ringing, Answered }
    private PhoneState state;

    public void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

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

        //Icone exclamacao
        ringSequence?.Kill();
        exclamationIcon.transform.DOKill();
        exclamationIcon.transform.localRotation = Quaternion.identity;

        //Notificacao
        notificationObject.transform.DOKill();
        notificationObject.transform
            .DOScale(Vector3.zero, 0.1f)
            .SetEase(Ease.InBack)
            .OnComplete(() =>
            {
                notificationObject.SetActive(false);
            });

        MissionsScreenController missions = SingleEntryPoint.Instance.UiManager.MissionsScreenController;
        missions.SetupQuests();
        missions.SetupIcons();
        missions.ShowMissionsScreen();
    }

    public void ResetPhone()
    {
        StopAllCoroutines();

        state = PhoneState.Idle;
        phoneButton.interactable = false;
        phoneImage.overrideSprite = null;
        phoneImage.sprite = idleSprite;

        //Icone exclamacao
        ringSequence?.Kill();
        exclamationIcon.transform.DOKill();
        exclamationIcon.transform.localRotation = Quaternion.identity;

        //Notificacao
        notificationObject.transform.DOKill();
        notificationObject.SetActive(false);
    }

    public void PhoneRing()
    {
        if (state != PhoneState.Idle)
            return;

        StopAllCoroutines();

        state = PhoneState.Ringing;
        phoneButton.interactable = true;

        //Notificacao
        notificationObject.transform.DOKill();
        notificationObject.transform.localScale = Vector3.zero;
        notificationObject.SetActive(true);

        //Icone exclamacao
        exclamationIcon.transform.DOKill();
        exclamationIcon.transform.localRotation = Quaternion.identity;
        exclamationIcon.SetActive(true);

        notificationObject.transform
            .DOScale(Vector3.one, animationDuration)
            .SetEase(Ease.OutBack)
            .OnComplete(AnimateExclamation);
    }

    private void AnimateExclamation()
    {
        ringSequence?.Kill();

        exclamationIcon.transform.localRotation = Quaternion.identity;
        ringSequence = DOTween.Sequence();

        ringSequence
            .Append(exclamationIcon.transform.DOLocalRotate(
                new Vector3(0, 0, -20f), 0.08f))

            .Append(exclamationIcon.transform.DOLocalRotate(
                new Vector3(0, 0, 20f), 0.16f))

            .Append(exclamationIcon.transform.DOLocalRotate(
                new Vector3(0, 0, -20f), 0.16f))

            .Append(exclamationIcon.transform.DOLocalRotate(
                new Vector3(0, 0, 20f), 0.16f))

            .Append(exclamationIcon.transform.DOLocalRotate(
                Vector3.zero, 0.08f))

            .AppendInterval(0.4f)
            .SetLoops(-1, LoopType.Restart);
    }
}
