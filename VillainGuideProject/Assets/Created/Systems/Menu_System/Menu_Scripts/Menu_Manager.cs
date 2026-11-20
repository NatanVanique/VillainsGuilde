
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;
using System.Collections.Generic;

public sealed class MenuManager : MonoBehaviour
{
    [Header("Hover Animation")]
    [SerializeField] private float ButtonBigXScale = 1.2f;
    [SerializeField] private float AnimationDuration = 0.2f;

    [Header("Hover Colors")]
    [SerializeField] private Color HoverColor = Color.blue;
    [SerializeField] private Color HoverOutlineColor = Color.black;
    [SerializeField] private Color HoverTextColor = Color.white;

    [Header("Text Material")]
    [SerializeField] private Material HoverTextMaterial;

    private Dictionary<GameObject, Color> OriginalButtonColors = new();
    private Dictionary<GameObject, Color> OriginalOutlineColors = new();
    private Dictionary<GameObject, Color> OriginalTextColors = new();
    private Dictionary<GameObject, Material> OriginalTextMaterials = new();

    public void OnPointerEnter(GameObject button)
    {
        SaveOriginalColors(button);

        ChangeScale(button, ButtonBigXScale);
        ChangeColor(button, HoverColor, HoverOutlineColor, HoverTextColor);
        ChangeTextMaterial(button, HoverTextMaterial);
    }

    public void OnPointerExit(GameObject button)
    {
        ChangeScale(button, 1f);

        ChangeColor(
            button,
            OriginalButtonColors[button],
            OriginalOutlineColors[button],
            OriginalTextColors[button]
        );

        ChangeTextMaterial(button, OriginalTextMaterials[button]);
    }

    private void SaveOriginalColors(GameObject button)
    {
        Image image = button.GetComponentInChildren<Image>();
        Outline outline = button.GetComponentInChildren<Outline>();
        TextMeshProUGUI text = button.GetComponentInChildren<TextMeshProUGUI>();

        if (!OriginalButtonColors.ContainsKey(button))
            OriginalButtonColors.Add(button, image.color);

        if (!OriginalOutlineColors.ContainsKey(button))
            OriginalOutlineColors.Add(button, outline.effectColor);

        if (!OriginalTextColors.ContainsKey(button))
            OriginalTextColors.Add(button, text.color);

        if (!OriginalTextMaterials.ContainsKey(button))
            OriginalTextMaterials.Add(button, text.fontMaterial);
    }

    private void ChangeScale(GameObject button, float scaleX)
    {
        button.transform.DOKill();

        button.transform.DOScaleX(scaleX, AnimationDuration)
            .SetEase(Ease.OutBack);
    }

    private void ChangeColor(GameObject button, Color buttonColor, Color outlineColor, Color textColor)
    {
        Image image = button.GetComponentInChildren<Image>();
        Outline outline = button.GetComponentInChildren<Outline>();
        TextMeshProUGUI text = button.GetComponentInChildren<TextMeshProUGUI>();

        image.DOKill();
        image.DOColor(buttonColor, AnimationDuration);

        outline.DOKill();
        outline.DOColor(outlineColor, AnimationDuration);

        text.DOKill();
        text.DOColor(textColor, AnimationDuration);
    }

    private void ChangeTextMaterial(GameObject button, Material material)
    {
        TextMeshProUGUI text = button.GetComponentInChildren<TextMeshProUGUI>();
        text.fontMaterial = material;
    }
}
