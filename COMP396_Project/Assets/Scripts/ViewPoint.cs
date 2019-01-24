using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ViewPoint : MonoBehaviour
{
    // Cached
    private GameObject viewpoint = GameObject.FindGameObjectWithTag("ViewPoint");
    // private Vector3[] BoundaryLine =(DrawObstacle)GameObject.FindGameObjectWithTag("Boundary").GetBoundaryLine();

    // Start is called before the first frame update
    void Start()
    {
        if (viewpoint)
        {

            // Physics2D.Linecast(viewpoint.transform.position, )
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
