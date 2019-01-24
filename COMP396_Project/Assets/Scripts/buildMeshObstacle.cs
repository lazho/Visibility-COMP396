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
            new Vector3(-7, -3, 0),

            // third obstacle
            new Vector3(5, 6, 0),
            new Vector3(6, 6, 0),
            new Vector3(6, 5, 0),

            // fourth
            new Vector3(7, 4, 0),
            new Vector3(16, 5, 0),
            new Vector3(8, 1, 0),


            // fifth
            new Vector3(6, -2, 0),
            new Vector3(12, 0, 0),
            new Vector3(9, -5, 0),
            new Vector3(3, -7, 0)
            
        };

        // Triangles
        int[] triangles = new int[]
        {
            // first
            0, 1, 2,
            0, 2, 3,

            // second
            4, 5, 6,

            // third 
            7, 8, 9,

            // fourth 
            10, 11, 12,

            // fifth
            13, 14, 15,
            13, 15, 16
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
