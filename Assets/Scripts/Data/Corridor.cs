using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum CorridorType
{
    HORIZONTAL,
    VERTICAL
}

public class Corridor 
{
    public CorridorType Type;

    public List<Vector2Int> Tiles = new List<Vector2Int>();

    public int Length => Tiles.Count;

    public Vector2Int Entrance => Tiles[0];
    public Vector2Int Exit => Tiles[Length - 1];

    public void Add(Vector2Int node)
    {
        Tiles.Add(node);
    }



}
