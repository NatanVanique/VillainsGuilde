using DG.Tweening;
using TMPro;
using UnityEngine;

public sealed class JournalMonsterReward : MonoBehaviour
{
    [SerializeField] private UnityEngine.UI.Image icon;
    [SerializeField] private UnityEngine.UI.Slider progress;
    [SerializeField] private TextMeshProUGUI level;
    [SerializeField] private TextMeshProUGUI levelUp;

    private Sequence animationSequence;
    private Vector2 levelUpPosition;

    private void Awake()
    {
        progress.interactable = false;
        levelUpPosition = levelUp.rectTransform.anchoredPosition;
    }

    public void Play(Sprite sprite, int startLevel, int startXp, int endLevel, int endXp)
    {
        icon.sprite = sprite;
        icon.preserveAspect = true;
        level.text = startLevel.ToString();
        progress.SetValueWithoutNotify(Fill(startLevel, startXp));
        if (startLevel == MonsterReferencer.MaxMonsterLevel)
        {
            return;
        }

        float initialAlpha = levelUp.alpha;
        animationSequence = DOTween.Sequence().SetUpdate(true);

        for (int next = startLevel + 1; next <= endLevel; next++)
        {
            int displayedLevel = next;
            animationSequence.Append(progress.DOValue(1f, 0.55f).SetEase(Ease.OutCubic));
            animationSequence.AppendCallback(() =>
            {
                level.text = displayedLevel.ToString();
                progress.SetValueWithoutNotify(Fill(displayedLevel, 0));
            });
        }

        if (endLevel < MonsterReferencer.MaxMonsterLevel && endXp > 0)
        {
            animationSequence.Append(progress.DOValue(Fill(endLevel, endXp), 0.45f).SetEase(Ease.OutCubic));
        }

        if (endLevel > startLevel)
        {
            animationSequence.AppendCallback(() =>
            {
                levelUp.rectTransform.anchoredPosition = levelUpPosition;
                levelUp.alpha = 0f;
                levelUp.gameObject.SetActive(true);
            });
            animationSequence.Append(DOTween.To(() => levelUp.alpha, alpha => levelUp.alpha = alpha, initialAlpha, 0.18f));
            animationSequence.Join(levelUp.rectTransform.DOAnchorPosY(levelUpPosition.y + 12f, 0.18f));
            animationSequence.AppendInterval(0.55f);
            animationSequence.Append(DOTween.To(() => levelUp.alpha, alpha => levelUp.alpha = alpha, 0f, 0.35f));
            animationSequence.Join(levelUp.rectTransform.DOAnchorPosY(levelUpPosition.y + 22f, 0.35f));
            animationSequence.AppendCallback(() => levelUp.gameObject.SetActive(false));
        }
    }

    private static float Fill(int monsterLevel, int xp)
    {
        return monsterLevel >= MonsterReferencer.MaxMonsterLevel
            ? 1f
            : (float)xp / MonsterReferencer.ExperiencePerLevel;
    }

    private void OnDisable()
    {
        animationSequence?.Kill();
    }
}
