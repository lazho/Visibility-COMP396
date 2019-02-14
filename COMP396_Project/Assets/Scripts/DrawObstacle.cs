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
            GenerateObstacles(obstacles);
            GenerateCollider(obstacles);
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

    private bool isConcave(Obstacle obstacle)
    {
        bool result = isClockWise(obstacle.obstaclePoints[0], obstacle.obstaclePoints[1], obstacle.obstaclePoints[2]);
        for (int i = 1; i < obstacle.obstaclePoints.Length; i++)
        {
            if (isClockWise(obstacle.obstaclePoints[i], 
                obstacle.obstaclePoints[(i + 1) % obstacle.obstaclePoints.Length], 
                obstacle.obstaclePoints[(i + 2) % obstacle.obstaclePoints.Length]) != result)
            {
                return true;
            }
        }
        return false;
    }

    private bool isClockWise(Vector2 a, Vector2 b, Vector2 c)
    {
        return (a.x - c.x) * (b.y - c.y) - (b.x - c.x) * (a.y - c.y) < 0 ? true : false;
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
            Obstacle remainObstacle = obstacle;
            while (isConcave(remainObstacle))
            {
                // Triangul
                Obstacle splitedObstacle = triangulationSplit(remainObstacle);
                addCollider(splitedObstacle);
            }
            addCollider(remainObstacle);
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


    /**
     * Split the concave obstacle based on trigulation purpose. Find a splitable vertex, 
     * then split the remain obstacle get a triangle obstacle and another remain polygon.
     * 
     * @invariant remainObstacle.obstaclePoints.Length > 2
     * @pre isConcave(remainObstacle)
     * @para remainObstacle  the obstacle need to be trianglized
     * @return the new trianle obstacle
     */
    public Obstacle triangulationSplit(Obstacle remainObstacle)
    {
        // Search for the concave point
        int searchIndex = 0;
        List<int> covexIndex = new List<int>();
        for (searchIndex = 0; searchIndex < remainObstacle.obstaclePoints.Length; searchIndex++)
        {
            List<Vector3> polygon = new List<Vector3>(remainObstacle.obstaclePoints);
            polygon.RemoveAt(searchIndex);
            if (DrawBoundary.isPointInsidePolygon(remainObstacle.obstaclePoints[searchIndex], polygon.ToArray()))
            {
                break;
            }
            else
            {
                covexIndex.Add(searchIndex);
            }
        }

        // Search for the splitable point
        int canFragementIndex = -1;// the index of splitable point
        for (int i = 0; i < remainObstacle.obstaclePoints.Length; i++)
        {
            if (i > searchIndex)
            {
                List<Vector3> polygon = new List<Vector3>(remainObstacle.obstaclePoints);
                polygon.RemoveAt(i);
                if (!DrawBoundary.isPointInsidePolygon(remainObstacle.obstaclePoints[i], polygon.ToArray()) && IsFragementIndex(i, remainObstacle.obstaclePoints.ToList()))
                {
                    canFragementIndex = i;
                    break;
                }
            }
            else
            {
                if (covexIndex.IndexOf(i) != -1 && IsFragementIndex(i, remainObstacle.obstaclePoints.ToList<Vector3>())) 
                {
                    canFragementIndex = i;
                    break;
                }
            }
        }

        if (canFragementIndex < 0)
        {
            throw new Exception("Data error!! Can not find splitable point");
        }
        else
        {
            //用可划分顶点将凹多边形划分为一个三角形和一个多边形
            Obstacle triangle = new Obstacle();
            Vector3[] triangleObstaclPoint = new Vector3[3];
            int next = (canFragementIndex == remainObstacle.obstaclePoints.Length - 1) ? 0 : canFragementIndex + 1;
            int prev = (canFragementIndex == 0) ? remainObstacle.obstaclePoints.Length - 1 : canFragementIndex - 1;
            triangleObstaclPoint[0] = remainObstacle.obstaclePoints[prev];
            triangleObstaclPoint[1] = remainObstacle.obstaclePoints[canFragementIndex];
            triangleObstaclPoint[2] = remainObstacle.obstaclePoints[next];
            //剔除可划分顶点及索引

            List<Vector3> tempObstclePoints = remainObstacle.obstaclePoints.ToList();
            tempObstclePoints.RemoveAt(canFragementIndex);
            remainObstacle.obstaclePoints = tempObstclePoints.ToArray();

            triangle.obstaclePoints = triangleObstaclPoint;

            return triangle;
        }

        
    }

    /// <summary>
    /// 是否是可划分顶点:新的多边形没有顶点在分割的三角形内
    /// </summary>
    private static bool IsFragementIndex(int index, List<Vector3> verts)
    {
        int len = verts.Count;
        List<Vector3> triangleVert = new List<Vector3>();
        int next = (index == len - 1) ? 0 : index + 1;
        int prev = (index == 0) ? len - 1 : index - 1;
        triangleVert.Add(verts[prev]);
        triangleVert.Add(verts[index]);
        triangleVert.Add(verts[next]);
        for (int i = 0; i < len; i++)
        {
            if (i != index && i != prev && i != next)
            {
                if (DrawBoundary.isPointInsidePolygon(verts[i], triangleVert.ToArray()))
                {
                    return false;
                }
            }
        }
        return true;
    }

}
