using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawBoundary : MonoBehaviour
{
    [SerializeField] private GameObject lineGeneratorPrefab;
    [SerializeField]
    private Vector3[] BoundaryLine = new Vector3[4]
    {
        new Vector3(15f, 8f, 0f),
        new Vector3(15f, -8f, 0f),
        new Vector3(-15f, -8f, 0f),
        new Vector3(-15f, 8f, 0f)
    };

    // Start is called before the first frame update
    void Start()
    {
        GenerateNewLine(BoundaryLine);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void GenerateNewLine(Vector3[] linePoints)
    {
        if (linePoints.Length > 1)
        {
            SpawnLineGenerator(linePoints);
        }
        else
        {
            Debug.Log("Need 2 or more points to draw a line.");
        }
    }

    private void SpawnLineGenerator(Vector3[] linePoints)
    {
        GameObject newLineGen = Instantiate(lineGeneratorPrefab);
        LineRenderer lRend = newLineGen.GetComponent<LineRenderer>();

        lRend.positionCount = linePoints.Length;
        lRend.SetPositions(linePoints);
    }
}
