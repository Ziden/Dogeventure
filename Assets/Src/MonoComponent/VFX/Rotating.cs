using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotating : MonoBehaviour
{
    public float X;
    public float Y;
    public float Z;
    
    void Update()
    {
        transform.Rotate(X, Y, Z);
    }
}
