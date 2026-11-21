using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class SplashTimerSystem : MonoBehaviour
{
    int splashDuration = 5;
    [SerializeField] GameObject bisc8Logo;
    //[SerializeField] GameObject asteraLogo;
    
    public void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        StartCoroutine(StartLogosSequence());
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
    
    private IEnumerator StartLogosSequence()
    {
        yield return new WaitForSeconds(1f);

        bisc8Logo.SetActive(true);

        yield return new WaitForSeconds(splashDuration);

        EndSplash();
    }

}