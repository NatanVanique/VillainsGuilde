using UnityEngine;
using Unity.Collections;
using UnityEngine.UI;
using DG.Tweening;

public class JournalManager : MonoBehaviour
{
    [SerializeField] GameObject journalPrefab;

    [Header("Animation")]
    [SerializeField] private float animationDuration = 0.5f;

    [Header("Ui Components")]
    private Button closeJournalButton;

    public void Setup()
    {
        Instantiate(journalPrefab);
        closeJournalButton = journalPrefab.GetComponentInChildren<Button>();
        closeJournalButton.onClick.AddListener(DeactiveJournal);
    }

    public void ActiveJournal()
    {
        journalPrefab.SetActive(true);
        journalPrefab.transform.localScale = Vector3.zero;
        journalPrefab.transform
            .DOScale(Vector3.one, animationDuration)
            .SetEase(Ease.OutCubic);
    }

    public void DeactiveJournal()
    {
        journalPrefab.transform.DOKill();
        journalPrefab.transform
            .DOScale(Vector3.zero, animationDuration)
            .SetEase(Ease.InCubic)
            .OnComplete(() =>
            {
                journalPrefab.SetActive(false);
            });

        new WaitForSeconds(3f);
        PhoneController.Instance.PhoneRing();
    }
}