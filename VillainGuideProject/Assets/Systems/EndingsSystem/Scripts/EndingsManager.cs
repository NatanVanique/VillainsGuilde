using UnityEngine;

public class EndingsManager : MonoBehaviour
{
    [SerializeField] private EndingScreenUi victoryScreen;
    [SerializeField] private EndingScreenUi defeatScreen;
    private bool ending;

    public static bool ShouldEndRun
    {
        get
        {
            ManagerManager managers = SingleEntryPoint.Instance.ManagerManager;
            return managers.HealthMan.CurrentHealth <= 0 || managers.DaySystem.HasCompletedAllDays;
        }
    }

    private void Awake()
    {
        victoryScreen.gameObject.SetActive(false);
        defeatScreen.gameObject.SetActive(false);
    }

    public void ShowEnding()
    {
        if (ending || !ShouldEndRun) return;
        ending = true;
        bool defeated = SingleEntryPoint.Instance.ManagerManager.HealthMan.CurrentHealth <= 0;
        EndingScreenUi screen = defeated ? defeatScreen : victoryScreen;
        void Prepare()
        {
            SingleEntryPoint.Instance.ManagerManager.HealthMan.ConsumePendingHealthLoss();
            PhoneController.Instance.ResetPhone();
            screen.Prepare();
        }
        TransitionManager.Instance.PlayTransition(Prepare, screen.AnimateEntrance);
    }
}
