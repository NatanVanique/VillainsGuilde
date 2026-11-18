using UnityEngine;

public class InitialFadeEvents : MonoBehaviour
{
    [SerializeField] SplashTimerSystem splashTimer;

    public void StartLogos()
    {
        splashTimer.StartLogos();
    }
}
