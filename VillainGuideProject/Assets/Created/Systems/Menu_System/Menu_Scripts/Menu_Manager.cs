
using System;
using UnityEngine;
using UnityEngine.SceneManagement;
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
    private Dictionary<GameObject, Vector3> OriginalScales = new();
    private Dictionary<GameObject, Vector2> OriginalAnchoredPositions = new();

    private void Start()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    public void SaveOriginalValues(GameObject button)
    {
        Image image = button.GetComponentInChildren<Image>();
        Outline outline = button.GetComponentInChildren<Outline>();
        TextMeshProUGUI text = button.GetComponentInChildren<TextMeshProUGUI>();

        if (!OriginalScales.ContainsKey(button))
            OriginalScales.Add(button, button.transform.localScale);

        RectTransform rectTransform = button.GetComponent<RectTransform>();
        if (rectTransform != null && !OriginalAnchoredPositions.ContainsKey(button))
            OriginalAnchoredPositions.Add(button, rectTransform.anchoredPosition);

        if (!OriginalButtonColors.ContainsKey(button))
            OriginalButtonColors.Add(button, image.color);

        if (!OriginalOutlineColors.ContainsKey(button))
            OriginalOutlineColors.Add(button, outline.effectColor);

        if (!OriginalTextColors.ContainsKey(button))
            OriginalTextColors.Add(button, text.color);

        if (!OriginalTextMaterials.ContainsKey(button))
            OriginalTextMaterials.Add(button, text.fontMaterial);
    }

    public void ChangeScale(GameObject button)
    {
        SaveOriginalValues(button);

        button.transform.DOKill();

        float targetScaleX = OriginalScales[button].x * ButtonBigXScale;
        button.transform.DOScaleX(targetScaleX, AnimationDuration).SetEase(Ease.OutBack);

        AnimateTowardsSibling(button, targetScaleX);
    }

    public void RestoreScale(GameObject button)
    {
        SaveOriginalValues(button);

        button.transform.DOKill();

        if (button.TryGetComponent(out RectTransform rectTransform))
        {
            rectTransform.DOAnchorPos(OriginalAnchoredPositions[button], AnimationDuration)
                .SetEase(Ease.OutCubic);
        }

        button.transform.DOScaleX(OriginalScales[button].x, AnimationDuration).SetEase(Ease.OutBack);
    }

    private void AnimateTowardsSibling(GameObject button, float targetScaleX)
    {
        if (!button.TryGetComponent(out RectTransform rectTransform) || rectTransform.parent == null)
            return;

        RectTransform sibling = null;
        int buttonCount = 0;
        foreach (Transform child in rectTransform.parent)
        {
            if (!child.TryGetComponent<Button>(out _) || !child.TryGetComponent(out RectTransform childRect))
                continue;

            buttonCount++;
            if (child != rectTransform)
            {
                sibling = childRect;
            }
        }

        if (buttonCount != 2 || sibling == null)
            return;

        bool isLeftHalf = rectTransform.anchorMin.x < sibling.anchorMin.x;
        float scaleIncrease = targetScaleX / OriginalScales[button].x - 1f;
        float offset = rectTransform.rect.width * scaleIncrease * 0.5f;
        Vector2 targetPosition = OriginalAnchoredPositions[button] + Vector2.right * (isLeftHalf ? offset : -offset);

        rectTransform.DOAnchorPos(targetPosition, AnimationDuration).SetEase(Ease.OutBack);
    }

    public void ChangeColor(GameObject button)
    {
        SaveOriginalValues(button);

        Image image = button.GetComponentInChildren<Image>();
        Outline outline = button.GetComponentInChildren<Outline>();
        TextMeshProUGUI text = button.GetComponentInChildren<TextMeshProUGUI>();

        image.DOKill();
        image.DOColor(HoverColor, AnimationDuration);

        outline.effectColor = HoverOutlineColor;

        text.DOKill();
        text.DOColor(HoverTextColor, AnimationDuration);
    }

    public void RestoreColor(GameObject button)
    {
        Image image = button.GetComponentInChildren<Image>();
        Outline outline = button.GetComponentInChildren<Outline>();
        TextMeshProUGUI text = button.GetComponentInChildren<TextMeshProUGUI>();

        image.DOKill();
        image.DOColor(OriginalButtonColors[button], AnimationDuration);

        outline.effectColor = OriginalOutlineColors[button];

        text.DOKill();
        text.DOColor(OriginalTextColors[button], AnimationDuration);
    }

    public void ChangeTextMaterial(GameObject button)
    {
        SaveOriginalValues(button);
        button.GetComponentInChildren<TextMeshProUGUI>().fontMaterial = HoverTextMaterial;
    }

    public void RestoreTextMaterial(GameObject button)
    {
        button.GetComponentInChildren<TextMeshProUGUI>().fontMaterial = OriginalTextMaterials[button];
    }

    public void NewGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public void ContinueGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 2);
    }

    public void ExitGame()
    {
        Application.Quit();
    }
}
