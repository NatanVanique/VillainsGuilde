using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
using System.Collections.Generic;

public class MissionsScreenController : MonoBehaviour
{
    [Header("Screen")]
    [SerializeField] private GameObject missionsScreen;
    [SerializeField] private Transform missionsTranform;
    [SerializeField] private Transform content;
    public Transform Content => content;

    [Header("Animation")]
    [SerializeField] private float animationDuration = 0.5f;
    [SerializeField] private Ease animationEase = Ease.OutCubic;

    [Header("Quests")]
    [SerializeField] private GameObject questCardPrefab;

    [Header("Ui Components")]
    [SerializeField] private Button acceptButton;

    private bool closing;


    public void Setup()
    {
        closing = false;
        missionsScreen.SetActive(false);
    }

    public void ShowMissionsScreen()
    {
        missionsScreen.SetActive(true);
        missionsScreen.transform.localScale = Vector3.zero;

        missionsScreen.transform
            .DOScale(Vector3.one, animationDuration)
            .SetEase(animationEase);
    }

    public void HideMissionsScreen()
    {
        if (closing || !missionsScreen.activeSelf)
            return;

        closing = true;
        missionsScreen.transform.DOKill();
        missionsScreen.transform
            .DOScale(Vector3.zero, animationDuration)
            .SetEase(Ease.InCubic)
            .OnComplete(() =>
            {
                missionsScreen.SetActive(false);
                TransitionManager.Instance.ReloadSceneTransitionIn();
            });

    }

    public void SetupQuests()
    {
        SingleEntryPoint.Instance.ManagerManager.QuestSelectionManager.ClearSelection();
        foreach (Transform child in content)
        {
            if (child.GetComponent<QuestCard>() != null)
            {
                child.gameObject.SetActive(false);
                Destroy(child.gameObject);
            }
        }

        List<QuestData> selectedQuests =
            SingleEntryPoint.Instance.ManagerManager.QuestSelector.SelectedQuests;

        foreach (var data in selectedQuests)
        {
            GameObject questCard = Instantiate(questCardPrefab, content);
            questCard.GetComponent<QuestCard>().Setup(data);
        }
    }
}
