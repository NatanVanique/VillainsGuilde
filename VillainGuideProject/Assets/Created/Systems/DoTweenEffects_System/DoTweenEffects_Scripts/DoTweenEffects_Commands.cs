using UnityEngine;
using DG.Tweening;

public static class DoTweenEffects_Commands
{
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
