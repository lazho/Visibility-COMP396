using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class HelpFunction : MonoBehaviour
{
    const double epsilon = 2e-3;

    public static bool floatLess(float value, float other)
    {
        return (other - value) > epsilon;
    }

    public static bool floatGreat(float value, float other)
    {
        return (value - other) > epsilon;
    }

    public static bool floatEqual(float value, float other)
    {
        return Mathf.Abs(value - other) < epsilon;
    }

    public static bool Vector2Equal(Vector2 a, Vector2 b)
    {
        return floatEqual(a.x, b.x) && floatEqual(a.y, b.y);
    }

    public static bool IsInsideSegement(Vector2 detectedPoint, Vector2 endPoint1, Vector2 endPoint2)
    {
        if (Vector2Equal(detectedPoint, endPoint1) || Vector2Equal(detectedPoint, endPoint2))
        {
            return true;
        }

        if ((!floatGreat(detectedPoint.x, endPoint1.x) && !floatLess(detectedPoint.x, endPoint2.x))
            || (!floatGreat(detectedPoint.x, endPoint2.x) && !floatLess(detectedPoint.x, endPoint1.x)))
        {
            return Vector2Equal((detectedPoint - endPoint2).normalized, (endPoint1 - detectedPoint).normalized);
        }
        return false;
    }

    /// <summary>
    /// Determine a point is in obstacle or not
    /// </summary>
    /// <param name="detectedPoint"></param>
    /// <param name="obstaclePoints"></param>
    /// <returns></returns>
    public static bool isInObstacle(Vector2 detectedPoint, Vector3[] obstaclePoints)
    {
        for (int i = 0; i < obstaclePoints.Length; i++)
        {
            if (IsInsideSegement(detectedPoint, obstaclePoints[i], obstaclePoints[(i + 1) % obstaclePoints.Length]))
            {
                return true;
            }
        }
        return false;
    }

    public static bool isInSameLineOfObstacle(Vector2 detectedPoint1, Vector2 detectedPoint2, Vector3[] obstaclePoints)
    {
        for (int i = 0; i < obstaclePoints.Length; i++)
        {
            if (IsInsideSegement(detectedPoint1, obstaclePoints[i], obstaclePoints[(i + 1) % obstaclePoints.Length])
                && IsInsideSegement(detectedPoint2, obstaclePoints[i], obstaclePoints[(i + 1) % obstaclePoints.Length]))
            {
                return true;
            }
        }
        return false;
    }

    /// <summary>
    /// Detect whether the given ray intersects with the the given segement
    /// </summary>
    /// <param name="ray">the given ray</param>
    /// <param name="p1">one of the endpoint of the segement</param>
    /// <param name="p2">another one of the endpoint of the segement</param>
    /// <returns></returns>
    private static bool IsDetectIntersect(Ray2D ray, Vector2 p1, Vector2 p2)
    {
        float pointY;//交点Y坐标，x固定值
        if (floatEqual(p1.x, p2.x))
        {
            return false;
        }
        else if (floatEqual(p1.y, p2.y))
        {
            pointY = p1.y;
        }
        else
        {
            //直线两点式方程：(y-y2)/(y1-y2) = (x-x2)/(x1-x2)
            float a = p1.x - p2.x;
            float b = p1.y - p2.y;
            float c = p2.y / b - p2.x / a;

            pointY = b / a * ray.origin.x + b * c;
        }

        if (floatLess(pointY, ray.origin.y))
        {
            //交点y小于射线起点y
            return false;
        }
        else
        {
            Vector3 leftP = floatLess(p1.x, p2.x) ? p1 : p2;//左端点
            Vector3 rightP = floatLess(p1.x, p2.x) ? p2 : p1;//右端点
            //交点x位于线段两个端点x之外，相交与线段某个端点时，仅将射线L与左侧多边形一边的端点记为焦点(即就是：只将右端点记为交点)
            if (!floatGreat(ray.origin.x, leftP.x) || floatGreat(ray.origin.x, rightP.x))
            {
                return false;
            }
        }
        return true;
    }

    /// <summary>
    /// Detect the given point is in the polygon or not
    /// </summary>
    /// <param name="point">the given point</param>
    /// <param name="polygonVertices">the rest of the vertices</param>
    /// <returns>true: the given point is inside the polygon，false: otherwise</returns>
    public static bool IsPointInsidePolygon(Vector2 point, List<Vector3> polygonVertices)
    {
        int len = polygonVertices.Count;
        Ray2D ray = new Ray2D(point, new Vector3(0, 1)); //y方向射线
        int interNum = 0;

        for (int i = 1; i < len; i++)
        {
            if (IsDetectIntersect(ray, polygonVertices[i - 1], polygonVertices[i]))
            {
                interNum++;
            }
        }

        //不是闭环
        if (!Vector2Equal(polygonVertices[0], polygonVertices[len - 1]))
        {
            if (IsDetectIntersect(ray, polygonVertices[len - 1], polygonVertices[0]))
            {
                interNum++;
            }
        }
        int remainder = interNum % 2;
        return remainder == 1;
    }

    /// <summary>
    /// Detect whether the point can split or not: 
    /// Detect whether new polygon have vertices in the splited triangle
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
                if (IsPointInsidePolygon(verts[i], triangleVert))
                {
                    return false;
                }
            }
        }
        return true;
    }

    /// <summary>
    /// 三角剖分
    /// 1.寻找一个可划分顶点
    /// 2.分割出新的多边形和三角形
    /// 3.新多边形若为凸多边形，结束；否则继续剖分
    /// 
    /// 寻找可划分顶点
    /// 1.顶点是否为凸顶点：顶点在剩余顶点组成的图形外
    /// 2.新的多边形没有顶点在分割的三角形内
    /// </summary>
    /// <param name="verts">顺时针排列的顶点列表</param>
    /// <param name="indexes">顶点索引列表</param>
    /// <returns>三角形列表</returns>
    public static List<OBSTACLE.Obstacle> triangularization(OBSTACLE.Obstacle obstacle)
    {
        int len = obstacle.obstaclePoints.Length;
        if (len <= 3)
        {
            List<OBSTACLE.Obstacle> newObstacleList = new List<OBSTACLE.Obstacle>();
            newObstacleList.Add(obstacle);
            return newObstacleList;
        }

        int searchIndex = isConcave(obstacle);
        List<int> covexIndex = new List<int>();

        if (searchIndex == -1)
        {
            List<OBSTACLE.Obstacle> newObstacleList = new List<OBSTACLE.Obstacle>();
            newObstacleList.Add(obstacle);
            return newObstacleList;
        }

        //查找可划分顶点
        int canFragementIndex = -1;//可划分顶点索引
        for (int i = 0; i < len; i++)
        {
            List<Vector3> polygon = new List<Vector3>(obstacle.obstaclePoints);
            polygon.RemoveAt(i);
            if (!IsPointInsidePolygon(obstacle.obstaclePoints[i], polygon) && IsFragementIndex(i, obstacle.obstaclePoints.ToList()))
            {
                canFragementIndex = i;
                break;
            }
        }

        if (canFragementIndex < 0)
        {
            throw new Exception("Invalid Argument");
        }

        //用可划分顶点将凹多边形划分为一个三角形和一个多边形
        List<Vector3> tTriangles = new List<Vector3>(3);
        int next = (canFragementIndex == len - 1) ? 0 : canFragementIndex + 1;
        int prev = (canFragementIndex == 0) ? len - 1 : canFragementIndex - 1;
        tTriangles.Add(obstacle.obstaclePoints[prev]);
        tTriangles.Add(obstacle.obstaclePoints[canFragementIndex]);
        tTriangles.Add(obstacle.obstaclePoints[next]);
        //剔除可划分顶点及索引

        List<Vector3> tempObstclePoints = obstacle.obstaclePoints.ToList();
        tempObstclePoints.RemoveAt(canFragementIndex);
        obstacle.obstaclePoints = tempObstclePoints.ToArray();

        // Recursion splitting
        List<OBSTACLE.Obstacle> leaveTriangles = triangularization(obstacle);
        OBSTACLE.Obstacle newObstacle = new OBSTACLE.Obstacle();
        newObstacle.obstaclePoints = tTriangles.ToArray();
        leaveTriangles.Add(newObstacle);
        return leaveTriangles;
    }

    /// <summary>
    /// 凸多边形，顺时针序列，以第1个点来剖分三角形，如下：
    /// 0---1
    /// |   |
    /// 3---2  -->  (0, 1, 2)、(0, 2, 3)
    /// </summary>
    /// <param name="verts">顺时针排列的顶点列表</param>
    /// <param name="indexes">顶点索引列表</param>
    /// <returns>三角形列表</returns>
    public static List<int> ConvexTriangleIndex(List<Vector3> verts, List<int> indexes)
    {
        int len = verts.Count;
        //若是闭环去除最后一点
        if (len > 1 && Vector2Equal(verts[0], verts[len - 1]))
        {
            len--;
        }
        int triangleNum = len - 2;
        List<int> triangles = new List<int>(triangleNum * 3);
        for (int i = 0; i < triangleNum; i++)
        {
            triangles.Add(indexes[0]);
            triangles.Add(indexes[i + 1]);
            triangles.Add(indexes[i + 2]);
        }
        return triangles;
    }

    public static int isConcave(OBSTACLE.Obstacle obstacle)
    {
        //bool result = isClockWise(obstacle.obstaclePoints[0], obstacle.obstaclePoints[1], obstacle.obstaclePoints[2]);
        //for (int i = 1; i < obstacle.obstaclePoints.Length; i++)
        //{
        //    if (isClockWise(obstacle.obstaclePoints[i],
        //        obstacle.obstaclePoints[(i + 1) % obstacle.obstaclePoints.Length],
        //        obstacle.obstaclePoints[(i + 2) % obstacle.obstaclePoints.Length]) != result)
        //    {
        //        return i;
        //    }
        //}
        //return -1;
        return isConcave(obstacle.obstaclePoints);
    }

    /// <summary>
    /// Detect whether the given obstacle polygon is concave or not.
    /// </summary>
    /// <param name="obstacle"> the given obstacle </param>
    /// <returns>-1: if the given obstacle is a not a concave polygon, the index of concave point :otherwise </returns>
    public static int isConcave(Vector2[] obstaclePoints)
    {
        bool result = isClockWise(obstaclePoints[0], obstaclePoints[1], obstaclePoints[2]);
        for (int i = 1; i < obstaclePoints.Length; i++)
        {
            if (isClockWise(obstaclePoints[i],
                obstaclePoints[(i + 1) % obstaclePoints.Length],
                obstaclePoints[(i + 2) % obstaclePoints.Length]) != result)
            {
                return i;
            }
        }
        return -1;
    }

    public static int isConcave(Vector3[] obstaclePoints)
    {
        Vector2[] temp = new Vector2[obstaclePoints.Length];
        for (int i = 0; i < obstaclePoints.Length; i++)
        {
            temp[i] = obstaclePoints[i];
        }
        return isConcave(temp);
    }

    public static int isConcave(ViewPoint.HitPoint[] obstaclePoints)
    {
        Vector2[] temp = new Vector2[obstaclePoints.Length];
        for (int i = 0; i < obstaclePoints.Length; i++)
        {
            temp[i] = obstaclePoints[i].location;
        }
        return isConcave(temp);
    }

    public static bool isClockWise(Vector2 a, Vector2 b, Vector2 c)
    {
        return (a.x - c.x) * (b.y - c.y) - (b.x - c.x) * (a.y - c.y) < 0 ? true : false;
    }

    /// <summary>
    /// compare two vector 
    /// </summary>
    /// <param name="v1">vector1</param>
    /// <param name="v2">vector2</param>
    /// <returns>0 -> v1 == v2; -1 -> v1 < v2; 1 -> v1 > v2</returns>
    public static int compareByAngle(Vector2 v1, Vector2 v2)
    {
        v1 = new Vector2(v1.x, v1.y);
        v2 = new Vector2(v2.x, v2.y);

        float angle1 = Vector2.Angle(v1, new Vector2(1, 0));
        float angle2 = Vector2.Angle(v2, new Vector2(1, 0));

        if (v1.y > 0)
        {
            angle1 = 360 - angle1;
        }

        if (v2.y > 0)
        {
            angle2 = 360 - angle2;
        }

        if (HelpFunction.floatEqual(angle1, angle2))
        {
            return 0;
        }
        else
        {
            return angle1 > angle2 ? 1 : -1;
        }
    }

    // clockwise
    public static bool isInsideClockRangeOfTwoVector(Vector2 start, Vector2 end, Vector2 test)
    {
        if (Vector2Equal(test.normalized, start.normalized) || Vector2Equal(test.normalized, end.normalized)) { return true; }
        float angle1 = Vector2.Angle(start, test);
        if (floatGreat(start.x * test.y - start.y * test.x, 0f))
        {
            angle1 = 360 - angle1;
        }

        float angle2 = Vector2.Angle(test, end);
        if (floatGreat(test.x * end.y - test.y * end.x, 0f))
        {
            angle2 = 360 - angle2;
        }

        float angle3 = Vector2.Angle(start, end);
        if (floatGreat(start.x * end.y - start.y * end.x, 0f))
        {
            angle3 = 360 - angle3;
        }

        return floatEqual(angle1 + angle2, angle3);
            
        /*
        if (start.y < 0)
        {
            return compareByAngle(start, test) < 0 && compareByAngle(test, end) < 0;
        }
        else
        {
            if (end.y > 0)
            {
                if (compareByAngle(start, end) < 0)
                {
                    bool test1 = compareByAngle(start, test) < 0 && compareByAngle(test, end) < 0;
                    return compareByAngle(start, test) < 0 && compareByAngle(test, end) < 0;
                }
                else
                {
                    bool test1 = !(compareByAngle(end, test) < 0 && compareByAngle(test, start) < 0);
                    return !(compareByAngle(end, test) < 0 && compareByAngle(test, start) < 0); 
                }
            }
            else
            {
                bool test1 = !(compareByAngle(end, test) < 0 && compareByAngle(test, start) < 0);
                return !(compareByAngle(end, test) < 0 && compareByAngle(test, start) < 0);
            }
        }
        */
    }

    public static float clockwiseAngle(Vector2 from, Vector2 to)
    {
        if (Vector2Equal(from.normalized, to.normalized))
        {
            return 0f;
        }
        return from.x * to.y - from.y * to.x > 0 ? 360 - Vector2.Angle(from, to) : Vector2.Angle(from, to);
    }

    // Find the points of intersection.
    public static int FindLineCircleIntersections( Vector2 center, float radius, Vector2 point1, Vector2 point2,
        out Vector2 intersection1, out Vector2 intersection2)
    { 
        float dx, dy, A, B, C, det, t;

        dx = point2.x - point1.x;
        dy = point2.y - point1.y;

        A = dx * dx + dy * dy;
        B = 2 * (dx * (point1.x - center.x) + dy * (point1.y - center.y));
        C = (point1.x - center.x) * (point1.x - center.x) +
            (point1.y - center.y) * (point1.y - center.y) -
            radius * radius;

        det = B * B - 4 * A * C;
        if (floatLess(A, 0f) || floatLess(det, 0f))
        {
            // No real solutions.
            intersection1 = new Vector2(float.NaN, float.NaN);
            intersection2 = new Vector2(float.NaN, float.NaN);
            return 0;
        }
        else if (floatEqual(det, 0f))
        {
            // One solution.
            t = -B / (2 * A);
            intersection1 = new Vector2(point1.x + t * dx, point1.y + t * dy);
            intersection2 = new Vector2(float.NaN, float.NaN);
            if (IsInsideSegement(intersection1, point1, point2))
            {
                return 1;
            }
            else
            {
                intersection1 = new Vector2(float.NaN, float.NaN);
                return 0;
            }
            
            
        }
        else
        {
            // Two solutions.
            t = (float)((-B + Math.Sqrt(det)) / (2 * A));
            intersection1 = new Vector2(point1.x + t * dx, point1.y + t * dy);
            t = (float)((-B - Math.Sqrt(det)) / (2 * A));
            intersection2 = new Vector2(point1.x + t * dx, point1.y + t * dy);
            int result = 2;
            if (!IsInsideSegement(intersection1, point1, point2))
            {
                intersection1 = new Vector2(float.NaN, float.NaN);
                result--;
            }
            if (!IsInsideSegement(intersection2, point1, point2))
            {
                intersection2 = new Vector2(float.NaN, float.NaN);
                result--;
            }
            return result;
        }
    }

    public static bool isInRangeBound(Vector2 testPoint, Vector2 center, int range)
    {
        return floatEqual((testPoint - center).magnitude, range);
    }

    private void Start()
    {
        //Debug.Log(isInsideClockRangeOfTwoVector(new Vector2(1, 0), new Vector2(-1, 1), new Vector2(15, 8)));
        //Debug.Log(isInsideClockRangeOfTwoVector(new Vector2(1, 0), new Vector2(-1, 1), new Vector2(-15, 8)));
        //Debug.Log(isInsideClockRangeOfTwoVector(new Vector2(1, 0), new Vector2(-1, 1), new Vector2(15, -8)));

        /*
        Debug.Log(clockwiseAngle(new Vector2(1, 1), new Vector2(-1, 1)) + "expected: 270");
        Debug.Log(clockwiseAngle(new Vector2(1, 0), new Vector2(1, -1)) + "expected: 45" );
        Debug.Log(clockwiseAngle(new Vector2(1, 0), new Vector2(1, 1)) + "expected: 315 ");
        Debug.Log(clockwiseAngle(new Vector2(1, 0.3f), new Vector2(1, 0.3f)) + "expected: 0");
        */
        Vector2 result1;
        Vector2 result2;
        Debug.Log(FindLineCircleIntersections(new Vector2(0, 0), 5f, new Vector2(5, 3), new Vector2(6, 3), out result1, out result2));
        Debug.Log(result1 + "" + result2);
        Debug.Log(FindLineCircleIntersections(new Vector2(0, 0), 5f, new Vector2(1, 0), new Vector2(2, 0), out result1, out result2));
        Debug.Log(result1 + "" + result2);
        Debug.Log(FindLineCircleIntersections(new Vector2(0, 0), 5f, new Vector2(6, -1), new Vector2(-1, 6), out result1, out result2));
        Debug.Log(result1 + "" + result2);
        Debug.Log(FindLineCircleIntersections(new Vector2(0, 0), 5f, new Vector2(5, 0), new Vector2(5, 1), out result1, out result2));
        Debug.Log(result1 + "" + result2);
        Debug.Log(FindLineCircleIntersections(new Vector2(0, 0), 5f, new Vector2(6, 0), new Vector2(6, 1), out result1, out result2));
    }

}

