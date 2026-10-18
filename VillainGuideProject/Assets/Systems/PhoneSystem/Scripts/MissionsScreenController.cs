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
        acceptButton.onClick.AddListener(AcceptButtonMethod);
    }

    private void AcceptButtonMethod()
    {
        List<QuestCardSelection> selectedQuests = SingleEntryPoint.Instance.ManagerManager.QuestSelectionManager.SelectedCards;
        OnQuestsAccepted?.Invoke(selectedQuests);

        HideMissionsScreen();
    }

    public void ShowMissionsScreen()
    {
        //Quests
        missionsScreen.DOKill();
        missionsScreen
            .DOScale(Vector3.one, animationDuration)
            .SetEase(animationEase);

        //Icones monstros
        monstersScreen.DOKill();
        monstersScreen
            .DOAnchorPosY(0f, 0.5f)
            .SetEase(Ease.InOutSine);
    }

    public void HideMissionsScreen()
    {
        if (closing)
            return;

        closing = true;

        //Quests
        missionsScreen.DOKill();
        missionsScreen
            .DOScale(Vector3.zero, animationDuration)
            .SetEase(Ease.InBack)
            .OnComplete(() =>
            {
                TransitionManager.Instance.ReloadSceneTransitionIn();
            });

        //Icones monstros
        monstersScreen.DOKill();
        monstersScreen
            .DOAnchorPosY(-255f, animationDuration)
            .SetEase(Ease.InOutSine);
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
        MonsterIconPrefab icon = Instantiate(monsterIconPrefab, monstersContent);
        icon.Setup(monsterInfo);
    }
}
