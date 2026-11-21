
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

    public void SaveOriginalValues(GameObject button)
    {
        Image image = button.GetComponentInChildren<Image>();
        Outline outline = button.GetComponentInChildren<Outline>();
        TextMeshProUGUI text = button.GetComponentInChildren<TextMeshProUGUI>();

        if (!OriginalScales.ContainsKey(button))
            OriginalScales.Add(button, button.transform.localScale);

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
        button.transform.DOKill();
        button.transform.DOScaleX(ButtonBigXScale, AnimationDuration).SetEase(Ease.OutBack);
    }

    public void RestoreScale(GameObject button)
    {
        button.transform.DOKill();
        button.transform.DOScaleX(OriginalScales[button].x, AnimationDuration).SetEase(Ease.OutBack);
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
