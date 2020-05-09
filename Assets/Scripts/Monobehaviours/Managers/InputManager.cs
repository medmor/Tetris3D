using System;
using UnityEngine;

public delegate void MoveAndRotate();
public class InputManager : Manager<InputManager>
{

    Vector2 firstPressPos;
    Vector2 secondPressPos;
    Vector2 currentSwipe;

    //bool swipTouch;

    GameObject hitObject = default;

    Enums.Directions swipDirection = default;

    public void WindowsInput(BoardManager boardManager)
    {
        if (boardManager.boardState == Enums.BoardStats.FALLING)
        {
            if (Input.GetKeyDown(KeyCode.LeftArrow))
                boardManager.MoveBrick(Enums.Directions.LEFT);

            else if (Input.GetKeyDown(KeyCode.RightArrow))
                boardManager.MoveBrick(Enums.Directions.RIGHT);

            else if (Input.GetKeyDown(KeyCode.DownArrow))
                boardManager.RotateBrick(Enums.Directions.RIGHT);

            else if (Input.GetKeyDown(KeyCode.UpArrow))
                boardManager.RotateBrick(Enums.Directions.LEFT);

            else if (Input.GetKeyDown(KeyCode.Space))
                boardManager.DropBrick();

            else if (Input.GetKeyDown(KeyCode.P))
                GameManager.Instance.TogglePause();

            else if (Input.GetKeyDown(KeyCode.D))
                boardManager.MoveBrick(Enums.Directions.BOTTOM);

            else if (Input.GetKeyDown(KeyCode.Alpha1))
                boardManager.RotateBrickFaces(Enums.Directions.RIGHT);

            else if (Input.GetKeyDown(KeyCode.Alpha2))
                boardManager.RotateBrickFaces(Enums.Directions.LEFT);

            else if (Input.GetKeyDown(KeyCode.Alpha3))
                boardManager.RotateBrickFaces(Enums.Directions.TOP);

            else if (Input.GetKeyDown(KeyCode.Alpha4))
                boardManager.RotateBrickFaces(Enums.Directions.BOTTOM);

        }
    }

    public Tuple<Enums.Directions, int> Swipe(Camera cam, string tag)
    {
        if (Input.touches.Length > 0)
        {
            Touch t = Input.GetTouch(0);
            if (t.phase == TouchPhase.Began)
                firstPressPos = new Vector2(t.position.x, t.position.y);

            var ray = cam.ScreenPointToRay(t.position);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit) && hitObject == null)
                if (hit.collider.gameObject.tag == tag)
                    hitObject = hit.collider.gameObject;

            if (t.phase == TouchPhase.Ended && hitObject != null)
            {
                secondPressPos = new Vector2(t.position.x, t.position.y);

                currentSwipe = new Vector3(secondPressPos.x - firstPressPos.x, secondPressPos.y - firstPressPos.y);

                if (Vector2.SqrMagnitude(currentSwipe) > 500)
                {
                    currentSwipe.Normalize();
                    if (Mathf.Abs(currentSwipe.x) > Mathf.Abs(currentSwipe.y))
                    {
                        if (currentSwipe.x < 0)
                            swipDirection = Enums.Directions.LEFT;

                        else
                            swipDirection = Enums.Directions.RIGHT;
                    }
                    else
                        if (currentSwipe.y < 0)
                        swipDirection = Enums.Directions.BOTTOM;
                    else
                        swipDirection = Enums.Directions.TOP;

                    int temp = -1;
                    if (hitObject.GetComponent<Cube1>())
                        temp = hitObject.GetComponent<Cube1>().Id;
                    hitObject = null;
                    return new Tuple<Enums.Directions, int>(swipDirection, temp);
                }
            }
        }
        return null;
    }

    public Tuple<Enums.Directions, int> Swipe(Camera sideCamera)
    {
        if (Input.touches.Length > 0)
        {
            Touch touch = Input.GetTouch(0);
            if (touch.phase == TouchPhase.Began)
                firstPressPos = new Vector2(touch.position.x, touch.position.y);

            var ray = sideCamera.ScreenPointToRay(touch.position);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit) && hitObject == null)
            {
                if (hit.collider.gameObject.tag == "SideBrick")
                    hitObject = hit.collider.gameObject;
            }
            else
            {
                ray = Camera.main.ScreenPointToRay(touch.position);
                if (Physics.Raycast(ray, out hit))
                    if (hit.collider.gameObject.tag == "Grid")
                        hitObject = hit.collider.gameObject;
            }

            if (touch.phase == TouchPhase.Ended && hitObject != null)
            {
                secondPressPos = new Vector2(touch.position.x, touch.position.y);

                currentSwipe = new Vector3(secondPressPos.x - firstPressPos.x, secondPressPos.y - firstPressPos.y);

                if (Vector2.SqrMagnitude(currentSwipe) > 500)
                {
                    currentSwipe.Normalize();
                    if (Mathf.Abs(currentSwipe.x) > Mathf.Abs(currentSwipe.y))
                    {
                        if (currentSwipe.x < 0)
                            swipDirection = Enums.Directions.LEFT;

                        else
                            swipDirection = Enums.Directions.RIGHT;
                    }
                    else
                        if (currentSwipe.y < 0)
                        swipDirection = Enums.Directions.BOTTOM;
                    else
                        swipDirection = Enums.Directions.TOP;

                    int temp = -1;
                    if (hitObject.GetComponent<Cube1>())
                        temp = hitObject.GetComponent<Cube1>().Id;
                    hitObject = null;
                    return new Tuple<Enums.Directions, int>(swipDirection, temp);
                }
            }
        }
        return null;
    }
}
