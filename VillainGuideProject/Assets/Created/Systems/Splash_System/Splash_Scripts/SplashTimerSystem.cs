using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class SplashTimerSystem : MonoBehaviour
{
    int splashDuration = 5;
    [SerializeField] GameObject bisc8Logo;
    //[SerializeField] GameObject asteraLogo;

    public void StartLogos()
    {
        bisc8Logo.SetActive(true);
        //asteraLogo.SetActive(true);

        StartCoroutine(SplashTimer());
    }
    public void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
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