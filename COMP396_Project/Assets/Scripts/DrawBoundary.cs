using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawBoundary : MonoBehaviour
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
        GenerateCollider(BoundaryLine);
    }



    // Update is called once per frame
    void Update()
    {
       
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

    private void GenerateCollider(Vector3[] boundaryLine)
    {
        EdgeCollider2D edgeCollider = GetComponent<EdgeCollider2D>();
        Vector2[] boundaryLine2D = new Vector2[boundaryLine.Length + 1];
        for (int i = 0; i < boundaryLine.Length; i++)
        {
            boundaryLine2D[i] = boundaryLine[i];
        }
        boundaryLine2D[boundaryLine.Length] = boundaryLine[0];
        edgeCollider.points = boundaryLine2D;
    }
}
