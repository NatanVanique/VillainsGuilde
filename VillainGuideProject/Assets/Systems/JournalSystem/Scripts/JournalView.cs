using UnityEngine;
using UnityEngine.UI;

public class JournalView : MonoBehaviour
{
    [SerializeField] private Transform animationTarget;
    [SerializeField] private Button closeButton;

    public Transform AnimationTarget => animationTarget;
    public Button CloseButton => closeButton;
}
