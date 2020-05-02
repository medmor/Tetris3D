﻿using UnityEngine;

public class CubeInput : MonoBehaviour
{
    Vector2 firstPressPos;
    Vector2 secondPressPos;
    Vector2 currentSwipe;

    Cube touchedCube;
    [SerializeField] Camera sideCamera = default;

    void Update()
    {
        Swipe();
    }

    public void Swipe()
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
                        if (currentSwipe.x < 0)
                            touchedCube.FlipCube(Enums.Directions.LEFT);
                        else
                            touchedCube.FlipCube(Enums.Directions.RIGHT);
                    else
                        if (currentSwipe.y < 0)
                            touchedCube.FlipCube(Enums.Directions.BOTTOM);
                        else
                            touchedCube.FlipCube(Enums.Directions.TOP);
                    touchedCube = null;
                }
            }
        }
    }
}