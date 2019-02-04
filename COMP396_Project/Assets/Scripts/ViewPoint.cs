using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ViewPoint : MonoBehaviour
{
    [SerializeField] private Vector3[] BoundaryLine;
    [SerializeField] private GameObject IntersectionPointPrefab;
    [SerializeField] private GameObject ViewPointPrefab;
    [SerializeField] private DrawObstacle.Obstacle[] ObstaclesLine;
    [SerializeField] private GameObject lineGeneratorPrefab;
    private float screenWidthInUnits = 32f;
    private float screenHeightInUnits = 18f;
    private float offsetX = 16f;
    private float offsetY = 9f;

    public readonly double ACCURACY = 0.001;

    // cache variable
    DrawBoundary drawboundary;
    DrawObstacle drawobstacle;

    // Start is called before the first frame update
    void Start()
    {
        drawboundary = GameObject.Find("BoundaryManager").GetComponent<DrawBoundary>();
        drawobstacle = GameObject.Find("ObstacleManager").GetComponent<DrawObstacle>();

        // Generate the obstacles and boundry in advance
        BoundaryLine = drawboundary.GetBoundaryLine();
        ObstaclesLine = drawobstacle.GetObstacles();

    }

    // Update is called once per frame
    void Update()
    {

        if (!drawobstacle.bUserDefine)
        {
            GameObject viewpoint = GameObject.FindGameObjectWithTag("ViewPoint");
            viewpoint.transform.position = new Vector2(Mathf.Clamp(GetMousePosition().x, -15, 15), Mathf.Clamp(GetMousePosition().y, -8, 8));
            GameObject viewpointPrefab = Instantiate(ViewPointPrefab, viewpoint.transform.position, Quaternion.identity);

            GenerateCriticalPoint(viewpoint);
            Destroy(viewpointPrefab, 0.02f);
        }
        else
        {
            // update the obstacles and boundry in advance
            BoundaryLine = drawboundary.GetBoundaryLine();
            ObstaclesLine = drawobstacle.GetObstacles();
        }
    }

    private void GenerateCriticalPoint(GameObject viewpoint)
    {
        if (viewpoint)
        {
            if (BoundaryLine.Length != 0)
            {
                GenerateLineCast(viewpoint, BoundaryLine);
                foreach (DrawObstacle.Obstacle obstacleLine in ObstaclesLine)
                {
                    GenerateLineCast(viewpoint, obstacleLine.obstaclePoints);
                }
            }
            else
            {
                Debug.Log("No element in BoundaryLine.");
            }
        }
    }

    private Vector3 GetMousePosition()
    {
        return new Vector2(Input.mousePosition.x / Screen.width * screenWidthInUnits - offsetX, Input.mousePosition.y / Screen.height * screenHeightInUnits - offsetY);
    }

    /**
     * Generate all the critical points based on the position of the viewpoint and end points
     * @param viewpoint   the oject of the view point
     * @param endPoints   the list of the end points that need to connected with the view point     
     */
    private void GenerateLineCast(GameObject viewpoint, Vector3[] endPoints)
    {
        LinkedList<Vector2> criticalPoints = new LinkedList<Vector2>();
        Vector2 criticalPoint = new Vector2();
        for (int i = 0; i < endPoints.Length; i++)
        {
            Vector2 direction = endPoints[i] - viewpoint.transform.position;
            /*
            if (Physics2D.Raycast(viewpoint.transform.position, direction))
            {
                Debug.Log(endPoints[i]);
            }
            */

            // RaycastHit2D[] rayCastHits2D = Physics2D.RaycastAll(viewpoint.transform.position, endPoints[i]);
            // RaycastHit2D[] rayCastHits2D = Physics2D.RaycastAll(viewpoint.transform.position, direction, Mathf.Infinity, 511);
            // Debug.Log(endPoints[i] + ": " + rayCastHits2D.Length);


            LinkedList<RaycastHit2D> rayCastHits2D = new LinkedList<RaycastHit2D>();
            Vector2 start = viewpoint.transform.position;

            while (Physics2D.Raycast(start, direction))
            {
                RaycastHit2D rayHit = Physics2D.Raycast(start, direction, Mathf.Infinity);
                // Debug.Log("start:" + start + "    end: " + endPoints[i] + "direction:" + direction + "rayHit:" + rayHit.point);

                // rayCastHits2D.AddLast(rayCastHit);
                // start = rayCastHit.point + direction.normalized;

                // if the hit result is the same position as obstacle position
                if (Math.Abs(rayHit.point.x - endPoints[i].x) < ACCURACY && Math.Abs(rayHit.point.y - endPoints[i].y) < ACCURACY)
                {
                    // Instantiate(IntersectionPointPrefab, rayHit.point, Quaternion.identity);
                    criticalPoints.AddFirst(rayHit.point);
                    // If the neighbour endpoints of the hitting result are both in the one side, keep the hitting result
                    Vector3 prev = endPoints[(i + endPoints.Length - 1) % endPoints.Length];
                    Vector3 next = endPoints[(i + 1) % endPoints.Length];

                    if (AreSameSide(endPoints[i] - viewpoint.transform.position, prev, next))
                    {
                        start = rayHit.point + direction.normalized;
                        continue;
                    }
                    else
                    {
                        // that is what I want
                        criticalPoint = rayHit.point;
                        break;
                    }
                }
                else
                {
                    criticalPoint = rayHit.point;
                    break;
                }
            }
            /*
            foreach (RaycastHit2D ray in rayCastHits2D)
            {
                Debug.Log("collide" + endPoints[i] + ray.point);
            }
            */

            LinkedList<GameObject> crticalpointsPrefab = new LinkedList<GameObject>();
            LinkedList<LineRenderer> criticalpointsLineRenderers = new LinkedList<LineRenderer>();
            foreach (Vector2 criticalpoint in criticalPoints)
            {
                crticalpointsPrefab.AddLast(Instantiate(IntersectionPointPrefab, criticalPoint, Quaternion.identity));
                
                // TODO use line renderer to generate the debug line

                GameObject newLineGen = Instantiate(lineGeneratorPrefab);
                LineRenderer lRend = newLineGen.GetComponent<LineRenderer>();
                if(lRend)
                {
                    // viewpoint and the end points
                    lRend.positionCount = 2;
                    lRend.SetPosition(0, new Vector3(viewpoint.transform.position.x, viewpoint.transform.position.y, 0));
                    lRend.SetPosition(1, new Vector3(criticalpoint.x, criticalpoint.y, 0));
                    criticalpointsLineRenderers.AddLast(lRend);
                }
                else
                {
                    Debug.Log("Something bad!!");
                }
            }

            foreach (GameObject criticalpointPrefab in crticalpointsPrefab)
            {
                Destroy(criticalpointPrefab, 0.02f);
            }

            foreach (LineRenderer lineRenderer in criticalpointsLineRenderers)
            {
                Destroy(lineRenderer, 0.03f);
            }




            // Track through all the hit result
            foreach (RaycastHit2D rayHit in rayCastHits2D)
            {
                /*
                // if the hit result is the same position as obstacle position
                if (Math.Abs(rayHit.point.x - endPoints[i].x) < ACCURACY && Math.Abs(rayHit.point.y - endPoints[i].y) < ACCURACY)
                {
                    // Instantiate(IntersectionPointPrefab, rayHit.point, Quaternion.identity);
                    criticalPoints.AddFirst(rayHit.point);
                    // If the neighbour endpoints of the hitting result are both in the one side, keep the hitting result
                    Vector3 prev = endPoints[(i + endPoints.Length - 1) % endPoints.Length];
                    Vector3 next = endPoints[(i + 1) % endPoints.Length];

                    if (AreSameSide(endPoints[i] - viewpoint.transform.position, prev, next))
                    {
                        continue;
                    }
                    else
                    {
                        // that is what I want
                        criticalPoint = rayHit.point;
                        break;
                    }
                }
                else
                {
                    criticalPoint = rayHit.point;
                    break;
                }
                */
            }
            // Debug.DrawLine(viewpoint.transform.position, criticalPoint, Color.blue, 100.0f);
        }
    }

    // test the two points are on the same side of test line or not
    private bool AreSameSide(Vector2 testLine, Vector2 testPoint1, Vector2 testPoint2)
    {
        float test1 = CrossProduct(testLine, testPoint1);
        float test2 = CrossProduct(testLine, testPoint2);
        return test1 * test2 > 0;
    }

    // @return the cross product of two vectors
    private float CrossProduct(Vector2 vector1, Vector2 vector2)
    {
        return vector1.x * vector2.y - vector1.y * vector2.x;
    }
}
