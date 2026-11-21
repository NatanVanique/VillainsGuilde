using UnityEngine;
using UnityEngine.UI;

public sealed class DuckManager : MonoBehaviour
{
    [Header("Animation")]
    [Min(0f)] public float intensity = 0.5f;
    [Min(0f)] public float duration = 0.2f;
    
    private Transform duckTransform;
    private Button duckButton;

    private void Start()
    {
        VisualReferencer visual = Main_Referencer.Instance.visual;
        duckTransform = visual.duckObject.transform;
        duckButton = visual.duckButton;
        duckButton.onClick.AddListener(PlayDuckFeedback);
    }

    public void PlayDuckFeedback()
    {
        DoTweenEffects_Commands.DoTweenSquash(duckTransform, duration, intensity);
    }
    
}