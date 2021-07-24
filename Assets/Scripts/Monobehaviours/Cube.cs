using System.Collections.Generic;
using UnityEngine;

public class Cube : MonoBehaviour
{

    private Dictionary<Vector3, Enums.Directions> faces = new Dictionary<Vector3, Enums.Directions>()
    {
        {new Vector3(0.0f, 0.0f, 1.0f), Enums.Directions.FRONT},
        {new Vector3(0.0f, -1.0f, 0.0f), Enums.Directions.DOWN},
        {new Vector3(0.0f, 0.0f, -1.0f), Enums.Directions.BACK},
        {new Vector3(0.0f, 1.0f, 0.0f), Enums.Directions.TOP},
        {new Vector3(-1.0f, 0.0f, 0.0f), Enums.Directions.LEFT},
        {new Vector3(1.0f, 0.0f, 0.0f), Enums.Directions.RIGHT},
    };
    private Vector3 faceVector = new Vector3();

    public int Id { get; set; }


    public void SetPosition(Vector3 pos)
    {
        transform.localPosition = pos;
    }


    public Enums.Directions FrontCurrentFace()
    {
        faceVector.x = Mathf.Round(Vector3.Dot(-Vector3.forward, transform.right));
        faceVector.y = Mathf.Round(Vector3.Dot(-Vector3.forward, transform.up));
        faceVector.z = Mathf.Round(Vector3.Dot(-Vector3.forward, transform.forward));
        return faces[faceVector];
    }

    private Vector3 RightCurrentFace()
    {
        faceVector.x = Mathf.Round(Vector3.Dot(Vector3.right, transform.right));
        faceVector.y = Mathf.Round(Vector3.Dot(Vector3.right, transform.up));
        faceVector.z = Mathf.Round(Vector3.Dot(Vector3.right, transform.forward));
        return faceVector;
    }

    private Vector3 UpCurrentFace()
    {
        faceVector.x = Mathf.Round(Vector3.Dot(Vector3.up, transform.right));
        faceVector.y = Mathf.Round(Vector3.Dot(Vector3.up, transform.up));
        faceVector.z = Mathf.Round(Vector3.Dot(Vector3.up, transform.forward));
        return faceVector;
    }

    public void FlipCube(Enums.Directions direction)
    {
        switch (direction)
        {
            case Enums.Directions.LEFT:
                transform.Rotate(UpCurrentFace(), 90f);
                break;
            case Enums.Directions.RIGHT:
                transform.Rotate(UpCurrentFace(), -90f);
                break;
            case Enums.Directions.TOP:
                transform.Rotate(RightCurrentFace(), 90f);
                break;
            case Enums.Directions.DOWN:
                transform.Rotate(RightCurrentFace(), -90f);
                break;
            default:
                break;
        }
    }

    public bool IsFacingGood()
    {
        return transform.up == Vector3.up
            && transform.forward == Vector3.forward
            && transform.right == Vector3.right;
    }

    public void Shake(int count, RemoveLine removeLine)
    {
        if (count < 2)
        {
            Vector3 initalPosition = gameObject.transform.position;
            var offset = Random.insideUnitSphere * .1f;
            LeanTween.moveLocal(gameObject, transform.position + offset, .05f)
                .setOnComplete(() =>
                {
                    LeanTween.moveLocal(gameObject, initalPosition, .05f)
                    .setOnComplete(() =>
                    {
                        Shake(count + 1, removeLine);
                    });
                });
        }
        else { removeLine(); }

    }
}
public delegate void RemoveLine();