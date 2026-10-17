using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class SplashTimerSystem : MonoBehaviour
{
    int splashDuration = 6;
    public void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        StartCoroutine(SplashTimer());
    }

    public void EndSplash()
    {
        TransitionManager.Instance.LoadSceneAsync("01_MenuScene");
    }

    public IEnumerator SplashTimer()
    {
        yield return new WaitForSeconds(splashDuration);
        EndSplash();
    }

}