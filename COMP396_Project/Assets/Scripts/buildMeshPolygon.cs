using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class buildMeshPolygon : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        MeshFilter mf = GetComponent<MeshFilter>();
        Mesh mesh = mf.mesh;

        // Vertices
        Vector3[] vertices = new Vector3[]
        {
            new Vector3(-16, -9f, 0),
            new Vector3(-16, 9f, 0),
            new Vector3(16, 9f, 0),
            new Vector3(16, -9f, 0)
        };

        // Triangles
        int[] triangles = new int[]
        {
            0, 1, 2,
            0, 2, 3,

            0, 2, 1,
            0, 3, 2
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
