using UnityEngine;
using DG.Tweening;

public class MissionsScreenController : MonoBehaviour
{
    public static MissionsScreenController Instance { get; private set; }

    [SerializeField] private GameObject missionsScreen;
    [SerializeField] private float animationDuration = 0.5f;
    [SerializeField] private Ease animationEase = Ease.OutCubic;

    private void Awake()
    {
        Instance = this;
    }

    public void ShowMissionsScreen()
    {
        missionsScreen.SetActive(true);
        missionsScreen.transform.localScale = Vector3.zero;

        missionsScreen.transform
            .DOScale(Vector3.one, animationDuration)
            .SetEase(animationEase);
    }

    public void HideMissionsScreen()
    {
        missionsScreen.transform
            .DOScale(Vector3.zero, animationDuration)
            .SetEase(Ease.InCubic)
            .OnComplete(() =>
            {
                missionsScreen.SetActive(false);
            });
    }
}