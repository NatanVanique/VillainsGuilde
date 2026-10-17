using UnityEngine;
using DG.Tweening;

public class JournalManager : MonoBehaviour
{
    [SerializeField] private GameObject journalPrefab;

    [Header("Animation")]
    [SerializeField] private float animationDuration = 0.5f;

    [Header("Ui Components")]
    private JournalView journalInstance;
    private Vector3 openScale;
    private bool closing;

    public void Setup()
    {
        journalInstance = Instantiate(journalPrefab, transform).GetComponent<JournalView>();
        openScale = journalInstance.AnimationTarget.localScale;
        journalInstance.CloseButton.onClick.AddListener(DeactiveJournal);
        journalInstance.gameObject.SetActive(false);
    }

    public void ActiveJournal()
    {
        closing = false;
        journalInstance.AnimationTarget.DOKill();
        journalInstance.AnimationTarget.localScale = Vector3.zero;
        journalInstance.gameObject.SetActive(true);
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
        }
    }
}