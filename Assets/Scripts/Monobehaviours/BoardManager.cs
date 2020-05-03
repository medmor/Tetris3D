using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class BoardManager : MonoBehaviour
{
    #region Declarations
    private readonly int[,] SCORE_TABLE = new int[20, 5] {
        { 0, 40, 100, 300, 1200 },
        { 0, 80, 200, 600, 2400 },
        { 0, 120, 300, 900, 3600 },
        { 0, 160, 400, 1200, 4800 },
        { 0, 200, 500, 1500, 6000 },
        { 0, 240, 600, 1800, 7200 },
        { 0, 280, 700, 2100, 8400 },
        { 0, 320, 800, 2400, 9600 },
        { 0, 360, 900, 2700, 10800 },
        { 0, 400, 1000, 3000, 12000},
        { 0, 400, 1000, 3000, 12000},
        { 0, 400, 1000, 3000, 12000},
        { 0, 400, 1000, 3000, 12000},
        { 0, 400, 1000, 3000, 12000},
        { 0, 400, 1000, 3000, 12000},
        { 0, 400, 1000, 3000, 12000},
        { 0, 400, 1000, 3000, 12000},
        { 0, 400, 1000, 3000, 12000},
        { 0, 400, 1000, 3000, 12000},
        { 0, 400, 1000, 3000, 12000},    };
    private readonly int[] SPEED_TABLE = new int[30] { 48, 43, 38, 33, 28, 23, 18, 13, 8, 6, 5, 5, 5, 4, 4, 4, 3, 3, 3, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 1 };
    private const int BOARD_WIDTH = 10;
    private const int BOARD_HEIGHT = 21;

    private float speed;
    private int numLinesRemoved = 0;
    private int score = 0;
    private int level = 4;
    private int linesToLevel;

    [SerializeField] private Brick currentBrick = default;
    [SerializeField] private Brick dropBrick = default;
    [SerializeField] private Brick[] nextBricks = default;
    [SerializeField] private SideBrick sideBrick = default;
    [SerializeField] private GameObject cubePrefab = default;

    private Cube[] board;
    [SerializeField] private Material gridMaterial = default;
    [SerializeField] private Material cubeMaterial = default;

    private Enums.BoardStats boardState;
    #endregion


    #region Game Initiation
    private void Start()
    {
        GameManager.Instance.UpdateState(Enums.GameState.RUNNING);// to review ??
        Init();
        StartCoroutine(nextTick());
    }
    private IEnumerator nextTick()
    {
        while (true)
        {
            //if(GameManager.Instance.CurrentGameState != Enums.GameState.PAUSED)
                if (boardState == Enums.BoardStats.GENERATING_NEW_BRICK)
                    newBrick();
                else if(boardState == Enums.BoardStats.FALLING 
                    && !currentBrick.Move(Enums.Directions.BOTTOM, BOARD_WIDTH, ShapeAt))
                    updateBoard();
             yield return new WaitForSeconds(speed);
        }
        
    }
    void Init()
    {
        linesToLevel = level * 10 + 10; // depends on starting level
        speed = SPEED_TABLE[level] * 0.017f;

        board = new Cube[BOARD_WIDTH * BOARD_HEIGHT];
        for (int i = 0; i < BOARD_HEIGHT * BOARD_WIDTH; i++)
        {
            board[i] = Instantiate(cubePrefab).GetComponent<Cube>();
            board[i].transform.position = new Vector3(Mathf.Floor(i % BOARD_WIDTH), i/BOARD_WIDTH, 0);
            board[i].gameObject.GetComponent<MeshRenderer>().material = gridMaterial;
            board[i].transform.SetParent(transform);
            if (i >= (BOARD_HEIGHT - 1) * BOARD_WIDTH)
                board[i].GetComponent<Renderer>().enabled = false;
        }
        
        currentBrick.gameObject.SetActive(false);
        dropBrick.gameObject.SetActive(false);

        for (int i = 0; i < nextBricks.Length; i++)
        {
            nextBricks[i].transform.position = new Vector3(11.5f, 18 - i * 3, 0);
            nextBricks[i].SetRandomShape();
            nextBricks[i].transform.localScale *= .6f;
        }
        newBrick();
    }

    #endregion


    #region Game Updates
    private void updateBoard()
    {
        boardState = Enums.BoardStats.UPDATING_BOARD;

        for (int i = 0; i < 4; i++)
        {
            var pos = currentBrick.brickShape[i].transform.position;
            var index = Mathf.RoundToInt(pos.y * BOARD_WIDTH + pos.x);

            board[index].IsActiveInBoard = true;
            board[index].transform.rotation = currentBrick.brickShape[i].transform.rotation;
            board[index].gameObject.GetComponent<MeshRenderer>().material = cubeMaterial;
         }
        currentBrick.gameObject.SetActive(false);
        dropBrick.gameObject.SetActive(false);
        removeFullLinesLogic();
        if (boardState == Enums.BoardStats.GENERATING_NEW_BRICK)
        {
            newBrick();
        }
    }
    private void newBrick()
    {
        updateNextBricks();
        currentBrick.gameObject.SetActive(true);
        currentBrick.transform.rotation = Quaternion.identity;
        dropBrick.transform.rotation = Quaternion.identity;
        dropBrick.gameObject.SetActive(true);
        currentBrick.transform.position = new Vector3(BOARD_WIDTH / 2, BOARD_HEIGHT-1, 0);
        updateDropBrick();
        if (!currentBrick.Move(Enums.Directions.BOTTOM, BOARD_WIDTH, ShapeAt))
        {
            //currentBrick.SetShape(0);
            GameManager.Instance.LoadLevel("Boot");
        }
        boardState = Enums.BoardStats.FALLING;
    }
    private void updateNextBricks()
    {
        currentBrick.SetShape(nextBricks[0].brickType);
        sideBrick.SetShape(currentBrick.brickType, true);
        dropBrick.SetShape(nextBricks[0].brickType);
        nextBricks[0].SetShape(nextBricks[1].brickType);
        nextBricks[1].SetShape(nextBricks[2].brickType);
        nextBricks[2].SetRandomShape();
    }
    private void updateDropBrick()
    {
        dropBrick.transform.position = currentBrick.transform.position;
        dropBrick.transform.rotation = currentBrick.transform.rotation;
        //sideBrick.transform.rotation = currentBrick.transform.rotation;
        dropBrick.DropDown(BOARD_WIDTH, ShapeAt);
    }
    public bool ShapeAt(int x, int y) => board[y * BOARD_WIDTH + x].IsActiveInBoard;
    #endregion


    #region RemoveFullLines
    private Enums.Directions firstCubeLineDirection;
    private void removeFullLinesLogic()
    {
        int numFullLines = getAndCallRemoveFullLines();

        if (numFullLines > 0)
        {
            playFullLineSound(numFullLines);
            updateInfo(numFullLines);
        }
        else
        {
            boardState = Enums.BoardStats.GENERATING_NEW_BRICK;
        }
    }
    private int getAndCallRemoveFullLines()
    {
        int numFullLines = 0;
        for (int i = BOARD_HEIGHT - 1; i >= 0; i--)
        {
            firstCubeLineDirection = board[i * BOARD_WIDTH].FrontCurrentFace();
            bool lineIsFull = true;
            for (int j = 0; j < BOARD_WIDTH; j++)
            {

                if (!ShapeAt(j, i))
                {
                    lineIsFull = false;
                    break;
                }
                if (GameManager.Instance.GameMode != Enums.GameMode.NORMAL)
                {
                    if (firstCubeLineDirection != board[i * BOARD_WIDTH + j].FrontCurrentFace())
                    {
                        lineIsFull = false;
                        break;
                    }
                }

            }
            if (lineIsFull)
            {
                numFullLines++;
                int count = 0;
                removeFullLines(i, count);
            }
        }
        return numFullLines;
    }

    private void updateInfo(int numFullLines)
    {
        numLinesRemoved += numFullLines;
        UIManager.Instance.UpdateLines(numLinesRemoved);
        score += SCORE_TABLE[level, numFullLines];
        UIManager.Instance.UpdateScore(score);
        linesToLevel -= numFullLines;
        if (linesToLevel <= 0)
        {
            level++;
            UIManager.Instance.UpdateLevel(level);
            linesToLevel = 10;
            speed = SPEED_TABLE[level] * 0.017f;
        }
    }

    private void playFullLineSound(int numFullLines)
    {
        if (numFullLines == 1) SoundManager.Instance.PlaySound(Enums.SoundsEffects.ONE_LINE);
        else if (numFullLines == 2) SoundManager.Instance.PlaySound(Enums.SoundsEffects.TWO_LINE);
        else if (numFullLines == 3) SoundManager.Instance.PlaySound(Enums.SoundsEffects.TREE_LINE);
        else if (numFullLines == 4) SoundManager.Instance.PlaySound(Enums.SoundsEffects.FOUR_LINE);
    }

    private void removeFullLines(int fullLineIndex, int count)
    {
        for (int j = 0; j < BOARD_WIDTH; j++)
        {
            Cube cube = board[(fullLineIndex * BOARD_WIDTH) + j];
            cube.Shake(1, () => {
                count++;
                if (count == BOARD_WIDTH)
                {
                    for (int k = fullLineIndex; k < BOARD_HEIGHT - 1; k++)
                    {
                        for (int jj = 0; jj < BOARD_WIDTH; jj++)
                        {
                            cube = board[(k * BOARD_WIDTH) + jj];
                            var topCube = board[((k + 1) * BOARD_WIDTH) + jj];
                            cube.IsActiveInBoard = ShapeAt(jj, k + 1);
                            cube.GetComponent<MeshRenderer>().material = topCube.GetComponent<MeshRenderer>().material;
                            cube.transform.rotation = topCube.transform.rotation;
                        }
                    }
                    boardState = Enums.BoardStats.GENERATING_NEW_BRICK;
                }
            });
        }
    }
    #endregion

    #region InputHandling
    private void Update()
    {
        var swipInfo = sideBrick.Swipe();
        if(swipInfo != null)
        {
            for (int i = 0; i < currentBrick.brickShape.Length; i++)
            {
                if(currentBrick.brickShape[i].Id == swipInfo.Item2)
                {
                    currentBrick.brickShape[i].FlipCube(swipInfo.Item1);
                }
            }
        }
        //    if (boardState == Enums.BoardStats.FALLING)
        //    {
        //        if (Input.GetKeyDown(KeyCode.LeftArrow))
        //            MoveBrick(Enums.Directions.LEFT);

        //        else if (Input.GetKeyDown(KeyCode.RightArrow))
        //            MoveBrick(Enums.Directions.RIGHT);

        //        else if (Input.GetKeyDown(KeyCode.DownArrow))
        //            RotateBrick(Enums.Directions.RIGHT);

        //        else if (Input.GetKeyDown(KeyCode.UpArrow))
        //            RotateBrick(Enums.Directions.LEFT);

        //        else if (Input.GetKeyDown(KeyCode.Space))
        //            DropBrick();

        //        else if (Input.GetKeyDown(KeyCode.D))
        //            MoveBrick(Enums.Directions.BOTTOM);

        //        else if (Input.GetKeyDown(KeyCode.Alpha1))
        //            RotateBrickFaces(Enums.Directions.RIGHT);

        //        else if (Input.GetKeyDown(KeyCode.Alpha2))
        //            RotateBrickFaces(Enums.Directions.LEFT);

        //        else if (Input.GetKeyDown(KeyCode.Alpha3))
        //            RotateBrickFaces(Enums.Directions.TOP);

        //        else if (Input.GetKeyDown(KeyCode.Alpha4))
        //            RotateBrickFaces(Enums.Directions.BOTTOM);

        //    }
    }

    public void RotateBrick(Enums.Directions direction)
    {
        switch (direction)
        {
            case Enums.Directions.LEFT:
                currentBrick.RotateLeft(BOARD_WIDTH, BOARD_HEIGHT, ShapeAt);
                break;
            case Enums.Directions.RIGHT:
                currentBrick.RotateRight(BOARD_WIDTH, BOARD_HEIGHT, ShapeAt);
                break;
            default:
                break;
        }
        updateDropBrick();
    }

    public void MoveBrick(Enums.Directions direction)
    {
        switch (direction)
        {
            case Enums.Directions.LEFT:
                currentBrick.Move(Enums.Directions.LEFT, BOARD_WIDTH, ShapeAt);
                break;
            case Enums.Directions.RIGHT:
                currentBrick.Move(Enums.Directions.RIGHT, BOARD_WIDTH, ShapeAt);
                break;
            case Enums.Directions.BOTTOM:
                if (!currentBrick.Move(Enums.Directions.BOTTOM, BOARD_WIDTH, ShapeAt))
                    updateBoard();
                return;
            default:
                break;
        }
        updateDropBrick();
    }

    public void DropBrick()
    {
        currentBrick.DropDown(BOARD_WIDTH, ShapeAt);
        updateBoard();
    }

    public void RotateBrickFaces(Enums.Directions direction)
    {
        switch (direction)
        {
            case Enums.Directions.LEFT:
                currentBrick.RotateFaces(Enums.Directions.LEFT);
                break;
            case Enums.Directions.RIGHT:
                currentBrick.RotateFaces(Enums.Directions.RIGHT);
                break;
            case Enums.Directions.TOP:
                currentBrick.RotateFaces(Enums.Directions.TOP);
                break;
            case Enums.Directions.BOTTOM:
                currentBrick.RotateFaces(Enums.Directions.BOTTOM);
                break;
            default:
                break;
        }
    }

    #endregion
}