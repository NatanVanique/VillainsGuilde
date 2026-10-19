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

    private GameObject reloadTransition;

    public void PlayTransition(System.Action onCovered, System.Action onRevealed)
    {
        if (!transitioning)
            StartCoroutine(ScreenTransitionRoutine(onCovered, onRevealed));
    }

    private IEnumerator ScreenTransitionRoutine(System.Action onCovered, System.Action onRevealed)
    {
        transitioning = true;
        GameObject transition = Instantiate(transitionPrefab, transform);
        Animator animator = transition.GetComponentInChildren<Animator>();
        animator.SetTrigger(inTrigger);
        yield return new WaitForSecondsRealtime(transitionDuration);
        onCovered.Invoke();
        animator.SetTrigger(outTrigger);
        yield return new WaitForSecondsRealtime(transitionDuration);
        Destroy(transition);
        transitioning = false;
        onRevealed.Invoke();
    }

    public void ReloadSceneTransitionIn()
    {
        if (!transitioning)
            StartCoroutine(ReloadInRoutine());
    }

    private IEnumerator ReloadInRoutine()
    {
        transitioning = true;
        reloadTransition = Instantiate(transitionPrefab, transform);
        reloadTransition.GetComponentInChildren<Animator>().SetTrigger(inTrigger);
        yield return new WaitForSeconds(transitionDuration);
        SingleEntryPoint.Instance.UiManager.JournalManager.HideAfterTransition();
        SingleEntryPoint.Instance.ManagerManager.DaySystem.AdvanceDay();
        SingleEntryPoint.Instance.UiManager.MissionsScreenController.Setup();
        yield return ReloadOutRoutine();
    }

    public void ReloadSceneTransitionOut()
    {
        if (reloadTransition != null)
            StartCoroutine(ReloadOutRoutine());
    }

    private IEnumerator ReloadOutRoutine()
    {
        PhoneController.Instance.ResetPhone();
        reloadTransition.GetComponentInChildren<Animator>().SetTrigger(outTrigger);
        yield return new WaitForSeconds(transitionDuration);
        Destroy(reloadTransition);
        reloadTransition = null;
        transitioning = false;
        SingleEntryPoint.Instance.UiManager.JournalManager.ActiveJournal();
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
