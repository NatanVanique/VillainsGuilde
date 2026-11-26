using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization;

public sealed class MonsterManager : MonoBehaviour
{
    private readonly List<MonsterReferencer.Monsters> availableMonsters = new();
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
    }

    private void OnDestroy()
    {
        ClearDisplayedName();
    }

    public void OpenSelection(QuestMail_Info quest)
    {
        Main_Referencer references = Main_Referencer.Instance;

        availableMonsters.Clear();
        foreach (MonsterReferencer.Monsters monster in references.monster.monsters)
        {
            if (monster != null && monster.contracted && monster.monstersProfile != null)
            {
                availableMonsters.Add(monster);
            }
        }

        if (availableMonsters.Count == 0)
        {
            return;
        }

        selectedQuest = quest;
        selectedIndex = Mathf.Max(0, availableMonsters.FindIndex(
            monster => monster == quest.AssignedMonster));
        bool canNavigate = availableMonsters.Count > 1;
        VisualReferencer visual = references.visual;
        visual.previousButton_MonsterSelection.interactable = canNavigate;
        visual.nextButton_MonsterSelection.interactable = canNavigate;

        visual.voiceMailCanvas_voiceMail.SetActive(false);
        RefreshSelection();
        visual.selectionCanvas_MonsterSelection.SetActive(true);
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
        bool blocked = isInProgress || reservedElsewhere;

        references.visual.sendButton_MonsterSelection.interactable = !blocked;
        references.visual.sendButtonText_MonsterSelection.text = isInProgress ? "Já está em missão"
            : reservedElsewhere ? "Já selecionado"
            : assignedHere ? "Remove"
            : sendLabel;
        return blocked;
    }

    public void RefreshMissionAvailability()
    {
        if (selectedQuest != null && Main_Referencer.Instance.visual.selectionCanvas_MonsterSelection.activeSelf)
        {
            UpdateSendButton(availableMonsters[selectedIndex]);
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
        MonsterReferencer.Monsters monster = availableMonsters[selectedIndex];
        CreateMonster_Data profile = monster.monstersProfile;
        UpdateSendButton(monster);
        VisualReferencer visual = Main_Referencer.Instance.visual;
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
