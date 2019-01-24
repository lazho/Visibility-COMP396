using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawObstacle : MonoBehaviour
{
    [SerializeField] private GameObject lineGeneratorPrefab;
    [SerializeField] private GameObject linePointPrefab;
    private LinkedList<Vector3[]> Obstacles = new LinkedList<Vector3[]>();

    // Start is called before the first frame update
    void Start()
    {
        InitializeObstacles();
        GenerateObstacles(Obstacles);
    }

    private void InitializeObstacles()
    {
        // first obstacle
        Obstacles.AddFirst(new Vector3[] {
            new Vector3(-8f, 3f, 0),
            new Vector3(-7f, 7f, 0),
            new Vector3(-1f, 5f, 0),
            new Vector3(-6f, -1f, 0),
        });

        // second obstacle
        Obstacles.AddFirst(new Vector3[] {
            new Vector3(-9, -7, 0),
            new Vector3(-8, 1, 0),
            new Vector3(-7, -3, 0),
        });

        // third obstacle
        Obstacles.AddFirst(new Vector3[] {
            new Vector3(5, 6, 0),
            new Vector3(6, 6, 0),
            new Vector3(6, 5, 0),
        });

        // fourth obstacle
        Obstacles.AddFirst(new Vector3[] {
            new Vector3(7, 4, 0),
            new Vector3(14, 5, 0),
            new Vector3(8, 1, 0),
        });

        // fifth obstacle
        Obstacles.AddFirst(new Vector3[] {
            new Vector3(6, -2, 0),
            new Vector3(12, 0, 0),
            new Vector3(9, -5, 0),
            new Vector3(3, -7, 0)
        });
    }

    private void GenerateObstacles(LinkedList<Vector3[]> Obstacles)
    {
        foreach (Vector3[] obstacle in Obstacles)
        {
            // Generate the obstacle points
            foreach (Vector3 obstaclePoint in obstacle)
            {
                Instantiate(linePointPrefab, obstaclePoint, Quaternion.identity);
            }
            // Generate the obstacles
            GenerateNewLine(obstacle);
        }
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
}
