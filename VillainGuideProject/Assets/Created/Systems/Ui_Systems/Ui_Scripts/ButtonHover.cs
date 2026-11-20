using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;

public sealed class ButtonHover : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private float normalScale = 0.95f;
    private float hoverScale = 1.05f;
    private float duration = 0.22f;

    private Vector3 originalScale;
    private Tween scaleTween;

    private void Awake()
    {
        originalScale = transform.localScale;
        transform.localScale = originalScale * normalScale;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        AnimateScale(hoverScale, Ease.OutBack);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        AnimateScale(normalScale, Ease.OutCubic);
    }

    private void AnimateScale(float scale, Ease ease)
    {
        scaleTween?.Kill();

        scaleTween = transform.DOScale(
            originalScale * scale,
            duration
        ).SetEase(ease);
    }

    private void OnDisable()
    {
        scaleTween?.Kill();
        transform.localScale = originalScale * normalScale;
    }
}