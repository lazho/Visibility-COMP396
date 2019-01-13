using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class buildMeshObstacle : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        MeshFilter mf = GetComponent<MeshFilter>();
        Mesh mesh = mf.mesh;

        // Vertices
        Vector3[] vertices = new Vector3[]
        {
            // first obstacle
            new Vector3(-8, 3, 0),
            new Vector3(-7, 7, 0),
            new Vector3(-1, 5, 0),
            new Vector3(-6, -1, 0),

            // second obstacle
            new Vector3(-9, -7, 0),
            new Vector3(-8, 1, 0),
            new Vector3(-7, -3, 0)
        };

        // Triangles
        int[] triangles = new int[]
        {
            // first
            0, 1, 2,
            0, 2, 3,

            // second
            4, 5, 6,
        };

        // UVs

        mesh.Clear();
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.RecalculateNormals();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
