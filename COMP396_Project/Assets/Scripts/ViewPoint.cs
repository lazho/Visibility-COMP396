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
            RaycastHit2D rayCastHit2D = Physics2D.Linecast(viewpoint.transform.position, endPoint);
            Instantiate(IntersectionPointPrefab, rayCastHit2D.point, Quaternion.identity);
            Debug.DrawLine(viewpoint.transform.position, rayCastHit2D.point, Color.blue, 100.0f);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
