using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class TransitionManager : MonoBehaviour
{
    public static TransitionManager Instance;

    [SerializeField] GameObject transitionPrefab;
    [SerializeField] string inTrigger = "InTransition";
    [SerializeField] string outTrigger = "OutTransition";
    float transitionDuration = 2f;

    bool transitioning;

    void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void LoadSceneAsync(string sceneName)
    {
        if (!transitioning)
            StartCoroutine(TransitionRoutine(sceneName));
    }

    IEnumerator TransitionRoutine(string sceneName)
    {
        transitioning = true;

        Scene oldScene = SceneManager.GetActiveScene();

        GameObject transition = Instantiate(transitionPrefab);
        DontDestroyOnLoad(transition);
        Animator animator = transition.GetComponentInChildren<Animator>();
        animator.SetTrigger(inTrigger);
        yield return new WaitForSeconds(transitionDuration);

        yield return SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);

        Scene newScene = SceneManager.GetSceneByName(sceneName);
        SceneManager.SetActiveScene(newScene);

        yield return SceneManager.UnloadSceneAsync(oldScene);

        animator.SetTrigger(outTrigger);
        yield return new WaitForSeconds(transitionDuration);

        Destroy(transition);
        transitioning = false;
    }
}