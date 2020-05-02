using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : Manager<GameManager>
{
    private string currentLevelName = "";
    public Enums.GameState currentGameState { get; private set; } = Enums.GameState.PREGAME;
    public Events.EventGameState OnGameStateChanged;

    public GameObject[] SystemPrefabs;
    private List<GameObject> instancedSystemPrefabs;

    private void Start()
    {
        instancedSystemPrefabs = new List<GameObject>();
        InstantiateSystemPrefabs();
    }

    void InstantiateSystemPrefabs()
    {
        GameObject prefabInstance;
        for (int i = 0; i < SystemPrefabs.Length; ++i)
        {
            prefabInstance = Instantiate(SystemPrefabs[i]);
            instancedSystemPrefabs.Add(prefabInstance);
        }
    }

    public void LoadLevel(string levelName)
    {
        AsyncOperation ao = SceneManager.LoadSceneAsync(levelName, LoadSceneMode.Single);
        if (ao == null)
        {
            Debug.LogError("[GameManager] Unable to load level " + levelName);
            return;
        }

        ao.completed += OnLoadOperationComplete;
        currentLevelName = levelName;
    }

    void OnLoadOperationComplete(AsyncOperation ao)
    {
        if (currentLevelName == "Main")
        {
            UIManager.Instance.HideMainMenu();
            UpdateState(Enums.GameState.RUNNING);
            UIManager.Instance.GetBoardManager();
            //Instance.InitSessions();
        }
        else if(currentLevelName == "Boot")
        {
            UIManager.Instance.ShowMainMenu();
        }
    }

    public void UpdateState(Enums.GameState state)
    {
        //Enums.GameState previousGameState = CurrentGameState;
        currentGameState = state;

        switch (currentGameState)
        {
            case Enums.GameState.PREGAME:
                Time.timeScale = 1.0f;
                break;

            case Enums.GameState.RUNNING:
                Time.timeScale = 1.0f;
                break;

            case Enums.GameState.PAUSED:
                Time.timeScale = 0.0f;
                break;

            default:
                break;
        }
        //OnGameStateChanged.Invoke(CurrentGameState, previousGameState);
    }

    public void TogglePause()
    {
        UpdateState(currentGameState == Enums.GameState.RUNNING 
            ? Enums.GameState.PAUSED : Enums.GameState.RUNNING);
    }
    public void RestartGame()
    {
        UpdateState(Enums.GameState.PREGAME);
    }
}
