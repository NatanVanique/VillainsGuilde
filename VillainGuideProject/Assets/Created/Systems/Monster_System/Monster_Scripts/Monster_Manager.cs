using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization;

public sealed class MonsterManager : MonoBehaviour
{
    private readonly List<MonsterReferencer.Monsters> availableMonsters = new();
    private RoomMonsterInteraction[] roomMonsters;
    private MonsterReferencer.Monsters roomProfile;
    private string sendLabel;
    private QuestMail_Info selectedQuest;
    private LocalizedString displayedName;
    private int selectedIndex;

    private void Start()
    {
        VisualReferencer visual = Main_Referencer.Instance.visual;
        sendLabel = visual.sendButtonText_MonsterSelection.text;
        visual.previousButton_MonsterSelection.onClick.AddListener(SelectPrevious);
        visual.nextButton_MonsterSelection.onClick.AddListener(SelectNext);
        visual.sendButton_MonsterSelection.onClick.AddListener(SendMonster);
        visual.backButton_MonsterSelection.onClick.AddListener(CloseSelection);
        visual.vacationButton_MonsterSelection.onClick.AddListener(GiveVacation);
        roomMonsters = visual.roomMonsters.GetComponentsInChildren<RoomMonsterInteraction>(true);
        Main_Referencer.Instance.manager.dayCicleManager.DayChanged += RefreshRoster;
        RefreshRoster();
    }

    private void OnDestroy()
    {
        ClearDisplayedName();
        if (Main_Referencer.Instance != null)
            Main_Referencer.Instance.manager.dayCicleManager.DayChanged -= RefreshRoster;
    }

    public bool OpenSelection(QuestMail_Info quest)
    {
        CollectAvailableMonsters();
        if (availableMonsters.Count == 0)
        {
            return false;
        }

        selectedQuest = quest;
        roomProfile = null;
        selectedIndex = Mathf.Max(0, availableMonsters.IndexOf(quest.AssignedMonster));
        VisualReferencer visual = Main_Referencer.Instance.visual;
        visual.previousButton_MonsterSelection.gameObject.SetActive(true);
        visual.nextButton_MonsterSelection.gameObject.SetActive(true);
        visual.sendButton_MonsterSelection.gameObject.SetActive(true);
        visual.vacationControls_MonsterSelection.SetActive(false);
        visual.voiceMailCanvas_voiceMail.SetActive(false);
        RefreshSelection();
        visual.selectionCanvas_MonsterSelection.SetActive(true);
        return true;
    }

    internal void OpenRoomProfile(CreateMonster_Data profile)
    {
        Main_Referencer references = Main_Referencer.Instance;
        VisualReferencer visual = references.visual;
        if (visual.selectionCanvas_MonsterSelection.activeSelf || profile == null)
            return;

        MonsterReferencer.Monsters monster = FindMonster(profile);
        if (monster == null || !monster.IsAvailable(references.value.currentDay_HourValues))
            return;

        selectedQuest = null;
        roomProfile = monster;
        visual.previousButton_MonsterSelection.gameObject.SetActive(false);
        visual.nextButton_MonsterSelection.gameObject.SetActive(false);
        visual.sendButton_MonsterSelection.gameObject.SetActive(false);
        visual.vacationControls_MonsterSelection.SetActive(true);
        RefreshSelection();
        visual.selectionCanvas_MonsterSelection.SetActive(true);
    }

    internal MonsterReferencer.Monsters FindMonster(CreateMonster_Data profile)
    {
        return Array.Find(Main_Referencer.Instance.monster.monsters,
            monster => monster != null && monster.monstersProfile == profile);
    }

    internal void RefreshRoster()
    {
        Main_Referencer references = Main_Referencer.Instance;
        foreach (RoomMonsterInteraction view in roomMonsters)
        {
            MonsterReferencer.Monsters monster = FindMonster(view.Profile);
            view.SetAvailable(monster != null && monster.IsAvailable(references.value.currentDay_HourValues));
        }

        if (selectedQuest == null)
            return;

        MonsterReferencer.Monsters previousSelection = availableMonsters[selectedIndex];
        CollectAvailableMonsters();
        if (availableMonsters.Count == 0)
        {
            CloseSelection();
            return;
        }

        selectedIndex = Mathf.Max(0, availableMonsters.IndexOf(previousSelection));
        RefreshSelection();
    }

    private void CollectAvailableMonsters()
    {
        Main_Referencer references = Main_Referencer.Instance;
        availableMonsters.Clear();
        foreach (MonsterReferencer.Monsters monster in references.monster.monsters)
        {
            if (monster != null && monster.IsAvailable(references.value.currentDay_HourValues))
                availableMonsters.Add(monster);
        }
    }

    public void CloseSelection()
    {
        VisualReferencer visual = Main_Referencer.Instance.visual;
        if (!visual.selectionCanvas_MonsterSelection.activeSelf)
        {
            return;
        }

        ClearDisplayedName();
        visual.selectionCanvas_MonsterSelection.SetActive(false);
        if (roomProfile != null)
        {
            roomProfile = null;
            return;
        }

        selectedQuest = null;
        visual.voiceMailCanvas_voiceMail.SetActive(true);
        RoomManager roomManager = Main_Referencer.Instance.manager.roomManager;
        roomManager.GoToScreen(0);
        roomManager.ReactivateNavigationButtons();
    }

    private void SelectPrevious()
    {
        selectedIndex = (selectedIndex + availableMonsters.Count - 1) % availableMonsters.Count;
        RefreshSelection();
    }

    private void SelectNext()
    {
        selectedIndex = (selectedIndex + 1) % availableMonsters.Count;
        RefreshSelection();
    }

    private void GiveVacation()
    {
        Main_Referencer references = Main_Referencer.Instance;
        if (roomProfile == null || !roomProfile.IsAvailable(references.value.currentDay_HourValues) ||
            references.manager.questManager.IsMonsterOnMission(roomProfile))
            return;

        roomProfile.vacationReturnDay = references.value.currentDay_HourValues +
            Mathf.Max(1, references.value.vacationDays_MonsterValues);
        CloseSelection();
        RefreshRoster();
        GameSaveSystem.RequestAutosave();
    }

    private void SendMonster()
    {
        if (selectedQuest == null)
        {
            return;
        }

        MonsterReferencer.Monsters monster = availableMonsters[selectedIndex];
        if (UpdateSendButton(monster))
        {
            return;
        }

        if (selectedQuest.AssignedMonster == monster)
        {
            selectedQuest.AssignMonster(null);
            UpdateSendButton(monster);
            GameSaveSystem.RequestAutosave();
            return;
        }

        selectedQuest.AssignMonster(monster);
        CloseSelection();
        GameSaveSystem.RequestAutosave();
    }

    internal string GetSelectedMonsterId(QuestMail_Info mail)
    {
        return selectedQuest == mail && availableMonsters.Count > 0
            ? availableMonsters[selectedIndex].monstersProfile.SaveId : null;
    }

    internal void RestoreSelection(QuestMail_Info mail, MonsterReferencer.Monsters selected)
    {
        OpenSelection(mail);
        if (selectedQuest != mail)
            return;
        int index = availableMonsters.IndexOf(selected);
        if (index >= 0 && index != selectedIndex)
        {
            selectedIndex = index;
            RefreshSelection();
        }
    }

    private bool UpdateSendButton(MonsterReferencer.Monsters monster)
    {
        Main_Referencer references = Main_Referencer.Instance;
        QuestManager quests = references.manager.questManager;
        bool isInProgress = quests.IsMonsterInProgress(monster);
        bool assignedHere = selectedQuest != null && selectedQuest.AssignedMonster == monster;
        bool reservedElsewhere = !isInProgress && !assignedHere && quests.IsMonsterOnMission(monster);
        bool blocked = isInProgress || reservedElsewhere ||
            !monster.IsAvailable(references.value.currentDay_HourValues);

        references.visual.sendButton_MonsterSelection.interactable = !blocked;
        references.visual.sendButtonText_MonsterSelection.text = isInProgress ? "Já está em missão"
            : reservedElsewhere ? "Já selecionado"
            : assignedHere ? "Remove"
            : sendLabel;
        return blocked;
    }

    public void RefreshMissionAvailability()
    {
        if ((selectedQuest != null || roomProfile != null) &&
            Main_Referencer.Instance.visual.selectionCanvas_MonsterSelection.activeSelf)
        {
            RefreshAction();
        }
    }

    public void OnQuestMailResolved(QuestMail_Info quest)
    {
        if (selectedQuest == quest)
        {
            CloseSelection();
        }
        else
        {
            RefreshMissionAvailability();
        }
    }

    private void RefreshSelection()
    {
        MonsterReferencer.Monsters monster = roomProfile ?? availableMonsters[selectedIndex];
        CreateMonster_Data profile = monster.monstersProfile;
        RefreshAction();
        VisualReferencer visual = Main_Referencer.Instance.visual;
        bool canNavigate = roomProfile == null && availableMonsters.Count > 1;
        visual.previousButton_MonsterSelection.interactable = canNavigate;
        visual.nextButton_MonsterSelection.interactable = canNavigate;
        visual.monsterImage_MonsterSelection.sprite = profile.bigImage;
        visual.specialAbilityText_MonsterSelection.text = profile.mainPower.ToString();
        visual.levelText_MonsterSelection.text = $"Level: {monster.monsterLevel}";
        visual.levelSlider_MonsterSelection.SetValueWithoutNotify((float)monster.monsterLevel / MonsterReferencer.MaxMonsterLevel);
        visual.happinessSlider_MonsterSelection.SetValueWithoutNotify(monster.happiness);

        foreach (VisualReferencer.StatSlider stat in visual.statSliders_MonsterSelection)
        {
            stat.slider.SetValueWithoutNotify(monster.GetStatValue(stat.statType));
        }

        ClearDisplayedName();
        string profileName = profile.name;
        visual.monsterNameText_MonsterSelection.text =
            profileName.EndsWith("Data", StringComparison.Ordinal) ? profileName[..^4] : profileName;
        if (profile.monsterName != null && !profile.monsterName.IsEmpty)
        {
            displayedName = profile.monsterName;
            displayedName.StringChanged += SetMonsterName;
        }
    }

    private void RefreshAction()
    {
        if (roomProfile == null)
        {
            UpdateSendButton(availableMonsters[selectedIndex]);
            return;
        }

        Main_Referencer references = Main_Referencer.Instance;
        references.visual.vacationButton_MonsterSelection.interactable =
            !references.manager.questManager.IsMonsterOnMission(roomProfile);
    }

    private void SetMonsterName(string localizedName)
    {
        Main_Referencer.Instance.visual.monsterNameText_MonsterSelection.text = localizedName;
    }

    private void ClearDisplayedName()
    {
        if (displayedName != null)
        {
            displayedName.StringChanged -= SetMonsterName;
            displayedName = null;
        }
    }
}
