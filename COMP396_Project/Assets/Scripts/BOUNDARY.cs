using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BOUNDARY : MonoBehaviour
{
    [SerializeField] private GameObject lineGeneratorPrefab;
    [SerializeField]
    private Vector3[] BoundaryLine = new Vector3[4]
    {
        new Vector3(15f, 8f, 0f),
        new Vector3(15f, -8f, 0f),
        new Vector3(-15f, -8f, 0f),
        new Vector3(-15f, 8f, 0f)
    };

    // Getter and Setter
    public Vector3[] GetBoundaryLine()
    {
        return BoundaryLine;
    }

    // Start is called before the first frame update
    void Start()
    {
        GenerateNewLine(BoundaryLine);
        GenerateColliders(BoundaryLine);
    }

    private void GenerateNewLine(Vector3[] linePoints)
    {
        if (linePoints.Length > 1)
        {
            SpawnLineGenerator(linePoints);
        }
        else
        {
            Debug.Log("Need 2 or more points to draw a line.");
        }
    }

    private void SpawnLineGenerator(Vector3[] linePoints)
    {
        GameObject newLineGen = Instantiate(lineGeneratorPrefab);
        LineRenderer lRend = newLineGen.GetComponent<LineRenderer>();

        lRend.positionCount = linePoints.Length;
        lRend.SetPositions(linePoints);
    }

    private void GenerateCollider(Vector2 v1, Vector2 v2)
    {
        EdgeCollider2D edgeCollider = gameObject.AddComponent<EdgeCollider2D>();
        edgeCollider.points = new Vector2[]
        {
            v1,
            v2
        };

    }

    private void GenerateColliders(Vector3[] boundaryLine)
    {
        for (int i = 0; i < boundaryLine.Length; i++)
        {
            GenerateCollider(new Vector2(boundaryLine[i].x, boundaryLine[i].y)
                , new Vector2(boundaryLine[(i + 1) % boundaryLine.Length].x, boundaryLine[(i + 1) % boundaryLine.Length].y));
        }
    }
}
