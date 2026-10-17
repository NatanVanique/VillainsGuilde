using UnityEngine;
using DG.Tweening;
using System.Collections.Generic;

public class MissionsScreenController : MonoBehaviour
{
    public static MissionsScreenController Instance { get; private set; }

    [Header("Screen")]
    [SerializeField] private GameObject missionsScreen;
    [SerializeField] private Transform missionsTranform;

    [Header("Animation")]
    [SerializeField] private float animationDuration = 0.5f;
    [SerializeField] private Ease animationEase = Ease.OutCubic;

    [Header("Quests")]
    [SerializeField] private GameObject questCardPrefab;

    private void Awake()
    {
        Instance = this;
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
        missionsScreen.transform
            .DOScale(Vector3.zero, animationDuration)
            .SetEase(Ease.InCubic)
            .OnComplete(() =>
            {
                missionsScreen.SetActive(false);
            });
    }

    public void SetupQuests()
    {
        List<QuestData> selectedQuests = QuestSelector.Instance.SelectedQuests;

        foreach(var data in selectedQuests)
        {
            GameObject questCard = Instantiate(questCardPrefab, missionsTranform);
            questCard.GetComponent<QuestCard>().Setup(data);
        }
    }
}