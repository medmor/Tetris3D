using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : Manager<UIManager>
{
    BoardManager boardManager = default; //Instantiated when the level load in the game manager

    [SerializeField] private GameObject mainMenu = default;
    [SerializeField] private Camera dummyCammera = default;
    [SerializeField] private Button normalModeButton = default;
    [SerializeField] private Button hardModeButton = default;

    [SerializeField] private GameObject gameInfo = default;
    [SerializeField] private TextMeshProUGUI score = default;
    [SerializeField] private TextMeshProUGUI level = default;
    [SerializeField] private TextMeshProUGUI lines = default;

    [SerializeField] private GameObject controls = default;
    [SerializeField] private Button leftButton = default;
    [SerializeField] private Button rightButton = default;
    [SerializeField] private Button downButton = default;
    [SerializeField] private Button rotateRightButton = default;
    [SerializeField] private Button rotateLeftButton = default;
    [SerializeField] private Button dropButton = default;

    private void Start()
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
    public void HideMainMenu()
    {
        controls.SetActive(true);
        gameInfo.SetActive(true);
        mainMenu.SetActive(false);
        dummyCammera.gameObject.SetActive(false);
    }

    public void ShowMainMenu()
    {
        controls.SetActive(false);
        gameInfo.SetActive(false);
        mainMenu.SetActive(true);
        dummyCammera.gameObject.SetActive(true);
    }

    public void UpdateScore(int value)
    {
        score.text = value.ToString();
    }

    public void UpdateLevel(int value)
    {
        level.text = value.ToString();
    }

    public void UpdateLines(int value)
    {
        lines.text = value.ToString();
    }

    public void GetBoardManager()
    {
        boardManager = FindObjectOfType<BoardManager>();
    }
    public void Pause()
    {
        GameManager.Instance.TogglePause();
    }

}
