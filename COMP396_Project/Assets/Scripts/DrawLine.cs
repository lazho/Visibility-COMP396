using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawLine : MonoBehaviour
{
    [SerializeField] private GameObject lineGeneratorPrefab;
    [SerializeField] private GameObject linePointPrefab;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector3 newPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            newPos.z = 0;
            CreatePointMarker(newPos);
        }

        if (Input.GetMouseButtonDown(1))
        {
            clearAllPoints();
        }

        if (Input.GetKeyDown("e"))
        {
            GenerateNewLine();
        }
    }

    private void CreatePointMarker(Vector3 pointPosition)
    {
        Instantiate(linePointPrefab, pointPosition, Quaternion.identity);
    }

    private void clearAllPoints()
    {
        GameObject[] allPoints = GameObject.FindGameObjectsWithTag("PointMarker");

        foreach (GameObject p in allPoints)
        {
            Destroy(p);
        }
    }

    private void GenerateNewLine()
    {
        GameObject[] allPoints = GameObject.FindGameObjectsWithTag("PointMarker");
        Vector3[] allPointPositions = new Vector3[allPoints.Length];

        if (allPoints.Length > 1)
        {
            for (int i = 0; i < allPoints.Length; i++)
            {
                allPointPositions[i] = allPoints[i].transform.position;
            }

            SpawnLineGenerator(allPointPositions);
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

        /*
        lRend.SetPosition(0, new Vector3(-2f, 0f, 0f));
        lRend.SetPosition(1, new Vector3(2f, 0f, 0f));
        lRend.SetPosition(2, new Vector3(-2f, 2f, 0f));
        lRend.SetPosition(3, new Vector3(2f, 2f, 0f));
        */

        // Destroy(newLineGen, 5f);
    }


}
