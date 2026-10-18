using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class DuckClick : MonoBehaviour
{
    [Header("Reference")]
    [SerializeField] private GameObject DuckObject;
    private Button duckButton;

    [Header("Animation")]
    public float xSquash = 1.5f;
    public float ySquash = 0.7f;
    public float duration = 0.05f;

    private Vector3 originalScale;

    private void Awake()
    {
        originalScale = DuckObject.transform.localScale;

        duckButton = DuckObject.GetComponent<Button>();
        duckButton.onClick.AddListener(PlayDuckFeedback);
    }

    public void PlayDuckFeedback()
    {
        DuckObject.transform.DOKill();

        Vector3 squashScale = new Vector3(
            xSquash * originalScale.x,
            ySquash * originalScale.y,
            originalScale.z);

        DuckObject.transform
            .DOScale(squashScale, duration)
            .SetEase(Ease.OutQuad)
            .OnComplete(() =>
            {
                DuckObject.transform
                    .DOScale(originalScale, duration)
                    .SetEase(Ease.InQuad);
            });
    }
}