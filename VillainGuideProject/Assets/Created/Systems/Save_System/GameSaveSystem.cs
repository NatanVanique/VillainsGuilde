using System;
using UnityEngine;
using UnityEngine.SceneManagement;

// Scene managers initialize their views and event listeners in Start.
[DefaultExecutionOrder(10000)]
[DisallowMultipleComponent]
public sealed class GameSaveSystem : MonoBehaviour
{
    private const string GameScene = "03_GameScene";
    private const string MenuScene = "01_MenuScene";
    private const float AutosaveSeconds = 60f;

    private static GameSaveSystem active;
    private static GameProgress pendingLoad;
    private static bool newGame;
    private static bool recoveredBackup;

    private Main_Referencer references;
    private bool ready;
    private bool requested;
    private double nextSave;

    public static bool HasSave => LocalSaveFile.TryLoad(out _, out _);
    public static bool HasExistingSave => LocalSaveFile.Exists;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    private static void ResetSession()
    {
        active = null;
        pendingLoad = null;
        newGame = false;
        recoveredBackup = false;
    }

    public static void StartNewGame()
    {
        pendingLoad = null;
        newGame = true;
        recoveredBackup = false;
        OpenGameScene();
    }

    public static void ContinueGame()
    {
        if (!LocalSaveFile.TryLoad(out pendingLoad, out recoveredBackup))
            return;

        newGame = false;
        OpenGameScene();
    }

    private static void OpenGameScene()
    {
        if (active != null)
            active.ready = false;

        Time.timeScale = 1f;
        SceneManager.LoadScene(GameScene);
    }

    private void Start()
    {
        // Opening the gameplay scene directly in the Editor is for testing scene defaults.
        if (!newGame && pendingLoad == null)
            return;

        references = GetComponent<Main_Referencer>();
        active = this;

        if (pendingLoad != null)
        {
            try
            {
                pendingLoad.gameplay.Restore(references);
                pendingLoad.quests.Restore(references);
                UnityEngine.Random.state = pendingLoad.randomState;
            }
            catch (Exception exception)
            {
                pendingLoad = null;
                ReturnToMenu("The saved game could not be restored. " + exception);
                return;
            }
            pendingLoad = null;
        }

        ready = true;
        if (newGame || recoveredBackup)
            SaveNow();
        else
            ScheduleNextSave();
    }

    private static void ReturnToMenu(string reason)
    {
        Debug.LogError(reason);
        Time.timeScale = 1f;
        SceneManager.LoadScene(MenuScene);
    }

    public static void RequestAutosave()
    {
        if (active != null && active.ready)
            active.requested = true;
    }

    private void LateUpdate()
    {
        if (ready && (requested || Time.realtimeSinceStartupAsDouble >= nextSave))
            SaveNow();
    }

    private void OnApplicationPause(bool paused)
    {
        if (paused) SaveNow();
    }

    private void OnApplicationFocus(bool focused)
    {
        if (!focused) SaveNow();
    }

    private void OnApplicationQuit() => SaveNow();

    private void OnDestroy()
    {
        if (active == this) active = null;
    }

    private void ScheduleNextSave()
    {
        requested = false;
        nextSave = Time.realtimeSinceStartupAsDouble + AutosaveSeconds;
    }

    private void SaveNow()
    {
        if (!ready) return;
        ScheduleNextSave();

        try
        {
            var progress = new GameProgress
            {
                gameplay = GameplaySaveData.Capture(references),
                quests = QuestSaveData.Capture(references),
                randomState = UnityEngine.Random.state
            };
            LocalSaveFile.Write(progress, newGame, recoveredBackup);
            newGame = false;
            recoveredBackup = false;
        }
        catch (Exception exception)
        {
            Debug.LogError("Could not save the game. " + exception);
        }
    }
}

[Serializable]
internal sealed class GameProgress
{
    public GameplaySaveData gameplay;
    public QuestSaveData quests;
    public UnityEngine.Random.State randomState;
}
