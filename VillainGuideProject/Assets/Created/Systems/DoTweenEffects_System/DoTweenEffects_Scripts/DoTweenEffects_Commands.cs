using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public static class DoTweenEffects_Commands
{

    public static Sequence DoTweenLoseHealth(Image image, Sprite lostSprite, Vector3 originalScale,
        float duration, float intensity)
    {
        Sequence sequence = DOTween.Sequence();
        sequence.Append(image.transform.DOScale(originalScale * 0.6f, duration * 0.5f).SetEase(Ease.InBack));
        sequence.AppendCallback(() => image.sprite = lostSprite);
        sequence.Append(image.transform.DOScale(originalScale, duration * 0.5f).SetEase(Ease.OutBack, intensity));
        return sequence;
    }

    public static void DoTweenHover(Transform target, float intensity, float duration)
    {
        target.DOKill();

        Vector3 originalScale = target.localScale;

        target.DOScale(originalScale * (1f + intensity), duration)
            .SetEase(Ease.OutBack);
    }

    public static void DoTweenHoverExit(Transform target, float duration)
    {
        target.DOKill();

        target.DOScale(Vector3.one, duration)
            .SetEase(Ease.OutQuad);
    }

    public static void DoTweenShowPanel(GameObject panel, float duration, float intensity)
    {
        panel.transform.DOComplete();
        panel.transform.DOKill();
        panel.SetActive(true);
        DoTweenPop(panel.transform, duration, intensity);
    }

    public static void DoTweenHidePanel(GameObject panel)
    {
        panel.transform.DOComplete();
        panel.transform.DOKill();
        panel.SetActive(false);
    }

    public static Tween DoTweenAnchorPosX(RectTransform target, float positionX, float duration, Ease ease)
    {
        target.DOKill();
        return target.DOAnchorPosX(positionX, duration).SetEase(ease);
    }

    public static void DoTweenAnchorPosYAndHeight(RectTransform target, float positionY, float height, float duration, Ease ease)
    {
        target.DOKill();
        target.DOAnchorPosY(positionY, duration).SetEase(ease);
        target.DOSizeDelta(new Vector2(target.sizeDelta.x, height), duration).SetEase(ease);
    }

    public static void DoTweenPop(Transform target, float duration, float intensity)
    {
        Vector3 originalScale = target.localScale;
        target.localScale = Vector3.zero;

        target.DOScale(originalScale, duration).SetEase(Ease.OutBack, intensity);
    }
    
    public static void DoTweenSpin(Transform target, float duration, float angle)
    {
        target.DOLocalRotate(new Vector3(0, 0, angle), duration).SetEase(Ease.InOutSine);
    }
    
    public static Tween DoTweenSquash(Transform target, float duration, float intensity)
    {
        target.DOComplete();
        target.DOKill();
        Vector3 squashObjOriginalScale = target.localScale;

        Vector3 squashScale = new Vector3(squashObjOriginalScale.x * (1f + intensity), 
            squashObjOriginalScale.y * (1f - intensity * 0.6f), squashObjOriginalScale.z);

        Vector3 stretchScale = new Vector3(squashObjOriginalScale.x * 0.9f, 
            squashObjOriginalScale.y * 1.12f, squashObjOriginalScale.z);

        Sequence sequence = DOTween.Sequence().SetTarget(target);
        
        sequence.Append(target.DOScale(squashScale, duration * 0.25f).SetEase(Ease.OutQuad));
        sequence.Append(target.DOScale(stretchScale, duration * 0.3f).SetEase(Ease.OutBack));
        sequence.Append(target.DOScale(squashObjOriginalScale, duration * 0.45f).SetEase(Ease.OutElastic, 0.5f, 0.5f));
        sequence.OnComplete(() => target.localScale = squashObjOriginalScale);
        
        return sequence;
    }
}
