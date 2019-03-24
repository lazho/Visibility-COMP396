using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class OBSTACLE : MonoBehaviour
{
    [SerializeField] private GameObject lineGeneratorPrefab;
    [SerializeField] private GameObject linePointPrefab;
    [System.Serializable] public struct Obstacle
    {
        public Vector3[] obstaclePoints;
        public int index;

        public Obstacle(Vector3[] points, int i)
        {
            obstaclePoints = points;
            index = i;
        }
    }
    [SerializeField] private Obstacle[] obstacles;
    [SerializeField] public bool bUserDefine = false;

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
            GenerateIndex();
            GenerateObstacles(obstacles);
            GenerateCollider(obstacles);
        }
    }

    private void GenerateIndex()
    {
        for (int i = 0; i < obstacles.Length; i++)
        {
            obstacles[i].index = i + 1;
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

            if (Input.GetKeyDown(KeyCode.Space))
            {
                if (cacheObstacle.Count > 2)
                {
                    // Add new obstacle to obstacles
                    Obstacle[] temp = new Obstacle[obstacles.Length + 1];
                    for (int i = 0; i < obstacles.Length; i++)
                    {
                        temp[i] = obstacles[i];
                    }
                    temp[obstacles.Length] = new Obstacle { obstaclePoints = cacheObstacle.ToArray() };
                    obstacles = temp;

                    GenerateObstacle(obstacles[obstacles.Length - 1]);
                    cacheObstacle.Clear();
                }
            }

            if(Input.GetKeyDown(KeyCode.Escape))
            {
                bUserDefine = false;
                GenerateCollider(obstacles);
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

    private void GenerateCollider(Obstacle[] obstacles)
    {
        foreach(Obstacle obstacle in obstacles)
        {
            List<Obstacle> trianglizedObstacles = new List<Obstacle>();
            trianglizedObstacles = HelpFunction.triangularization(obstacle);
            foreach (Obstacle trianglizedObstacle in trianglizedObstacles)
            {
                addCollider(trianglizedObstacle);
            }
        }
    }

    private void addCollider(Obstacle obstacle)
    {
        PolygonCollider2D newPolygonCollider2D = gameObject.AddComponent<PolygonCollider2D>();
        newPolygonCollider2D.pathCount = 1;
        Vector2[] obstacles2D = new Vector2[obstacle.obstaclePoints.Length];
        for (int j = 0; j < obstacle.obstaclePoints.Length; j++)
        {
            obstacles2D[j] = obstacle.obstaclePoints[j];
        }
        newPolygonCollider2D.SetPath(0, obstacles2D);
    }
}
