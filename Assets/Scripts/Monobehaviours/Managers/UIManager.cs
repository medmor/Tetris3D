using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : Manager<UIManager>
{
    //GameBoard boardManager = default; //Instantiated when the level load in the game manager


    [Header("Boot Menu")]
    [SerializeField] private GameObject mainMenu = default;
    [SerializeField] private Camera dummyCammera = default;
    [SerializeField] private Button normalModeButton = default;
    [SerializeField] private Button hardModeButton = default;
    [SerializeField] private Button settings = default;

    [Header("Pause Menu")]
    [SerializeField] private GameObject pauseMenu = default;
    [SerializeField] private Button pause = default;
    [SerializeField] private Button resume = default;
    [SerializeField] private Button boot = default;
    [SerializeField] private Button quite = default;

    [Header("Settings Menu")]
    [SerializeField] private GameObject settingsMenu = default;
    [SerializeField] private Button closeSettings = default;
    [SerializeField] private TextMeshProUGUI bestScoreNormalSettings = default;
    [SerializeField] private TextMeshProUGUI levelNormalSettings = default;
    [SerializeField] private Button resetNormalSettings = default;
    [SerializeField] private TextMeshProUGUI bestScoreHardSettings = default;
    [SerializeField] private TextMeshProUGUI levelHardSettings = default;
    [SerializeField] private Button resetHardSettings = default;

    [Header("Game Info")]
    [SerializeField] private GameObject gameInfo = default;
    [SerializeField] private TextMeshProUGUI score = default;
    [SerializeField] private TextMeshProUGUI level = default;
    [SerializeField] private TextMeshProUGUI lines = default;

    [Header("Controls")]
    [SerializeField] private GameObject controls = default;
    [SerializeField] private Button leftButton = default;
    [SerializeField] private Button rightButton = default;
    [SerializeField] private Button downButton = default;
    [SerializeField] private Button rotateRightButton = default;
    [SerializeField] private Button rotateLeftButton = default;
    [SerializeField] private Button dropButton = default;

    [Header("GameOver Menu")]
    [SerializeField] private GameObject gameOverMenu = default;
    [SerializeField] private TextMeshProUGUI finalScore = default;
    [SerializeField] private TextMeshProUGUI bestScore = default;
    [SerializeField] private Button restart = default;
    [SerializeField] private Button quite2 = default;
    [SerializeField] private Button boot2 = default;

    private void Start()
    {
        bootMenuListeners();
        pauseListeners();
        settingsListeners();
        controlsListeners();
        gameOverListeners();
    }

    #region Listeners
    private void bootMenuListeners()
    {
        normalModeButton
            .onClick.AddListener(() =>
            {
                HideMainMenu();
                GameManager.Instance.GameMode = Enums.GameMode.NORMAL;
            });

        hardModeButton
            .onClick.AddListener(() =>
            {
                HideMainMenu();
                GameManager.Instance.GameMode = Enums.GameMode.HARD;
            });

        settings
            .onClick.AddListener(() =>
            {
                pauseMenu.SetActive(false);
                settingsMenu.SetActive(true);
                bestScoreNormalSettings.text = GameSaveManager.Instance.GetBestScore(
                    Enums.GameMode.NORMAL).ToString();
                levelNormalSettings.text = GameSaveManager.Instance.GetLevel(
                    Enums.GameMode.NORMAL).ToString();
                bestScoreHardSettings.text = GameSaveManager.Instance.GetBestScore(
                    Enums.GameMode.HARD).ToString();
                levelHardSettings.text = GameSaveManager.Instance.GetLevel(
                    Enums.GameMode.HARD).ToString();

            });
    }

    private void pauseListeners()
    {
        pause
            .onClick.AddListener(() =>
            {
                GameManager.Instance.TogglePause();
                pauseMenu.gameObject.SetActive(true);
                pause.gameObject.SetActive(false);
            });

        resume
            .onClick.AddListener(() =>
            {
                GameManager.Instance.TogglePause();
                pauseMenu.gameObject.SetActive(false);
                pause.gameObject.SetActive(true);
            });

        boot
            .onClick.AddListener(() =>
            {
                pauseMenu.gameObject.SetActive(false);
                GameManager.Instance.LoadLevel("Boot");
            });

        quite
            .onClick.AddListener(() =>
            {
                Application.Quit();
            });
    }

    private void settingsListeners()
    {
        closeSettings
            .onClick.AddListener(() =>
            {
                GameManager.Instance.TogglePause();
                settingsMenu.SetActive(false);
            });

        resetNormalSettings
            .onClick.AddListener(() =>
            {
                GameSaveManager.Instance.SetBestScore(Enums.GameMode.NORMAL, 0);
                GameSaveManager.Instance.SetLevel(Enums.GameMode.NORMAL, 0);
                bestScoreNormalSettings.text = "0";
                levelNormalSettings.text = "0";
            });
        resetHardSettings
            .onClick.AddListener(() =>
            {
                GameSaveManager.Instance.SetBestScore(Enums.GameMode.HARD, 0);
                GameSaveManager.Instance.SetLevel(Enums.GameMode.HARD, 0);
                bestScoreHardSettings.text = "0";
                levelHardSettings.text = "0";
            });
    }

    private void controlsListeners()
    {
        leftButton
            .onClick.AddListener(() =>
            {
                InputManager.Instance.ControlsInput.Invoke(Enums.ControlsEvents.LEFT);
                //boardManager.MoveBrick(Enums.Directions.LEFT);
            });

        rightButton
            .onClick.AddListener(() =>
            {
                InputManager.Instance.ControlsInput.Invoke(Enums.ControlsEvents.RIGHT);
                //boardManager.MoveBrick(Enums.Directions.RIGHT);
            });

        downButton
            .onClick.AddListener(() =>
            {
                InputManager.Instance.ControlsInput.Invoke(Enums.ControlsEvents.DOWN);
                //boardManager.MoveBrick(Enums.Directions.BOTTOM);
            });

        dropButton
            .onClick.AddListener(() =>
            {
                InputManager.Instance.ControlsInput.Invoke(Enums.ControlsEvents.DROP);
                //boardManager.DropBrick();
            });

        rotateRightButton
            .onClick.AddListener(() =>
            {
                InputManager.Instance.ControlsInput.Invoke(Enums.ControlsEvents.RIGHTROTATION);
                //boardManager.RotateBrick(Enums.Directions.RIGHT);
            });

        rotateLeftButton
            .onClick.AddListener(() =>
            {
                InputManager.Instance.ControlsInput.Invoke(Enums.ControlsEvents.LEFTROTATION);
                //boardManager.RotateBrick(Enums.Directions.LEFT);
            });
    }

    private void gameOverListeners()
    {
        restart.onClick.AddListener(() =>
        {
            HideGameOverMenu();
            GameManager.Instance.LoadLevel("Main");
        });
        quite2.onClick.AddListener(() => { Application.Quit(); });
        boot2.onClick.AddListener(() =>
        {
            HideGameOverMenu();
            GameManager.Instance.LoadLevel("Boot");
        });
    }
    #endregion

    #region Menus Togglers

    public void HideMainMenu()
    {
        GameManager.Instance.LoadLevel("Main");
        controls.SetActive(true);
        gameInfo.SetActive(true);
        pause.gameObject.SetActive(true);
        mainMenu.SetActive(false);
        dummyCammera.gameObject.SetActive(false);
    }

    public void ShowMainMenu()
    {
        controls.SetActive(false);
        gameInfo.SetActive(false);
        pause.gameObject.SetActive(false);
        mainMenu.SetActive(true);
        dummyCammera.gameObject.SetActive(true);
    }

    public void ShowGameOverMenu(int score)
    {
        finalScore.text = score.ToString();
        bestScore.text = GameSaveManager.Instance.GetBestScore(GameManager.Instance.GameMode).ToString();
        gameOverMenu.gameObject.SetActive(true);
    }

    public void HideGameOverMenu()
    {
        gameOverMenu.gameObject.SetActive(false);
    }

    #endregion

    #region Info Updates
    public void UpdateScore(int value) => score.text = value.ToString();

    public void UpdateLevel(int value) => level.text = value.ToString();

    public void UpdateLines(int value) => lines.text = value.ToString();

    public void UpdateGameOverScore(int value) => finalScore.text = value.ToString();

    public void UpdateGameOverBestScore(int value) => bestScore.text = value.ToString();

    public void ResetInfo()
    {
        UpdateScore(0);
        UpdateLines(0);
    }

    #endregion

    //public void GetBoardManager() => boardManager = FindObjectOfType<GameBoard>();

}
