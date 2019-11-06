using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chamber
{
    public Vector2Int StartCorner;
    public Vector2Int EndCorner;
    
    public int Size => EndCorner.x - StartCorner.x;

    public Chamber(Vector2Int startCorner, Vector2Int endCorner)
    {
        StartCorner = startCorner;
        EndCorner = endCorner;
    }
}
