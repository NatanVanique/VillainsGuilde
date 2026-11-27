using System.Collections.Generic;
using UnityEngine;

public sealed class ContractManager : MonoBehaviour
{
    private readonly List<ContractMonsterCard> cards = new();
    private MonsterReferencer.Monsters selectedMonster;
    private VisualReferencer visual;

    internal int PendingChoices { get; private set; }
    internal bool IsChoosing => visual.lookingForJobCanvas_LookingForJob.gameObject.activeSelf ||
        visual.contractTooltipCanvas_ContractTolltip.gameObject.activeSelf;

    private void Start()
    {
        visual = Main_Referencer.Instance.visual;
        visual.hireButton_ContractTooltip.onClick.AddListener(HireSelected);
        visual.returnButton_ContractTooltip.onClick.AddListener(ShowPendingChoices);
        visual.infoButton_ContractTolltip.onClick.AddListener(ShowInfo);
        visual.statsButton_ContractTolltip.onClick.AddListener(ShowStats);
        SetScreens(false, false);
    }

    internal int AddRecruitChoices(int amount)
    {
        int candidates = 0;
        foreach (MonsterReferencer.Monsters monster in Main_Referencer.Instance.monster.monsters)
        {
            if (monster?.monstersProfile != null && !monster.contracted)
                candidates++;
        }

        int previous = PendingChoices;
        PendingChoices = Mathf.Min(previous + Mathf.Max(0, amount), candidates);
        return PendingChoices - previous;
    }

    internal void RestoreChoices(int amount, bool choosing)
    {
        PendingChoices = 0;
        AddRecruitChoices(amount);
        if (choosing)
            ShowPendingChoices();
        else
            SetScreens(false, false);
    }

    public void ShowPendingChoices()
    {
        selectedMonster = null;
        foreach (ContractMonsterCard card in cards)
        {
            card.gameObject.SetActive(false);
            Destroy(card.gameObject);
        }
        cards.Clear();

        if (PendingChoices > 0)
        {
            foreach (MonsterReferencer.Monsters monster in Main_Referencer.Instance.monster.monsters)
            {
                if (monster?.monstersProfile == null || monster.contracted)
                    continue;

                ContractMonsterCard card = Instantiate(visual.contractCard_LookingForJob,
                    visual.contractCardContainer_LookingForJob.transform).GetComponent<ContractMonsterCard>();
                card.Bind(monster.monstersProfile, () => SelectMonster(monster));
                cards.Add(card);
            }
        }

        SetScreens(cards.Count > 0, false);
    }

    private void SelectMonster(MonsterReferencer.Monsters monster)
    {
        selectedMonster = monster;
        CreateMonster_Data profile = monster.monstersProfile;
        visual.nameText_ContractTolltip.StringReference = profile.monsterName;
        visual.descriptionText_ContractTolltip.StringReference = profile.monsterDescription;
        visual.bigImage_ContractTolltip.sprite = profile.bigImage;
        visual.iconImage__ContractTolltip.sprite = profile.icon;
        foreach (VisualReferencer.StatSlider stat in visual.statsSlider_ContractTolltip)
            stat.slider.SetValueWithoutNotify(monster.GetStatValue(stat.statType));

        ShowInfo();
        SetScreens(false, true);
    }

    private void HireSelected()
    {
        if (selectedMonster == null || selectedMonster.contracted || PendingChoices <= 0)
            return;

        selectedMonster.contracted = true;
        selectedMonster.happiness = 1f;
        PendingChoices--;
        Main_Referencer.Instance.manager.monsterManager.RefreshRoster();
        ShowPendingChoices();
        GameSaveSystem.RequestAutosave();
    }

    private void ShowInfo()
    {
        visual.infoWindow_ContractTooltip.SetActive(true);
        visual.statsWindow_ContractTooltip.SetActive(false);
    }

    private void ShowStats()
    {
        visual.infoWindow_ContractTooltip.SetActive(false);
        visual.statsWindow_ContractTooltip.SetActive(true);
    }

    private void SetScreens(bool boardOpen, bool tooltipOpen)
    {
        SetScreen(visual.lookingForJobCanvas_LookingForJob, boardOpen);
        SetScreen(visual.contractTooltipCanvas_ContractTolltip, tooltipOpen);
    }

    private static void SetScreen(Canvas canvas, bool open)
    {
        canvas.enabled = open;
        canvas.gameObject.SetActive(open);
    }
}
