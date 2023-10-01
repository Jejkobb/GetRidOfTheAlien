using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using cakeslice;

public class MovableObject : MonoBehaviour
{
    public Outline[] outlines;

    public bool moveable = true;

    // Start is called before the first frame update
    void Start()
    {
        foreach (Outline o in outlines)
        {
            o.enabled = false;
        }
    }

    public void EnableOutlines()
    {
        foreach (Outline o in outlines)
        {
            if (moveable == true)
            {
                o.enabled = true;
            }
        }
    }

    public void DisableOutlines()
    {
        foreach (Outline o in outlines)
        {
            o.enabled = false;
        }
    }
}
