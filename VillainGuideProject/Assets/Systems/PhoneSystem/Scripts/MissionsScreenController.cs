using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
using System.Collections.Generic;
using System;

public class MissionsScreenController : MonoBehaviour
{
    [Header("Screen")]
    [SerializeField] private GameObject missionsScreen;
    [SerializeField] private Transform missionsTranform;
    [SerializeField] private Transform questsContent;
    [SerializeField] private Transform iconsContent;

    public Transform Content => questsContent;

    [Header("Animation")]
    [SerializeField] private float animationDuration = 0.5f;
    [SerializeField] private Ease animationEase = Ease.OutCubic;

    [Header("Quests")]
    [SerializeField] private GameObject questCardPrefab;
    [SerializeField] MonsterIconPrefab monsterIconPrefab;

    [Header("Ui Components")]
    [SerializeField] private Button acceptButton;

    public static event Action<List<QuestCardSelection>> OnQuestsAccepted;   

    private bool closing;


    public void Setup()
    {
        closing = false;
        missionsScreen.SetActive(false);

        SetButtons();
    }

    private void SetButtons()
    {
        acceptButton.onClick.AddListener(AcceptButtonMethod);
    }

    private void AcceptButtonMethod()
    {
        Debug.Log("AcceptButton Clicked");
        List<QuestCardSelection> selectedQuests = SingleEntryPoint.Instance.ManagerManager.QuestSelectionManager.SelectedCards;
        OnQuestsAccepted?.Invoke(selectedQuests);

        HideMissionsScreen();
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
        foreach (Transform child in questsContent)
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
            GameObject questCard = Instantiate(questCardPrefab, questsContent);
            questCard.GetComponent<QuestCard>().Setup(data);
        }
    }

    public void SetupIcons()
    {
        List<MonsterInfo> listOfMonsters = SingleEntryPoint.Instance.ManagerManager.MonsterMan.ListOfRegisteredMonsters;

        foreach (var monster in listOfMonsters)
        {
            InstantiateIcon(monster);
        }
    }

    void InstantiateIcon(MonsterInfo monsterInfo)
    {
        MonsterIconPrefab icon = Instantiate(monsterIconPrefab, iconsContent);
        icon.Setup(monsterInfo);
    }
}
