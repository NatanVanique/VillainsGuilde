public class MenuUiManager : UiButtonManager
{
    public void NewGame()
    {
        TransitionManager.Instance.LoadSceneAsync("02_GameScene");
    }
}
