using System;
using DG.Tweening;
using UnityEngine;
using Random = UnityEngine.Random;

public sealed class QuestManager : MonoBehaviour
{
    private const float VoiceMailBackgroundHeight = 750f;
    private const float VoiceMailBackgroundTweenDuration = 0.35f;
    private const float InitialVisionFadeDuration = 0.2f;

    private int nextRouteIndex;

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
    
    public void AddQuest(CreateQuest_Data.QuestType type)
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
}
