using System;
using UnityEngine;

public delegate bool ShapeAtDelegate(int x, int y);

public class Brick : MonoBehaviour
{
    private float[,,] ALLSHAPES = new float[7, 4, 2] {
                                        {{0, 0}, {0, 1}, {-1, -1}, {0, -1}},//S
                                        {{-1, 0}, {0, 0}, {0, -1}, {1, -1}},//Z
                                        {{-2, 0}, {-1, 0}, {0, 0}, {1, 0}},//I
                                        {{-1, 0}, {0, 0}, {1, 0}, {0, -1}},//T
                                        {{-1, 0}, {0, 0}, {-1, -1}, {0, -1}},//O
                                        {{-1, 0}, {0, 0}, {1, 0}, {1, -1}},//J
                                        {{-1, 0}, {0, 0}, {1, 0}, {-1, -1}}//L
                                    };

    public Enums.BrickType brickType;
    public Cube1[] brickShape  = new Cube1[4];

    public void SetShape(Enums.BrickType type, bool offset = false)
    {
        brickType = type;
        int index = (int) brickType;
        float off = offset? 1.5f : 1;
        for (int i = 0; i < ALLSHAPES.GetLength(1); i++)
        {
            brickShape[i].SetPosition(new Vector3(ALLSHAPES[index, i, 0] * off, ALLSHAPES[index, i, 1] * off, 0));
            brickShape[i].transform.rotation = Quaternion.identity;
            brickShape[i].Id = i;
        }
    }

    public void SetRandomShape()
    {
        Array values = Enum.GetValues(typeof(Enums.BrickType));
        SetShape((Enums.BrickType)values.GetValue(UnityEngine.Random.Range(1, 7)));
    }

    public float MinY()
    {
        float m = brickShape[0].transform.localPosition.y;
        for (int i = 1; i < 4; i++)
        {
            m = Mathf.Min(m, brickShape[i].transform.localPosition.y);
        }
        return m;
    }

    public void RotateLeft(int boardWidth, int boardHeight, ShapeAtDelegate shapeAt)
    {
        if (brickType == Enums.BrickType.OSHAPE)
        {
            SoundManager.Instance.PlaySound(Enums.SoundsEffects.ROTATION);
            return;
        }
        var shape = copyShape();
        for (int i = 0; i < 4; i++)
        {
            var temp = shape[i ,0];
            shape[i, 0] = shape[i, 1];
            shape[i, 1] = -temp;
        }
        if (canRotate(shape, boardWidth, boardHeight, shapeAt))
        {
            SoundManager.Instance.PlaySound(Enums.SoundsEffects.ROTATION);
            transform.Rotate(new Vector3(0, 0, -90));
        }
    }

    public void RotateRight(int boardWidth, int boardHeight, ShapeAtDelegate shapeAt)
    {
        if (brickType == Enums.BrickType.OSHAPE)
        {
            SoundManager.Instance.PlaySound(Enums.SoundsEffects.ROTATION);
            return;
        }
        var shape = copyShape();
        for (int i = 0; i < 4; i++)
        {
            var temp = shape[i, 0];
            shape[i, 0] = -shape[i, 1];
            shape[i, 1] = temp;
        }
        if (canRotate(shape, boardWidth, boardHeight, shapeAt))
        {
            transform.Rotate(new Vector3(0, 0, 90));
            SoundManager.Instance.PlaySound(Enums.SoundsEffects.ROTATION);
        }
    }

    public void RotateFaces(Enums.Directions direction)
    {
        switch (direction)
        {
            case Enums.Directions.LEFT:
                for (int i = 0; i < 4; i++)
                {
                    brickShape[i].FlipCube(Enums.Directions.LEFT);
                }
                break;
            case Enums.Directions.RIGHT:
                for (int i = 0; i < 4; i++)
                {
                    brickShape[i].FlipCube(Enums.Directions.RIGHT);
                }
                break;
            case Enums.Directions.TOP:
                for (int i = 0; i < 4; i++)
                {
                    brickShape[i].FlipCube(Enums.Directions.TOP);
                }
                break;
            case Enums.Directions.BOTTOM:
                for (int i = 0; i < 4; i++)
                {
                    brickShape[i].FlipCube(Enums.Directions.BOTTOM);
                }
                break;
            default:
                break;
        }
        
    }

    public bool Move(Enums.Directions direction, int boardWidth, ShapeAtDelegate shapeAt)
    {
        bool canMoveDown = true;
        if (direction == Enums.Directions.RIGHT)
        {
            if (canMove(Enums.Directions.RIGHT, boardWidth, shapeAt))
            {
                transform.position += Vector3.right;
                SoundManager.Instance.PlaySound(Enums.SoundsEffects.MOVE_LR);
            }
        }
        else if (direction == Enums.Directions.LEFT)
        {
            if (canMove(Enums.Directions.LEFT, boardWidth, shapeAt))
            {
                transform.position += Vector3.left;
                SoundManager.Instance.PlaySound(Enums.SoundsEffects.MOVE_LR);
            }
        }
        else if (direction == Enums.Directions.BOTTOM)
        {
            if (canMove(Enums.Directions.BOTTOM, boardWidth, shapeAt))
            {
                transform.position += Vector3.down;
                SoundManager.Instance.PlaySound(Enums.SoundsEffects.FALL);
            }
            else
                canMoveDown = false;
        }
        return canMoveDown;
    }

    public void DropDown(int boardWidth, ShapeAtDelegate shapeAt)
    {
        float newY = transform.position.y;
        while (newY > float.Epsilon)
        {
            if (!Move(Enums.Directions.BOTTOM, boardWidth, shapeAt))
            {
                break;
            }
            newY--;
        }
    }

    private bool canMove(Enums.Directions direction, int boardWidth, ShapeAtDelegate shapeAt)
    {
        bool canMove = true;
        if (direction == Enums.Directions.RIGHT)
            for (int i = 0; i < 4; i++)
            {
                int x = Mathf.RoundToInt(brickShape[i].transform.position.x);
                int y = Mathf.RoundToInt(brickShape[i].transform.position.y);
                if (x + 1 >= boardWidth || shapeAt(x + 1, y))
                {
                    canMove = false;
                    break;
                }
            }
        else if (direction == Enums.Directions.LEFT)
            for (int i = 0; i < 4; i++)
            {
                int x = Mathf.RoundToInt(brickShape[i].transform.position.x);
                int y = Mathf.RoundToInt(brickShape[i].transform.position.y);
                if (x - 1 < 0 || shapeAt(x - 1, y))
                {
                    canMove = false;
                    break;
                }
            }

        else if (direction == Enums.Directions.BOTTOM)
        {
            for (int i = 0; i < 4; i++)
            {
                int x = Mathf.RoundToInt(brickShape[i].transform.position.x);
                int y = Mathf.RoundToInt(brickShape[i].transform.position.y);
                if (y - 1 < 0 || shapeAt(x, y - 1))
                {
                    canMove = false;
                    break;
                }
            }
        }
        else return false;
                    
        return canMove;
    }

    private bool canRotate(float[,] shape, int boardWidth, int boardHeight, ShapeAtDelegate shapeAt)
    {
        for (int i = 0; i < 4; i++)
        {
            float x = transform.localPosition.x + shape[i, 0];
            float y = transform.localPosition.y - shape[i, 1];
            if ( x < 0 || x >= boardWidth 
                || y < 0 || y >= boardHeight
                || shapeAt(Mathf.RoundToInt(x), Mathf.RoundToInt(y)))
                return false;
        }
            
        return true;
    }

    private float[,] copyShape()
    {
        float[,] shape = new float[4, 2];
        for (int i = 0; i < 4; i++)
        {
            shape[i, 0] = transform.position.x - brickShape[i].transform.position.x;
            shape[i, 1] = transform.position.y - brickShape[i].transform.position.y;
        }
        return shape;
    }

}
