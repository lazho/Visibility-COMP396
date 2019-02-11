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
    [SerializeField] private GameObject MeshManager;

    // Test 
    // Whether use mesh to show the visibility effect or not
    [SerializeField] private bool bMesh = false;

    private float screenWidthInUnits = 32f;
    private float screenHeightInUnits = 18f;
    private float offsetX = 16f;
    private float offsetY = 9f;

    public readonly double ACCURACY = 0.01;

    // cache variable
    DrawBoundary drawboundary;
    DrawObstacle drawobstacle;

    LinkedList<Vector2> criticalPoints = new LinkedList<Vector2>();
    GameObject viewpoint;

    // Start is called before the first frame update
    void Start()
    {
        drawboundary = GameObject.Find("BoundaryManager").GetComponent<DrawBoundary>();
        drawobstacle = GameObject.Find("ObstacleManager").GetComponent<DrawObstacle>();

        // Generate the obstacles and boundry in advance
        BoundaryLine = drawboundary.GetBoundaryLine();
        ObstaclesLine = drawobstacle.GetObstacles();

        /*
        viewpoint = GameObject.FindGameObjectWithTag("ViewPoint");
        if (viewpoint)
        {
            // TODO replace (0,0)
            viewpoint.transform.position = new Vector2(0, 0);
            if (isInBoundry(viewpoint.transform.position))
            {
                GameObject viewpointPrefab = Instantiate(ViewPointPrefab, viewpoint.transform.position, Quaternion.identity);

                GenerateCriticalPoint(viewpoint);
            }

            if (bMesh)
            {
                GenerateVisibilityEffectWithMesh(viewpoint, criticalPoints);
            }
        }
        */


    }

    // Update is called once per frame
    void Update()
    {
        
        if (!drawobstacle.bUserDefine)
        {
            viewpoint = GameObject.FindGameObjectWithTag("ViewPoint");

            if (viewpoint)
            {
                // TODO replace (0,0)
                viewpoint.transform.position = new Vector2(GetMousePosition().x, GetMousePosition().y);
                if (isInBoundry(viewpoint.transform.position))
                {
                    GameObject viewpointPrefab = Instantiate(ViewPointPrefab, viewpoint.transform.position, Quaternion.identity);

                    GenerateCriticalPoint(viewpoint);
                    Destroy(viewpointPrefab, 0.02f);
                }

                if (bMesh)
                {
                    GenerateVisibilityEffectWithMesh(viewpoint, criticalPoints);
                }
            }
        }
        else
        {
            // update the obstacles and boundry in advance
            BoundaryLine = drawboundary.GetBoundaryLine();
            ObstaclesLine = drawobstacle.GetObstacles();
        }
        
    }

    // Determine whether a point is inside boundry or not
    private bool isInBoundry(Vector2 point)
    {
        int BoundaryLength = BoundaryLine.Length, i = 0;
        bool inside = false;
        float pointX = point.x, pointY = point.y;
        float startX, startY, endX, endY;
        Vector2 endpoint = BoundaryLine[BoundaryLine.Length - 1];
        endX = endpoint.x;
        endY = endpoint.y;
        while (i < BoundaryLength)
        {
            startX = endX;
            startY = endY;
            endpoint = BoundaryLine[i++];
            endX = endpoint.x;
            endY = endpoint.y;
            inside ^= (endY > pointY ^ startY > pointY) && ((pointX - endX) < (pointY - endY) * (startX - endX) / (startY - endY));
        }
        return inside;
    }

    /**
     * 
     * @invariant viewpoint != null
     */
    private void GenerateCriticalPoint(GameObject viewpoint)
    {
        criticalPoints.Clear();
        if (BoundaryLine.Length != 0)
        {
            foreach (DrawObstacle.Obstacle obstacleLine in ObstaclesLine)
            {
                GenerateLineCast(viewpoint, obstacleLine.obstaclePoints);
            }
            

            GenerateLineCast(viewpoint, BoundaryLine);
        }
        else
        {
            Debug.Log("No element in BoundaryLine.");
        }

        /*
        foreach (Vector2 critical in criticalPoints)
        {
            Debug.Log(critical);
        }
        */

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
        Vector2 hitPoint = new Vector2();

        for (int i = 0; i < endPoints.Length; i++)
        {
            Vector2 direction = endPoints[i] - viewpoint.transform.position;

            RaycastHit2D[] rayCastHits2D = Physics2D.RaycastAll(viewpoint.transform.position, direction);

            foreach (RaycastHit2D rayCastHit2D in rayCastHits2D)
            {
                // if the hit result is the same position as obstacle position
                if (Math.Abs(rayCastHit2D.point.x - endPoints[i].x) < ACCURACY && Math.Abs(rayCastHit2D.point.y - endPoints[i].y) < ACCURACY)
                {

                    // If the neighbour endpoints of the hitting result are both in the one side, keep the hitting result
                    Vector3 prev = endPoints[(i + endPoints.Length - 1) % endPoints.Length];
                    Vector3 next = endPoints[(i + 1) % endPoints.Length];

                    if (AreSameSide(endPoints[i] - viewpoint.transform.position, prev - viewpoint.transform.position, next - viewpoint.transform.position))
                    {
                        // TODO delete debug
                        // Instantiate(ViewPointPrefab, rayCastHit2D.point, Quaternion.identity);
                        criticalPoints.AddFirst(rayCastHit2D.point);
                        continue;
                    }
                    else
                    {
                        // that is what I want
                        // TODO delete debug
                        //  Instantiate(ViewPointPrefab, rayCastHit2D.point, Quaternion.identity);
                        hitPoint = rayCastHit2D.point;
                        criticalPoints.AddFirst(hitPoint);
                        break;
                    }
                }
                else
                {
                    // TODO delete debug
                    // Instantiate(ViewPointPrefab, rayCastHit2D.point, Quaternion.identity);
                    hitPoint = rayCastHit2D.point;
                    criticalPoints.AddFirst(hitPoint);
                    break;
                }
            }
            // if (!bMesh)
            {
                GenerateVisibilityEffectWithLine(viewpoint, hitPoint);
            }
        }
    }

    private void GenerateVisibilityEffectWithMesh(GameObject viewpoint, LinkedList<Vector2> criticalPoints)
    {
        /*
        Debug.Log("before sort" + criticalPoints.Count);
        foreach (Vector2 v1 in criticalPoints)
        {
            Debug.Log(v1);
        }
        */
        List<Vector2> sortedcriticalPointsList = criticalPoints.ToList<Vector2>();
        sortedcriticalPointsList.Sort(compareByAngle);
        Debug.Log("after sort" + sortedcriticalPointsList.Count);

        
        foreach (Vector2 v1 in sortedcriticalPointsList)
        {
            Debug.Log(v1);
        }


        GenerateMeshTriangle(viewpoint, sortedcriticalPointsList);
    }

    private void GenerateMeshTriangle(GameObject viewpoint, List<Vector2> criticalPoints)
    {
        MeshFilter meshfilter = MeshManager.GetComponent<MeshFilter>();
        Mesh mesh = meshfilter.mesh;

        
        // Vertices
        criticalPoints.Add(viewpoint.transform.position);
        Vector3[] vertices = new Vector3[criticalPoints.Count];
        for (int i = 0; i < criticalPoints.Count; i++ )
        {
            vertices[i] = criticalPoints[i];
        }

        // Triangles
        int[] triangles;
        GenerateTrianglePositionOrder(vertices.Length - 1, out triangles);
        /*
        foreach(int i in triangles)
        {
            Debug.Log(i + ":" + vertices[i]);
        }
        */
        
        
        mesh.Clear();
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.RecalculateNormals();


        mesh.Clear();
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.RecalculateNormals();

    }

    private void GenerateTrianglePositionOrder(int length, out int[] triangles)
    {
        triangles = new int[length * 3];
        for (int i = 0; i < length; i++)
        {
            triangles[3 * i] = i;
            triangles[3 * i + 1] = (i + 1) % length;
            triangles[3 * i + 2] = length;
        }
    }

    // compare two vector 
    private int compareByAngle(Vector2 v1, Vector2 v2)
    {
        v1 = new Vector2(v1.x - viewpoint.transform.position.x, v1.y - viewpoint.transform.position.y);
        v2 = new Vector2(v2.x - viewpoint.transform.position.x, v2.y - viewpoint.transform.position.y);

        float angle1 = Vector2.Angle(v1, new Vector2(1, 0));
        float angle2 = Vector2.Angle(v2, new Vector2(1, 0));

        if (v1.y == 0 || v2.y == 0)
        {
            if (v1.y == 0 && v2.y == 0)
            {
                if (v1.x > 0)
                {
                    if(v2.x > 0)
                    {
                        return v1.x > v2.x ? 1 : -1;
                    } else
                    {
                        return -1;
                    }
                }
                else
                {
                    if (v2.x > 0)
                    {
                        return 1;
                    }
                    else
                    {
                        return v1.x > v2.x ? -1 : 1;
                    }
                }
            }
            else if (v1.y == 0)
            {
                if (v1.x > 0)
                {
                    return 1;
                }
                else
                {
                    return v2.y > 0 ? -1 : 1;
                }
            }
            else
            {
                if (v2.x > 0)
                {
                    return -1;
                }
                else
                {
                    return v1.y > 0 ? 1 : -1;
                }
            }
        }
        else if (v1.y * v2.y > 0)
        {
            if (v1.y > 0)
            {
                if (angle1 == angle2)
                {
                    return v1.y > v2.y ? -1 : 1;
                } else
                {
                    return angle1 < angle2 ? 1 : -1;
                }
            }
            else
            {
                if (angle1 == angle2)
                {
                    return v1.y > v2.y ? -1 : 1;
                }
                else
                {
                    return angle1 < angle2 ? -1 : 1;
                }
            }
        }
        else
        {
            return v1.y > 0 ? 1 : -1;
        }
    }

    private void GenerateVisibilityEffectWithLine(GameObject viewpoint, Vector2 hitPoint)
    {
        LinkedList<GameObject> crticalpointsPrefab = new LinkedList<GameObject>();
        LinkedList<LineRenderer> criticalpointsLineRenderers = new LinkedList<LineRenderer>();
        crticalpointsPrefab.AddLast(Instantiate(IntersectionPointPrefab, hitPoint, Quaternion.identity));

        // use line renderer to generate the debug line
        GameObject newLineGen = Instantiate(lineGeneratorPrefab);
        LineRenderer lRend = newLineGen.GetComponent<LineRenderer>();
        if (lRend)
        {
            // viewpoint and the end points
            lRend.positionCount = 2;
            lRend.SetPosition(0, new Vector3(viewpoint.transform.position.x, viewpoint.transform.position.y, 0));
            lRend.SetPosition(1, new Vector3(hitPoint.x, hitPoint.y, 0));
            criticalpointsLineRenderers.AddLast(lRend);
        }
        else
        {
            Debug.Log("Something bad!!");
        }

        foreach (GameObject criticalpointPrefab in crticalpointsPrefab)
        {
            Destroy(criticalpointPrefab, 0.02f);
        }

        foreach (LineRenderer lineRenderer in criticalpointsLineRenderers)
        {
            Destroy(lineRenderer, 0.03f);
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
