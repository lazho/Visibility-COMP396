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
                foreach (Vector3 boundaryPoint in BoundaryLine)
                {
                    RaycastHit2D rayCastHit2D = Physics2D.Linecast(viewpoint.transform.position, boundaryPoint);
                    Instantiate(IntersectionPointPrefab, rayCastHit2D.point, Quaternion.identity);
                    Debug.DrawLine(viewpoint.transform.position, rayCastHit2D.point, Color.blue, 100.0f);
                }

                foreach (DrawObstacle.Obstacle obstacleLine in ObstaclesLine)
                {
                    foreach (Vector3 obstalePoint in obstacleLine.obstaclePoints)
                    {
                        RaycastHit2D rayCastHit2D = Physics2D.Linecast(viewpoint.transform.position, obstalePoint);
                        Instantiate(IntersectionPointPrefab, rayCastHit2D.point, Quaternion.identity);
                        Debug.DrawLine(viewpoint.transform.position, rayCastHit2D.point, Color.blue, 100.0f);
                    }
                }
            }
            else
            {
                Debug.Log("No element in BoundaryLine.");
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
