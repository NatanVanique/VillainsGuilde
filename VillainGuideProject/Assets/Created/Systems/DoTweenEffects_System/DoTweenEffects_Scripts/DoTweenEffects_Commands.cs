using UnityEngine;
using DG.Tweening;

public static class DoTweenEffects_Commands
{
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
}
