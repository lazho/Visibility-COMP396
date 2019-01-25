using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ViewPoint : MonoBehaviour
{
    [SerializeField] private Vector3[] BoundaryLine;
    [SerializeField] private GameObject IntersectionPointPrefab;
    [SerializeField] private DrawObstacle.Obstacle[] ObstaclesLine;

    public readonly double ACCURACY = 0.001;

    // Start is called before the first frame update
    void Start()
    {
        DrawBoundary drawboundary = GameObject.Find("BoundaryManager").GetComponent<DrawBoundary>();
        DrawObstacle drawobstacle = GameObject.Find("ObstacleManager").GetComponent<DrawObstacle>();

        BoundaryLine = drawboundary.GetBoundaryLine();
        ObstaclesLine = drawobstacle.GetObstacles();
        GameObject viewpoint = GameObject.FindGameObjectWithTag("ViewPoint");
        if (viewpoint)
        {
            if (BoundaryLine.Length != 0)
            {
                GenerateLineCast(viewpoint, BoundaryLine);
                foreach (DrawObstacle.Obstacle obstacleLine in ObstaclesLine)
                {
                    GenerateLineCast(viewpoint, obstacleLine.obstaclePoints);
                }
                // Testing general case
                GenerateLineCast(viewpoint, new Vector3[]
                {
                    new Vector3(0, 9, 0)
                });
            }
            else
            {
                Debug.Log("No element in BoundaryLine.");
            }
        }
    }

    private void GenerateLineCast(GameObject viewpoint, Vector3[] endPoints)
    {
        
        for (int i = 0; i < endPoints.Length; i++)
        {
            Vector2 direction = endPoints[i] - viewpoint.transform.position;
            RaycastHit2D[] rayCastHits2D = Physics2D.RaycastAll(viewpoint.transform.position, direction);

             // Track through all the hit result
            foreach (RaycastHit2D rayHit in rayCastHits2D)
            {
                // if the hit result is the same position as obstacle position
                if (Math.Abs(rayHit.point.x - endPoints[i].x) < ACCURACY && Math.Abs(rayHit.point.y - endPoints[i].y) < ACCURACY)
                {
                    // If the neighbour endpoints of the hitting result are both in the one side, keep the hitting result
                    Vector3 prev = endPoints[(i + endPoints.Length - 1) % endPoints.Length];
                    Vector3 next = endPoints[(i + 1) % endPoints.Length];
                    // TODO test whether the two point are on the same side or not.
                    bool testResult = false;
                    if (testResult)
                    {
                        continue;
                    }
                    else
                    {
                        // that is what I want
                        Instantiate(IntersectionPointPrefab, rayHit.point, Quaternion.identity);
                        break;
                    }
                }
            }

            try
            {
                Debug.DrawLine(viewpoint.transform.position, rayCastHits2D[0].point, Color.blue, 100.0f);
            }
            catch (IndexOutOfRangeException ex)
            {
                Debug.Log(rayCastHits2D.Length + " " + endPoints[i]);
            }
            
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
