using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class SplashTimerSystem : MonoBehaviour
{
    [SerializeField] private Animator bisc8Logo;
    [SerializeField] private Transform asteraLogo;
    [SerializeField] private float splashDuration = 6f;

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
        float startTime = Time.time;
        yield return null;
        yield return new WaitUntil(() => bisc8Logo.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1f);

        asteraLogo.gameObject.SetActive(true);
        Animator asteraAnimator = asteraLogo.GetComponent<Animator>();
        yield return null;
        yield return new WaitUntil(() => asteraAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1f);
        float remainingTime = splashDuration - (Time.time - startTime);
        if (remainingTime > 0f)
            yield return new WaitForSeconds(remainingTime);
        EndSplash();
    }

}
