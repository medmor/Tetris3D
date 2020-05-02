using UnityEngine;

public class BrickController : MonoBehaviour
{
    private void Start()
    {
    }

    private void Update()
    {
            if (Input.GetKeyDown(KeyCode.UpArrow))
            {
                flipBrick(Enums.Directions.RIGHT);
            }
            else if (Input.GetKeyDown(KeyCode.DownArrow))
            {
                flipBrick(Enums.Directions.LEFT);
            }
            else if (Input.GetKeyDown(KeyCode.RightArrow))
            {
                move(Enums.Directions.RIGHT);
            }
            else if (Input.GetKeyDown(KeyCode.LeftArrow))
            {
                move(Enums.Directions.LEFT);
            }


    }

    private void move(Enums.Directions direction)
    {
        switch (direction)
        {
            case Enums.Directions.LEFT:
                gameObject.transform.position += Vector3.left;
                break;
            case Enums.Directions.RIGHT:
                gameObject.transform.position += Vector3.right;
                break;
            case Enums.Directions.BOTTOM:
                gameObject.transform.position += Vector3.down;
                break;
            default:
                break;
        }
    }

    private void flipBrick(Enums.Directions direction)
    {
        switch (direction)
        {
            case Enums.Directions.LEFT:
                transform.Rotate(0, 0, 90);
                break;
            case Enums.Directions.RIGHT:
                transform.Rotate(0, 0, -90);
                break;
            default: 
                break;
        }

    }
}