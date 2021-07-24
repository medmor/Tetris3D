using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameBoard : MonoBehaviour
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
    private const int BOARD_WIDTH = 7;
    private const int BOARD_HEIGHT = 15;

    // [SerializeField] Camera sideCamera = default;

    [SerializeField] Player player = default;

    [SerializeField] private Brick currentBrick = default;
    [SerializeField] private Brick dropBrick = default;
    [SerializeField] private SideBrick sideBrick = default;
    [SerializeField] private Brick[] nextBricks = default;
    [SerializeField] private GameObject cubePrefab = default;

    private Cube[] board;

    public Enums.BoardStats boardState;
    #endregion

    #region Game Initiation

    private void Start()
    {
        GameManager.Instance.UpdateState(Enums.GameState.RUNNING);// to review ??
        UIManager.Instance.ResetInfo();
        player.InitPlayer();
        init();
        ControlsInputEvetListning();
        SwipInputEventListning();
        StartCoroutine(nextTick());
    }
    private IEnumerator nextTick()
    {
        while (true)
        {
            if (boardState == Enums.BoardStats.GENERATING_NEW_BRICK)
                newBrick();
            else if (boardState == Enums.BoardStats.FALLING
                && !currentBrick.Move(Enums.Directions.DOWN, BOARD_WIDTH, ShapeAt))
                updateBoard();
            yield return new WaitForSeconds(player.speed);
        }

    }
    private void init()
    {
        player.SetLinesToLevelUp(player.level * 10 + 10);  // depends on starting level
        player.SetSpeed(SPEED_TABLE[player.level] * 0.017f);
        UIManager.Instance.UpdateLevel(player.level);
        UIManager.Instance.UpdateLines(player.linesRemoved);
        UIManager.Instance.UpdateLines(player.score);

        board = new Cube[BOARD_WIDTH * BOARD_HEIGHT + 1];
        for (int i = 0; i < board.Length; i++)
        {
            board[i] = Instantiate(cubePrefab).GetComponent<Cube>();
            board[i].transform.SetParent(transform);
            board[i].transform.position = new Vector3(Mathf.Floor(i % BOARD_WIDTH), i / BOARD_WIDTH, 0);
            board[i].gameObject.SetActive(false);
        }

        currentBrick.gameObject.SetActive(false);
        dropBrick.gameObject.SetActive(false);
        foreach (var c in dropBrick.brickShape)
            c.transform.localScale = new Vector3(.98f, .98f, .98f);

        for (int i = 0; i < nextBricks.Length; i++)
        {
            nextBricks[i].transform.position = new Vector3(26f, 13 - i * 2, 0);
            nextBricks[i].SetRandomShape();
            if (GameManager.Instance.GameMode == Enums.GameMode.HARD)
                nextBricks[i].RandomFaceColors(1);
            nextBricks[i].transform.localScale *= .6f;
        }
        newBrick();
    }
    private IEnumerator gameOver()
    {
        SoundManager.Instance.PlaySound(Enums.SoundsEffects.GAMEOVER);
        for (int i = 0; i < board.Length - 1; i++)
        {
            board[i].gameObject.SetActive(true);
            board[i].transform.rotation = Quaternion.identity;
            yield return new WaitForFixedUpdate();
        }

        GameSaveManager.Instance.SetLevel(GameManager.Instance.GameMode, player.level);

        if (GameSaveManager.Instance.IsBestScoreSaved(GameManager.Instance.GameMode))
        {
            int bestScore = GameSaveManager.Instance.GetBestScore(GameManager.Instance.GameMode);
            if (bestScore < player.score)
                GameSaveManager.Instance.SetBestScore(GameManager.Instance.GameMode, player.score);
        }
        else
        {
            GameSaveManager.Instance.SetBestScore(GameManager.Instance.GameMode, player.score);
        }
        UIManager.Instance.ShowGameOverMenu(player.score);
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
            board[index].gameObject.SetActive(true);
            board[index].transform.rotation = currentBrick.brickShape[i].transform.rotation;
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
        currentBrick.transform.position = new Vector3(BOARD_WIDTH / 2, BOARD_HEIGHT - 1, 0);
        updateDropBrick();
        if (!currentBrick.Move(Enums.Directions.DOWN, BOARD_WIDTH, ShapeAt))
        {
            GameManager.Instance.UpdateState(Enums.GameState.GAMEOVER);
            dropBrick.gameObject.SetActive(false);
            StopAllCoroutines();
            StartCoroutine(gameOver());
        }
        boardState = Enums.BoardStats.FALLING;
    }
    private void updateNextBricks()
    {
        currentBrick.CopyShape(nextBricks[0]);
        sideBrick.CopyShape(currentBrick, true);
        dropBrick.CopyShape(nextBricks[0]);
        nextBricks[0].CopyShape(nextBricks[1]);
        nextBricks[1].CopyShape(nextBricks[2]);
        nextBricks[2].SetRandomShape();
        if (GameManager.Instance.GameMode == Enums.GameMode.HARD)
        {
            nextBricks[2].RandomFaceColors(1);
        }
    }
    private void updateDropBrick()
    {
        dropBrick.transform.position = currentBrick.transform.position;
        dropBrick.transform.rotation = currentBrick.transform.rotation;
        dropBrick.DropDown(BOARD_WIDTH, ShapeAt);
    }
    public bool ShapeAt(int x, int y) => board[y * BOARD_WIDTH + x].gameObject.activeSelf;
    #endregion

    #region RemoveFullLines
    private Enums.Directions firstCubeLineDirection;
    private List<Enums.Directions> hardLinesDirections = new List<Enums.Directions>();
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
            hardLinesDirections.Add(firstCubeLineDirection);
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
        player.SetLinesRemoved(player.linesRemoved + numFullLines);
        UIManager.Instance.UpdateLines(player.linesRemoved);
        int score = player.score + SCORE_TABLE[player.level, numFullLines];
        if (GameManager.Instance.GameMode == Enums.GameMode.HARD)
            score *= hardLinesDirections.Count;
        player.SetScore(score);
        UIManager.Instance.UpdateScore(player.score);
        player.SetLinesToLevelUp(player.linesToLevelUp - numFullLines);
        if (player.linesToLevelUp <= 0)
        {
            player.SetLevel(player.level + 1);
            UIManager.Instance.UpdateLevel(player.level);
            player.SetLinesToLevelUp(player.linesToLevelUp + 10);
            player.SetSpeed(SPEED_TABLE[player.level] * 0.017f);
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
            cube.Shake(1, () =>
            {
                count++;
                if (count == BOARD_WIDTH)
                {
                    for (int k = fullLineIndex; k < BOARD_HEIGHT - 1; k++)
                    {
                        for (int jj = 0; jj < BOARD_WIDTH; jj++)
                        {
                            cube = board[(k * BOARD_WIDTH) + jj];
                            var topCube = board[((k + 1) * BOARD_WIDTH) + jj];
                            cube.gameObject.SetActive(ShapeAt(jj, k + 1));
                            //cube.GetComponent<MeshRenderer>().material = topCube.GetComponent<MeshRenderer>().material;
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
    //private void Update()
    //{
    //    //InputManager.Instance.KeyBoardInput(this);

    //    var swipInfo = InputManager.Instance.Swipe(sideCamera);
    //    if (swipInfo != null && swipInfo.Item2 == -1)
    //    {
    //        for (int i = 0; i < 4; i++)
    //        {
    //            sideBrick.brickShape[i].FlipCube(swipInfo.Item1);
    //            currentBrick.brickShape[i].FlipCube(swipInfo.Item1);
    //        }
    //    }
    //    else if (swipInfo != null && swipInfo.Item2 > -1)
    //    {
    //        for (int i = 0; i < 4; i++)
    //        {
    //            if (currentBrick.brickShape[i].Id == swipInfo.Item2)
    //            {
    //                sideBrick.brickShape[i].FlipCube(swipInfo.Item1);
    //                currentBrick.brickShape[i].FlipCube(swipInfo.Item1);
    //            }
    //        }
    //    }
    //}
    public void ControlsInputEvetListning()
    {
        InputManager.Instance.ControlsInput.AddListener((Enums.ControlsEvents controlsEvent) =>
        {
            switch (controlsEvent)
            {
                case Enums.ControlsEvents.RIGHT:
                    currentBrick.Move(Enums.Directions.RIGHT, BOARD_WIDTH, ShapeAt);
                    break;
                case Enums.ControlsEvents.DOWN:
                    currentBrick.Move(Enums.Directions.DOWN, BOARD_WIDTH, ShapeAt);
                    break;
                case Enums.ControlsEvents.LEFT:
                    currentBrick.Move(Enums.Directions.LEFT, BOARD_WIDTH, ShapeAt);
                    break;
                case Enums.ControlsEvents.DROP:
                    currentBrick.DropDown(BOARD_WIDTH, ShapeAt);
                    break;
                case Enums.ControlsEvents.RIGHTROTATION:
                    currentBrick.RotateLeft(BOARD_WIDTH, BOARD_HEIGHT, ShapeAt);
                    break;
                case Enums.ControlsEvents.LEFTROTATION:
                    currentBrick.RotateRight(BOARD_WIDTH, BOARD_HEIGHT, ShapeAt);
                    break;
            }
            updateDropBrick();
        });
    }
    public void SwipInputEventListning()
    {
        InputManager.Instance.SwipInput.AddListener((Enums.Directions direction, int cubeId) =>
        {
            if (cubeId == -1)
            {
                for (int i = 0; i < 4; i++)
                {
                    sideBrick.brickShape[i].FlipCube(direction);
                    currentBrick.brickShape[i].FlipCube(direction);
                }
            }
            else
            {
                for (int i = 0; i < 4; i++)
                {
                    if (currentBrick.brickShape[i].Id == cubeId)
                    {
                        sideBrick.brickShape[i].FlipCube(direction);
                        currentBrick.brickShape[i].FlipCube(direction);
                    }
                }
            }
        });
    }

    //public void RotateBrick(Enums.Directions direction)
    //{
    //    switch (direction)
    //    {
    //        case Enums.Directions.LEFT:
    //            currentBrick.RotateLeft(BOARD_WIDTH, BOARD_HEIGHT, ShapeAt);
    //            break;
    //        case Enums.Directions.RIGHT:
    //            currentBrick.RotateRight(BOARD_WIDTH, BOARD_HEIGHT, ShapeAt);
    //            break;
    //        default:
    //            break;
    //    }
    //    updateDropBrick();
    //}

    //public void MoveBrick(Enums.Directions direction)
    //{
    //    switch (direction)
    //    {
    //        case Enums.Directions.LEFT:
    //            currentBrick.Move(Enums.Directions.LEFT, BOARD_WIDTH, ShapeAt);
    //            break;
    //        case Enums.Directions.RIGHT:
    //            currentBrick.Move(Enums.Directions.RIGHT, BOARD_WIDTH, ShapeAt);
    //            break;
    //        case Enums.Directions.DOWN:
    //            if (!currentBrick.Move(Enums.Directions.DOWN, BOARD_WIDTH, ShapeAt))
    //                updateBoard();
    //            return;
    //        default:
    //            break;
    //    }
    //    updateDropBrick();
    //}

    //public void DropBrick()
    //{

    //    currentBrick.DropDown(BOARD_WIDTH, ShapeAt);
    //    updateBoard();
    //}

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
            case Enums.Directions.DOWN:
                currentBrick.RotateFaces(Enums.Directions.DOWN);
                break;
            default:
                break;
        }
    }

    #endregion

}