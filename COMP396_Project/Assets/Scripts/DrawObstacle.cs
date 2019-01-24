using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DrawObstacle : MonoBehaviour
{
    [SerializeField] private GameObject lineGeneratorPrefab;
    [SerializeField] private GameObject linePointPrefab;
    [System.Serializable] private struct Obstacle
    {
        public Vector3[] obstaclePoints;
    }
    [SerializeField] private Obstacle[] obstacles = new Obstacle[5];
    [SerializeField] private bool bUserDefine = false;

    private LinkedList<Vector3> cacheObstacle = new LinkedList<Vector3>();

    // Start is called before the first frame update
    void Start()
    {
        if (!bUserDefine)
        {
            GenerateObstacles(obstacles);
        }
    }

    // Update is called once per frame
    void Update()
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

    private void CreatePointMarker(Vector3 pointPosition)
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
}
