using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cursor : MonoBehaviour
{
    public Transform sightStart, sightEnd;
    public bool spotted = false;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Raycasting();
        Behaviours();
    }

    private void Behaviours()
    {
        //throw new NotImplementedException();
    }

    private void Raycasting()
    {
        Debug.DrawLine(sightStart.position, sightEnd.position, Color.green);
        spotted = Physics2D.Linecast(sightStart.position, sightEnd.position);
    }
}


