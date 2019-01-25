using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ViewPoint : MonoBehaviour
{
    [SerializeField] private Vector3[] BoundaryLine;
    [SerializeField] private GameObject IntersectionPointPrefab;
    [SerializeField] private DrawObstacle.Obstacle[] ObstaclesLine;

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
        foreach (Vector3 endPoint in endPoints)
        {
            Vector2 direction = endPoint - viewpoint.transform.position;
            RaycastHit2D[] rayCastHits2D = Physics2D.RaycastAll(viewpoint.transform.position, direction);
            foreach (RaycastHit2D rayHit in rayCastHits2D)
            {
                Instantiate(IntersectionPointPrefab, rayHit.point, Quaternion.identity);
            }

            try
            {
                Debug.DrawLine(viewpoint.transform.position, rayCastHits2D[0].point, Color.blue, 100.0f);
            }
            catch (IndexOutOfRangeException ex)
            {
                Debug.Log(rayCastHits2D.Length + " " + endPoint);
            }
            
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
