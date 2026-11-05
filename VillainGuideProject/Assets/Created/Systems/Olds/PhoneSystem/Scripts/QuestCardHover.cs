using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class QuestCardHover : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [Header("Animation Hover Parameters")]
    [SerializeField] private RectTransform hoverTarget;
    [SerializeField] private float animationDuration = 0.2f;

    private Button cardButton;
    private Tween hoverTween;

    private void Awake()
    {
        cardButton = GetComponent<Button>();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (cardButton.IsInteractable())
        {
            hoverTween.Kill();
            hoverTween = hoverTarget
                .DOScale(1f, animationDuration)
                .SetEase(Ease.OutBack);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        hoverTween.Kill();
        hoverTween = hoverTarget
            .DOScale(0.9f, animationDuration)
            .SetEase(Ease.OutCubic);
    }

    private void OnDisable()
    {
        hoverTween.Kill();
        hoverTarget.localScale = Vector3.one;
    }
}