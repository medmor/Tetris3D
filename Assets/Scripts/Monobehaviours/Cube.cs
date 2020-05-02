using UnityEngine;
using System.Collections.Generic;

public delegate void RemoveLine();
public class Cube : MonoBehaviour
{
    public bool IsActiveInBoard = false;
    private Dictionary<Vector3, Enums.Directions> faces = new Dictionary<Vector3, Enums.Directions>()
    {
        {new Vector3(0.0f, 0.0f, 1.0f), Enums.Directions.FRONT},
        {new Vector3(0.0f, -1.0f, 0.0f), Enums.Directions.BOTTOM},
        {new Vector3(0.0f, 0.0f, -1.0f), Enums.Directions.BACK},
        {new Vector3(0.0f, 1.0f, 0.0f), Enums.Directions.TOP},
        {new Vector3(-1.0f, 0.0f, 0.0f), Enums.Directions.LEFT},
        {new Vector3(1.0f, 0.0f, 0.0f), Enums.Directions.RIGHT},
    };
    private Vector3 faceVector = new Vector3();
    void Start()
    {
        initColors();
    }

    public void SetPosition(Vector3 pos)
    {
        transform.localPosition = pos;
    }

    private void initColors()
    {
        Mesh mesh = GetComponent<MeshFilter>().mesh;
        Vector2[] UVs = new Vector2[mesh.vertices.Length];
        // Front
        UVs[0] = new Vector2(0.0f, 0.0f);
        UVs[1] = new Vector2(0.333f, 0.0f);
        UVs[2] = new Vector2(0.0f, 0.333f);
        UVs[3] = new Vector2(0.333f, 0.333f);
        // Top
        UVs[4] = new Vector2(0.334f, 0.333f);
        UVs[5] = new Vector2(0.666f, 0.333f);
        UVs[8] = new Vector2(0.334f, 0.0f);
        UVs[9] = new Vector2(0.666f, 0.0f);
        // Back
        UVs[6] = new Vector2(1.0f, 0.0f);
        UVs[7] = new Vector2(0.667f, 0.0f);
        UVs[10] = new Vector2(1.0f, 0.333f);
        UVs[11] = new Vector2(0.667f, 0.333f);
        // Bottom
        UVs[12] = new Vector2(0.0f, 0.334f);
        UVs[13] = new Vector2(0.0f, 0.666f);
        UVs[14] = new Vector2(0.333f, 0.666f);
        UVs[15] = new Vector2(0.333f, 0.334f);
        // Left
        UVs[16] = new Vector2(0.334f, 0.334f);
        UVs[17] = new Vector2(0.334f, 0.666f);
        UVs[18] = new Vector2(0.666f, 0.666f);
        UVs[19] = new Vector2(0.666f, 0.334f);
        // Right        
        UVs[20] = new Vector2(0.667f, 0.334f);
        UVs[21] = new Vector2(0.667f, 0.666f);
        UVs[22] = new Vector2(1.0f, 0.666f);
        UVs[23] = new Vector2(1.0f, 0.334f);
        mesh.uv = UVs;
    }

    public Enums.Directions FrontCurrentFace()
    {
        faceVector. x = Mathf.Round(Vector3.Dot(-Vector3.forward, transform.right));
        faceVector. y = Mathf.Round(Vector3.Dot(-Vector3.forward, transform.up));
        faceVector. z = Mathf.Round(Vector3.Dot(-Vector3.forward, transform.forward));
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
            case Enums.Directions.BOTTOM:
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