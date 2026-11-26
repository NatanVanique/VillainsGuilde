using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using Random = UnityEngine.Random;

public sealed class QuestManager : MonoBehaviour
{
    private const float VoiceMailBackgroundHeight = 750f;
    private const float VoiceMailBackgroundTweenDuration = 0.35f;
    private const float InitialVisionFadeDuration = 0.2f;

    private int nextRouteIndex;
    private readonly List<QuestMail_Info> questMails = new();
    private readonly HashSet<MonsterReferencer.Monsters> monstersOnTimedMissions = new();
    private readonly List<TimedMission> timedMissions = new();

    private sealed class TimedMission
    {
        public CreateQuest_Data quest;
        public MonsterReferencer.Monsters monster;
        public bool accepted;
        public float deadline;
    }

    public void StartResponseTimer(QuestMail_Info mail)
    {
        StartCoroutine(WatchResponseTimer(mail));
    }

    private static IEnumerator WatchResponseTimer(QuestMail_Info mail)
    {
        while (mail != null && mail.UpdateResponseTimer())
        {
            yield return null;
        }
    }

    public void ResolveQuest(CreateQuest_Data quest, MonsterReferencer.Monsters monster, bool accepted)
    {
        StartMission(quest, monster, accepted, quest.timeToDoneSeconds);
        GameSaveSystem.RequestAutosave();
    }

    private void StartMission(CreateQuest_Data quest, MonsterReferencer.Monsters monster, bool accepted, float remaining)
    {
        if (!accepted && quest.type == CreateQuest_Data.QuestType.Route)
        {
            Main_Referencer.Instance.manager.journalManager.RegisterNew(
                quest, monster, isWinned: false, attempted: false);
            return;
        }

        if (accepted && !monstersOnTimedMissions.Add(monster))
            return;

        var mission = new TimedMission { quest = quest, monster = monster, accepted = accepted,
            deadline = Time.time + remaining };
        timedMissions.Add(mission);
        StartCoroutine(CompleteMissionAfterDelay(mission));
    }

    private IEnumerator CompleteMissionAfterDelay(TimedMission mission)
    {
        float remaining = mission.deadline - Time.time;
        if (remaining > 0f)
            yield return new WaitForSeconds(remaining);

        ManagerReferencer managers = Main_Referencer.Instance.manager;
        timedMissions.Remove(mission);
        bool succeeded = mission.accepted && MeetsQuestRequirements(mission.quest, mission.monster);
        if (mission.accepted)
        {
            monstersOnTimedMissions.Remove(mission.monster);
            managers.monsterManager.RefreshMissionAvailability();
        }

        managers.journalManager.RegisterNew(mission.quest, mission.monster, succeeded, attempted: mission.accepted);
        GameSaveSystem.RequestAutosave();
    }

    public void RefreshDiscoveredResults()
    {
        Main_Referencer.Instance.visual.questMailContainer_VoiceMail.GetComponentsInChildren(true, questMails);
        foreach (QuestMail_Info mail in questMails)
        {
            mail.RefreshDiscoveredResults();
        }
    }

    private static bool MeetsQuestRequirements(CreateQuest_Data quest, MonsterReferencer.Monsters monster)
    {
        if (monster == null)
        {
            return false;
        }

        var passed = new HashSet<CreateMonster_Data.StatsTypes>();
        if (quest.statusRequirements != null)
        {
            foreach (CreateQuest_Data.statusRequirementsValues requirement in quest.statusRequirements)
            {
                if (requirement != null && monster.GetStatValue(requirement.requerimenttype) >= requirement.requerimentValue)
                {
                    passed.Add(requirement.requerimenttype);
                }
            }
        }

        return passed.Count >= quest.statsToWin;
    }

    public bool IsMonsterInProgress(MonsterReferencer.Monsters monster)
    {
        return monstersOnTimedMissions.Contains(monster);
    }

    public bool IsMonsterOnMission(MonsterReferencer.Monsters monster)
    {
        if (IsMonsterInProgress(monster))
        {
            return true;
        }

        Main_Referencer.Instance.visual.questMailContainer_VoiceMail
            .GetComponentsInChildren(true, questMails);

        foreach (QuestMail_Info mail in questMails)
        {
            if (mail.AssignedMonster == monster)
            {
                return true;
            }
        }

        return false;
    }

    public void Start()
    {
        Main_Referencer.Instance.visual.closeVoiceMailButton.onClick.AddListener(CloseVoiceMail);
    }

    public void OpenVoiceMail()
    {
        VisualReferencer visual = Main_Referencer.Instance.visual;
        visual.voiceMailCanvas_voiceMail.SetActive(true);
        CanvasGroup initialVisionGroup = visual.firstVision.GetComponent<CanvasGroup>();
        DOTween.Kill(initialVisionGroup);
        initialVisionGroup.alpha = 0f;
        visual.firstVision.SetActive(true);
        AnimateVoiceMailBackground(isOpening: true, initialVisionGroup: initialVisionGroup);
        Main_Referencer.Instance.manager.telephoneManager.SetPhoneState(isOpen: true);
        Main_Referencer.Instance.visual.telephoneAnimator_Telephone.SetBool("Ringing", false);
        Main_Referencer.Instance.value.telephoneRinging = false;
    }
    
    public void CloseVoiceMail()
    {
        Main_Referencer.Instance.manager.monsterManager.CloseSelection();
        VisualReferencer visual = Main_Referencer.Instance.visual;
        CanvasGroup initialVisionGroup = visual.firstVision.GetComponent<CanvasGroup>();
        DOTween.Kill(initialVisionGroup);
        initialVisionGroup.DOFade(0f, InitialVisionFadeDuration)
            .OnComplete(() =>
            {
                visual.firstVision.SetActive(false);
                AnimateVoiceMailBackground(isOpening: false, deactivateCanvasOnComplete: true);
            });
        Main_Referencer.Instance.manager.telephoneManager.SetPhoneState(false);
    }

    private void AnimateVoiceMailBackground(
        bool isOpening,
        CanvasGroup initialVisionGroup = null,
        bool deactivateCanvasOnComplete = false
    )
    {
        RectTransform background = Main_Referencer.Instance.visual.voiceMailBackground.rectTransform;
        DOTween.Kill(background);

        float startHeight = VoiceMailBackgroundHeight;
        float targetHeight = 0f;
        if (isOpening)
        {
            startHeight = 0f;
            targetHeight = VoiceMailBackgroundHeight;
        }

        Vector2 size = background.sizeDelta;
        background.sizeDelta = new Vector2(size.x, startHeight);
        Sequence animation = DOTween.Sequence();
        animation.Append(
            background.DOSizeDelta(
                    new Vector2(background.sizeDelta.x, targetHeight),
                    VoiceMailBackgroundTweenDuration
                )
                .SetEase(Ease.OutCubic)
        );

        if (isOpening && initialVisionGroup != null)
        {
            animation.Insert(
                VoiceMailBackgroundTweenDuration * 0.5f,
                initialVisionGroup.DOFade(1f, InitialVisionFadeDuration)
            );
        }

        if (deactivateCanvasOnComplete)
        {
            animation.OnComplete(() =>
                Main_Referencer.Instance.visual.voiceMailCanvas_voiceMail.SetActive(false)
            );
        }

    }
    
    public bool AddQuest(CreateQuest_Data.QuestType type)
    {
        CreateQuest_Data data = null;

        if (type == CreateQuest_Data.QuestType.Single)
        {
            data = GetRandomSingleQuest();
        }
        else if (type == CreateQuest_Data.QuestType.Route)
        {
            data = GetNextUncalledRouteQuest();
        }

        if (data == null)
        {
            return false;
        }

        CreateMail().setInfo(data);
        GameSaveSystem.RequestAutosave();
        return true;
    }

    private QuestMail_Info CreateMail()
    {
        return Instantiate(
            Main_Referencer.Instance.visual.questMail_VoiceMail,
            Main_Referencer.Instance.visual.questMailContainer_VoiceMail.transform
        ).GetComponent<QuestMail_Info>();
    }

    internal void CaptureProgress(QuestSaveData data)
    {
        Main_Referencer refs = Main_Referencer.Instance;
        data.nextRouteIndex = nextRouteIndex;
        data.voiceMailOpen = refs.visual.openPhone.activeSelf && !refs.visual.selectionCanvas_MonsterSelection.activeSelf;
        refs.visual.questMailContainer_VoiceMail.GetComponentsInChildren(true, questMails);
        foreach (var mail in questMails)
        {
            var record = mail.CaptureProgress();
            if (record != null)
            {
                record.selectedMonsterId = refs.manager.monsterManager.GetSelectedMonsterId(mail);
                data.mails.Add(record);
            }
        }
        foreach (var mission in timedMissions)
        {
            data.missions.Add(new QuestSaveData.Mission
            {
                questId = mission.quest.SaveId,
                monsterId = mission.monster?.monstersProfile.SaveId,
                accepted = mission.accepted,
                remaining = Mathf.Max(0f, mission.deadline - Time.time)
            });
        }
    }

    internal void RestoreProgress(QuestSaveData data, QuestSaveData.Catalog catalog)
    {
        Main_Referencer refs = Main_Referencer.Instance;
        nextRouteIndex = data.nextRouteIndex;
        foreach (var mission in data.missions)
            StartMission(catalog.Quest(mission.questId), catalog.Monster(mission.monsterId), mission.accepted, mission.remaining);

        VisualReferencer visual = refs.visual;
        visual.voiceMailCanvas_voiceMail.SetActive(data.voiceMailOpen);
        visual.firstVision.SetActive(true);
        visual.firstVision.GetComponent<CanvasGroup>().alpha = 1f;
        visual.infoVision.SetActive(false);
        QuestMail_Info selectedMail = null;
        MonsterReferencer.Monsters selection = null;
        foreach (var record in data.mails)
        {
            QuestMail_Info mail = CreateMail();
            mail.RestoreProgress(catalog.Quest(record.questId), catalog.Monster(record.assignedMonsterId), record);
            if (!string.IsNullOrEmpty(record.selectedMonsterId) && !mail.IsResolved)
            {
                selectedMail = mail;
                selection = catalog.Monster(record.selectedMonsterId);
            }
        }
        if (selectedMail != null)
            refs.manager.monsterManager.RestoreSelection(selectedMail, selection);
    }

    private CreateQuest_Data GetRandomSingleQuest()
    {
        QuestReferencer.QuestReference[] singleQuests = Main_Referencer.Instance.quest.singleQuests;
        if (singleQuests == null || singleQuests.Length == 0)
        {
            return null;
        }

        return singleQuests[Random.Range(0, singleQuests.Length)].quest;
    }

    private CreateQuest_Data GetNextUncalledRouteQuest()
    {
        QuestReferencer.QuestRoute[] routes = Main_Referencer.Instance.quest.routeQuests;

        while (routes != null && nextRouteIndex < routes.Length)
        {
            QuestReferencer.QuestRoute route = routes[nextRouteIndex++];
            if (route.quests == null)
            {
                continue;
            }

            foreach (QuestReferencer.QuestReference quest in route.quests)
            {
                if (quest != null && quest.quest != null)
                {
                    return quest.quest;
                }
            }
        }
        
        return null;
    }
}
