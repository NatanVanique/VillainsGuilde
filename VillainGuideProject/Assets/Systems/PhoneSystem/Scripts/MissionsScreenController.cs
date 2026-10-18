using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MissionsScreenController : MonoBehaviour
{
    [Header("Screen")]
    [SerializeField] private RectTransform missionsScreen;
    [SerializeField] private RectTransform monstersScreen;
    [SerializeField] private Transform questsContent;
    [SerializeField] private Transform monstersContent;

    [Header("Animation")]
    [SerializeField] private float animationDuration = 0.9f;
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
        missionsScreen.DOKill();
        monstersScreen.DOKill();

        //Quests
        missionsScreen.gameObject.SetActive(true);
        missionsScreen.localScale = Vector3.zero;

        //Icones monstros
        monstersScreen.gameObject.SetActive(true);
        monstersScreen.anchoredPosition = new Vector2(monstersScreen.anchoredPosition.x, -255f);

        Canvas.ForceUpdateCanvases();
        SetButtons();
    }
    private void SetButtons()
    {
        acceptButton.onClick.RemoveListener(AcceptButtonMethod);
        acceptButton.onClick.AddListener(AcceptButtonMethod);
    }

    private void AcceptButtonMethod()
    {
        HideMissionsScreen();
    }

    public void ShowMissionsScreen()
    {
        //Quests
        missionsScreen.DOKill();
        missionsScreen.gameObject.SetActive(true);
        Canvas.ForceUpdateCanvases();
        LayoutRebuilder.ForceRebuildLayoutImmediate((RectTransform)questsContent);
        missionsScreen.DOScale(Vector3.one, animationDuration).SetEase(animationEase);
        questsContent.GetComponentInParent<HorizontalCarousel>(true)?.AnimateEntrance(animationDuration);

        //Icones monstros
        monstersScreen.DOKill();
        monstersScreen.gameObject.SetActive(true);
        monstersScreen.DOAnchorPosY(0f, 0.5f).SetEase(Ease.InOutSine);
    }

    public void HideMissionsScreen()
    {
        if (closing)
            return;

        closing = true;
        List<QuestCardSelection> selectedQuests = new List<QuestCardSelection>(
            SingleEntryPoint.Instance.ManagerManager.QuestSelectionManager.SelectedCards);
        OnQuestsAccepted?.Invoke(selectedQuests);

        //Quests
        missionsScreen.DOKill();
        missionsScreen.DOScale(Vector3.zero, animationDuration)
            .SetEase(Ease.InBack)
            .OnComplete(() => TransitionManager.Instance.ReloadSceneTransitionIn());

        //Icones monstros
        monstersScreen.DOKill();
        monstersScreen.DOAnchorPosY(-255f, animationDuration).SetEase(Ease.InOutSine);
    }

    public void SetupQuests()
    {
        ManagerManager managers = SingleEntryPoint.Instance.ManagerManager;
        managers.QuestSelector.EnsureCurrentDay(managers.DaySystem.CurrentDay);
        SingleEntryPoint.Instance.ManagerManager.QuestSelectionManager.ClearSelection();
        for (int i = questsContent.childCount - 1; i >= 0; i--)
        {
            Transform child = questsContent.GetChild(i);
            if (child.GetComponent<QuestCard>() != null)
            {
                child.gameObject.SetActive(false);
                child.SetParent(null, false);
                Destroy(child.gameObject);
            }
        }

        List<QuestData> selectedQuests =
            SingleEntryPoint.Instance.ManagerManager.QuestSelector.SelectedQuests;

        foreach (var data in selectedQuests)
        {
            if (data == null)
            {
                Debug.LogWarning("A mission for this day has a missing QuestData reference.", this);
                continue;
            }
            GameObject questCard = Instantiate(questCardPrefab, questsContent, false);
            ((RectTransform)questCard.transform).anchoredPosition3D = Vector3.zero;
            questCard.GetComponent<QuestCard>().Setup(data);
        }

        LayoutRebuilder.ForceRebuildLayoutImmediate((RectTransform)questsContent);
        questsContent.GetComponentInParent<HorizontalCarousel>(true)?.Refresh();
    }

    public void SetupIcons()
    {
        for (int i = monstersContent.childCount - 1; i >= 0; i--)
        {
            Transform child = monstersContent.GetChild(i);
            if (child.GetComponent<MonsterIconPrefab>() == null)
                continue;

            child.gameObject.SetActive(false);
            child.SetParent(null, false);
            Destroy(child.gameObject);
        }

        List<MonsterInfo> listOfMonsters = SingleEntryPoint.Instance.ManagerManager.MonsterMan.ListOfRegisteredMonsters;

        foreach (var monster in listOfMonsters)
        {
            InstantiateIcon(monster);
        }

        monstersContent.GetComponentInParent<HorizontalCarousel>(true)?.Refresh();
    }

    void InstantiateIcon(MonsterInfo monsterInfo)
    {
        MonsterIconPrefab icon = Instantiate(monsterIconPrefab, monstersContent);
        icon.Setup(monsterInfo);
    }

    private void OnDestroy()
    {
        if (acceptButton != null)
            acceptButton.onClick.RemoveListener(AcceptButtonMethod);
    }
}
