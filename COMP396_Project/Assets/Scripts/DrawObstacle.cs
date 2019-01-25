using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DrawObstacle : MonoBehaviour
{
    [SerializeField] private GameObject lineGeneratorPrefab;
    [SerializeField] private GameObject linePointPrefab;
    [System.Serializable] public struct Obstacle
    {
        public Vector3[] obstaclePoints;
    }
    [SerializeField] private Obstacle[] obstacles;
    [SerializeField] private bool bUserDefine = false;

    private LinkedList<Vector3> cacheObstacle = new LinkedList<Vector3>();

    // Getter and Setter
    public Obstacle[] GetObstacles()
    {
        return obstacles;
    }


    // Start is called before the first frame update
    void Start()
    {
        if (!bUserDefine)
        {
            GenerateObstacles(obstacles);
            // GenerateCollider(obstacles);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (bUserDefine)
        {
            if (Input.GetMouseButtonDown(0))
            {
                Vector3 newPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                newPos.z = 0;
                cacheObstacle.AddFirst(newPos);
                CreatePointMarker(newPos);
            }

            if (Input.GetMouseButtonDown(1))
            {
                clearAllPoints();
            }

            if (Input.GetKeyDown("space"))
            {
                GenerateObstacle(new Obstacle { obstaclePoints = cacheObstacle.ToArray() });
                cacheObstacle.Clear();
            }
        }
    }

    private void GenerateObstacle(Obstacle obstacle)
    {
        GenerateNewLine(obstacle.obstaclePoints);
    }


    private void GenerateObstacles(Obstacle[] Obstacles)
    {
        foreach (Obstacle obstacle in Obstacles)
        {
            // Generate the obstacle points
            foreach (Vector3 obstaclePoint in obstacle.obstaclePoints)
            {
                Instantiate(linePointPrefab, obstaclePoint, Quaternion.identity);
            }
            // Generate the obstacles
            GenerateObstacle(obstacle);
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

    private void CreatePointMarker(Vector2 pointPosition)
    {
        Instantiate(linePointPrefab, pointPosition, Quaternion.identity);
    }

    private void clearAllPoints()
    {
        GameObject[] allPoints = GameObject.FindGameObjectsWithTag("PointMarker");

        foreach (GameObject p in allPoints)
        {
            Destroy(p);
        }
    }

    private void GenerateCollider(int index, Obstacle obstacle)
    {
        PolygonCollider2D polygonCollider = GetComponent<PolygonCollider2D>();
        Vector2[] obstaclePoints2D = new Vector2[obstacle.obstaclePoints.Length];
        for (int i = 0; i < obstacle.obstaclePoints.Length; i++)
        {
            obstaclePoints2D[i] = new Vector2(obstacle.obstaclePoints[i].x, obstacle.obstaclePoints[i].y);
        }
        
        polygonCollider.CreatePrimitive(obstacle.obstaclePoints.Length);
        polygonCollider.SetPath(index, obstaclePoints2D);
    }

    private void GenerateCollider(Obstacle[] obstacles)
    {
        int index = 0;
        foreach (Obstacle obstacle in obstacles)
        {
            // TODO fix the function
            // GenerateCollider(index, obstacle);
            index++;
        }
    }
}
