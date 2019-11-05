using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Corridor 
{
    public List<Vector2Int> Tiles = new List<Vector2Int>();

    public int Length => Tiles.Count;

    public void Add(Vector2Int node)
    {
        Tiles.Add(node);
    }

}
