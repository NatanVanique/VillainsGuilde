using UnityEngine;
using DG.Tweening;

public class JournalManager : MonoBehaviour
{
    [SerializeField] private GameObject journalPrefab;

    [Header("Animation")]
    [SerializeField] private float animationDuration = 0.5f;

    [Header("Ui Components")]
    private JournalView journalInstance;
    private JournalUi journalUi;
    private Vector3 openScale;
    private bool closing;

    public void Setup()
    {
        journalInstance = Instantiate(journalPrefab, transform).GetComponent<JournalView>();
        journalUi = journalInstance.GetComponentInChildren<JournalUi>(true);
        openScale = journalInstance.AnimationTarget.localScale;
        if (openScale.sqrMagnitude < 0.001f)
            openScale = Vector3.one;
        journalInstance.CloseButton.onClick.AddListener(DeactiveJournal);
        if (journalUi != null)
            journalUi.DoneRequested += DeactiveJournal;
        journalInstance.gameObject.SetActive(false);
    }

    public void ActiveJournal()
    {
        closing = false;
        journalInstance.AnimationTarget.DOKill();
        journalInstance.AnimationTarget.localScale = Vector3.zero;
        journalInstance.gameObject.SetActive(true);
        if (journalUi != null)
            journalUi.Setup();
        journalInstance.AnimationTarget.DOScale(openScale, animationDuration).SetEase(Ease.OutCubic);
    }

    public void DeactiveJournal()
    {
        if (!journalInstance.gameObject.activeSelf || closing)
            return;

        closing = true;
        journalInstance.AnimationTarget.DOKill();
        journalInstance.AnimationTarget.DOScale(Vector3.zero, animationDuration)
            .SetEase(Ease.InCubic)
            .OnComplete(() =>
            {
                journalInstance.gameObject.SetActive(false);
                TransitionManager.Instance.ReloadSceneTransitionOut();
            });
    }

    private void OnDestroy()
    {
        if (journalInstance != null)
        {
            journalInstance.AnimationTarget.DOKill();
            journalInstance.CloseButton.onClick.RemoveListener(DeactiveJournal);
            if (journalUi != null)
                journalUi.DoneRequested -= DeactiveJournal;
        }
    }
}
