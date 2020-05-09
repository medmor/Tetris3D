using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : Manager<UIManager>
{
    BoardManager boardManager = default; //Instantiated when the level load in the game manager

    [Header("Boot Menu")]
    [SerializeField] private GameObject mainMenu = default;
    [SerializeField] private Camera dummyCammera = default;
    [SerializeField] private Button normalModeButton = default;
    [SerializeField] private Button hardModeButton = default;

    [Header("Pause Menu")]
    [SerializeField] private GameObject pauseMenu = default;
    [SerializeField] private Button pause = default;
    [SerializeField] private Button resume = default;
    [SerializeField] private Button quite = default;
    [SerializeField] private Button boot = default;

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
        mainMenuListners();
        pauseListeners();
        controlsListeners();
        gameOverListeners();
    }

    #region Listeners
    private void mainMenuListners()
    {
        normalModeButton.onClick.AddListener(() =>
        {
            GameManager.Instance.LoadLevel("Main");
            GameManager.Instance.GameMode = Enums.GameMode.NORMAL;
        });
        hardModeButton.onClick.AddListener(() =>
        {
            GameManager.Instance.LoadLevel("Main");
            GameManager.Instance.GameMode = Enums.GameMode.HARD;
        });
    }

    private void pauseListeners()
    {
        pause.onClick.AddListener(() => {
            GameManager.Instance.TogglePause();
            pauseMenu.gameObject.SetActive(true);
            pause.gameObject.SetActive(false);
        });

        resume.onClick.AddListener(() => {
            GameManager.Instance.TogglePause();
            pauseMenu.gameObject.SetActive(false);
            pause.gameObject.SetActive(true);
        });

        quite.onClick.AddListener(() => { Application.Quit(); });

        boot.onClick.AddListener(() => {
            pauseMenu.gameObject.SetActive(false);
            GameManager.Instance.LoadLevel("Boot");
        });
    }

    private void controlsListeners()
    {
        leftButton.onClick.AddListener(() => {
            boardManager.MoveBrick(Enums.Directions.LEFT);
        });

        rightButton.onClick.AddListener(() => {
            boardManager.MoveBrick(Enums.Directions.RIGHT);
        });

        downButton.onClick.AddListener(() => {
            boardManager.MoveBrick(Enums.Directions.BOTTOM);
        });

        dropButton.onClick.AddListener(() => {
            boardManager.DropBrick();
        });

        rotateRightButton.onClick.AddListener(() => {
            boardManager.RotateBrick(Enums.Directions.RIGHT);
        });

        rotateLeftButton.onClick.AddListener(() => {
            boardManager.RotateBrick(Enums.Directions.LEFT);
        });
    }

    private void gameOverListeners()
    {
        restart.onClick.AddListener(() => {
            HideGameOverMenu();
            GameManager.Instance.LoadLevel("Main");
        });
        quite2.onClick.AddListener(() => { Application.Quit(); });
        boot2.onClick.AddListener(() => {
            HideGameOverMenu();
            GameManager.Instance.LoadLevel("Boot");
        });
    }
    #endregion

    #region Menus Togglers

    public void HideMainMenu()
    {
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
        bestScore.text = PlayerPrefs.GetInt("bestScore").ToString();
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

    #endregion

    public void GetBoardManager() => boardManager = FindObjectOfType<BoardManager>();

}
