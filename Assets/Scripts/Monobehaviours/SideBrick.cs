using System;
using UnityEngine;

public class SideBrick : Brick
{
    Vector2 firstPressPos;
    Vector2 secondPressPos;
    Vector2 currentSwipe;

    Cube touchedCube;
    [SerializeField] Camera sideCamera = default;

    private Enums.Directions swipDirection = default;


    public Tuple<Enums.Directions, int> Swipe()
    {
        if (Input.touches.Length > 0)
        {
            Touch t = Input.GetTouch(0);
            if (t.phase == TouchPhase.Began)
                firstPressPos = new Vector2(t.position.x, t.position.y);

            var ray = sideCamera.ScreenPointToRay(t.position);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit) && touchedCube == null)
                touchedCube = hit.collider.gameObject.GetComponent<Cube>();

            if (t.phase == TouchPhase.Ended && touchedCube)
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
    
                    touchedCube.FlipCube(swipDirection);
                    int temp = touchedCube.Id;
                    touchedCube = null;
                    return new Tuple<Enums.Directions, int>(swipDirection, temp);
                }
            }
        }
        return null;
    }
}