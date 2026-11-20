using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;

public sealed class ButtonHover : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private float intensity = 0.08f;
    private float duration = 0.15f;

    private Vector3 originalScale;

    private void Awake()
    {
        originalScale = transform.localScale;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        DoTweenEffects_Commands.DoTweenHover(transform, intensity, duration);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        transform.DOKill();
        transform.DOScale(originalScale, duration).SetEase(Ease.OutQuad);
    }
}