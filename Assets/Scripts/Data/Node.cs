using UnityEngine;

/// <summary>
/// Stores all information relevant to the A* algorithm
/// </summary>
public class Node
{
    public bool Passable { get; set; }
    public int gCost { get; set; }
    public int hCost { get; set; }
    public Vector2Int Position { get; set; }
    public Node Parent { get; set; }

    public int fCost
    {
        get { return gCost + hCost; }
    }

    public Node(int x, int y, bool passable)
    {
        Position = new Vector2Int(x, y);
        this.Passable = passable;
    }

    public override string ToString()
    {
        return "x: " + Position.x + " y: " + Position.y + " passable: " + Passable;
    }

    public override bool Equals(object obj)
    {
        if (obj == null)
            return false;

        if (obj is Node)
            return Equals(obj as Node);

        return false;
    }

    public override int GetHashCode()
    {
        return Position.x ^ Position.y;
    }

    public bool Equals(Node obj)
    {
        if (obj == null)
        {
            return false;
        }

        return (Position.x == obj.Position.x) && (Position.y == obj.Position.y);
    }
}
