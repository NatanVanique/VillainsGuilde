using System;
using DG.Tweening;
using UnityEngine;
using Random = UnityEngine.Random;

public sealed class QuestManager : MonoBehaviour
{
    private const float VoiceMailBackgroundHeight = 750f;
    private const float VoiceMailBackgroundTweenDuration = 0.35f;

    private int nextRouteIndex;

    public void Start()
    {
        Main_Referencer.Instance.visual.closeVoiceMailButton.onClick.AddListener(CloseVoiceMail);
    }

    public void OpenVoiceMail()
    {
        AnimateVoiceMailBackground(isOpening: true);
        Main_Referencer.Instance.visual.voiceMailCanvas_voiceMail.SetActive(true);
        Main_Referencer.Instance.manager.telephoneManager.SetPhoneState(isOpen: true);
        Main_Referencer.Instance.visual.telephoneAnimator_Telephone.SetBool("Ringing", false);
        Main_Referencer.Instance.value.telephoneRinging = false;
    }
    
    public void CloseVoiceMail()
    {
        AnimateVoiceMailBackground(isOpening: false);
        Main_Referencer.Instance.visual.voiceMailCanvas_voiceMail.SetActive(false);
        Main_Referencer.Instance.manager.telephoneManager.SetPhoneState(false);
    }

    private void AnimateVoiceMailBackground(bool isOpening)
    {
        RectTransform background = Main_Referencer.Instance.visual.voiceMailBackground.rectTransform;
        DOTween.Kill(background);

        float targetHeight = 0f;
        if (isOpening)
        {
            targetHeight = VoiceMailBackgroundHeight;
        }

        background.DOSizeDelta(
                new Vector2(background.sizeDelta.x, targetHeight),
                VoiceMailBackgroundTweenDuration
            )
            .SetEase(Ease.OutCubic);
    }
    
    public void AddQuest(QuestType type)
    {
        CreateQuest_Data data = null;

        if (type == QuestType.Single)
        {
            data = GetRandomSingleQuest();
        }
        else if (type == QuestType.Route)
        {
            data = GetNextUncalledRouteQuest();
        }

        GameObject mailObject = Instantiate(
            Main_Referencer.Instance.visual.questMail_VoiceMail,
            Main_Referencer.Instance.visual.questMailContainer_VoiceMail.transform
        );

        QuestMail_Info mailInfo = mailObject.GetComponent<QuestMail_Info>();

        mailInfo.setInfo(data);
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
    
    public enum QuestType
    {
        Single,
        Route
    }
}
