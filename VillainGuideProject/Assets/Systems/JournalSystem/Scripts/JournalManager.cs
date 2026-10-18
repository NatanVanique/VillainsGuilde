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
    private CanvasGroup canvasGroup;
    private Vector3 openScale;
    private bool closing;

    public void Setup()
    {
        journalInstance = Instantiate(journalPrefab, transform).GetComponent<JournalView>();
        journalUi = journalInstance.GetComponentInChildren<JournalUi>(true);
        canvasGroup = journalInstance.GetComponent<CanvasGroup>();
        if (canvasGroup == null)
            canvasGroup = journalInstance.gameObject.AddComponent<CanvasGroup>();
        openScale = journalInstance.AnimationTarget.localScale;
        if (openScale.sqrMagnitude < 0.001f)
            openScale = Vector3.one;
        journalInstance.CloseButton.onClick.AddListener(CloseJournal);
        if (journalUi != null)
            journalUi.DoneRequested += DeactiveJournal;
        journalInstance.gameObject.SetActive(false);
    }

    public void ActiveJournal()
    {
        closing = false;
        journalInstance.AnimationTarget.DOKill();
        canvasGroup.DOKill();
        journalInstance.AnimationTarget.localScale = openScale;
        canvasGroup.alpha = 0f;
        canvasGroup.blocksRaycasts = true;
        canvasGroup.interactable = true;
        journalInstance.gameObject.SetActive(true);
        if (journalUi != null)
            journalUi.Setup();
        canvasGroup.DOFade(1f, animationDuration).SetEase(Ease.OutCubic);
    }

    private void CloseJournal()
    {
        if (closing || !journalInstance.gameObject.activeSelf)
            return;
        if (journalUi != null)
            journalUi.FinishJournal();
        else
            DeactiveJournal();
    }

    public void DeactiveJournal()
    {
        if (!journalInstance.gameObject.activeSelf || closing)
            return;

        closing = true;
        journalInstance.AnimationTarget.DOKill();
        canvasGroup.DOKill();
        canvasGroup.blocksRaycasts = false;
        canvasGroup.interactable = false;
        canvasGroup.DOFade(0f, animationDuration)
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
            if (canvasGroup != null)
                canvasGroup.DOKill();
            journalInstance.CloseButton.onClick.RemoveListener(CloseJournal);
            if (journalUi != null)
                journalUi.DoneRequested -= DeactiveJournal;
        }
    }
}
