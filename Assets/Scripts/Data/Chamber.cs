using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chamber
{
    public Vector2Int StartCorner;
    public Vector2Int EndCorner;
    
    public int Area => (EndCorner.x - StartCorner.x)*(EndCorner.y - StartCorner.y);

    public float Squareness =>
          Area / (float) Mathf.Min((EndCorner.x - StartCorner.x), (EndCorner.y - StartCorner.y));

    public Chamber(Vector2Int startCorner, Vector2Int endCorner)
    {
        StartCorner = startCorner;
        EndCorner = endCorner;
    }
}
