using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ConnectorType
{
    JOINT, //A joint has 3 or more passable tiles orthogonal to the centre
    TURN //A turn has exactly 2 passable tiles orthogonal to the centre
}

public class Connector
{
    public ConnectorType Type;
    public List<Vector2Int> Tiles = new List<Vector2Int>();
    public int Area => Tiles.Count;

    public void Add(Vector2Int node)
    {
        Tiles.Add(node);
    }

    public void SetType()
    {
        if (Area == 3)
        {
            Type = ConnectorType.TURN;
        }
        else
        {
            Type = ConnectorType.JOINT;
        }
    }

    public bool IsValidConnector()
    {
        return (Area > 2);
    }
}
