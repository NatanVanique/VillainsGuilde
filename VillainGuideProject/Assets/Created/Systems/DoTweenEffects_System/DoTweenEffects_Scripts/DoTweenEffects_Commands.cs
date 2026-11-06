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
}
