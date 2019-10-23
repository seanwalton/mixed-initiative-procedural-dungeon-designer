using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

public class PathFinder 
{
    /// <summary>
    /// Looks for a path returns true if a valid path exists, false otherwise
    /// </summary>
    /// <param name="start">The starting position</param>
    /// <param name="target">The destination the path should lead to</param>
    /// <param name="genome">The map</param>
    /// <param name="path">The path created</param>
    public static bool FindPath(Vector2Int start, Vector2Int target, DungeonGenome genome, out List<Node> path)
    {
        Debug.Log("looking for path");
        path = new List<Node>();
        Node startNode = new Node(start.x, start.y, true);
        Node targetNode = new Node(target.x, target.y, true);

        List<Node> openSet = new List<Node>();
        HashSet<Node> closedSet = new HashSet<Node>();

        openSet.Add(startNode);

        while (openSet.Count > 0)
        {
            Node currentNode = openSet[0];

            openSet.Remove(currentNode);
            closedSet.Add(currentNode);

            if (currentNode.Equals(targetNode))
            {
                path = RetracePath(startNode, currentNode);
                return true;
            }

            foreach (Node neighbour in GetNeighbours(currentNode, genome))
            {
                if (!neighbour.Passable || closedSet.Contains(neighbour))
                    continue;

                int newMovementCostToNeighbour = currentNode.gCost + GetDistance(currentNode, neighbour);

                if (newMovementCostToNeighbour < neighbour.gCost || !openSet.Contains(neighbour))
                {
                    neighbour.gCost = newMovementCostToNeighbour;
                    neighbour.hCost = GetDistance(neighbour, targetNode);
                    neighbour.Parent = currentNode;

                    if (!openSet.Contains(neighbour))
                    {
                        openSet.Add(neighbour);
                        openSet = openSet.OrderBy(a => a.fCost).ToList();
                    }
                }
            }

            openSet = openSet.OrderBy(a => a.fCost).ToList();
        }

        return false;
    }

    private static List<Node> RetracePath(Node startNode, Node endNote)
    {
        List<Node> path = new List<Node>();
        Node currentNode = endNote;

        while (!currentNode.Equals(startNode))
        {
            path.Add(currentNode);

            currentNode = currentNode.Parent;
        }
        Debug.Log("path found, steps: " + path.Count);

        path.Reverse();
        return path;
    }

    private static int GetDistance(Node a, Node b)
    {
        int dstX = Mathf.Abs(a.Position.x - b.Position.x);
        int dstY = Mathf.Abs(a.Position.y - b.Position.y);

        if (dstX > dstY)
            return 14 * dstY + 10 * (dstX - dstY);

        return 14 * dstX + 10 * (dstY - dstX);
    }

    private static List<Node> GetNeighbours(Node node, DungeonGenome genome)
    {
        List<Node> neighbours = new List<Node>();
        bool passable = false;

        for (int x = -1; x <= 1; x++)
        {
            for (int y = -1; y <= 1; y++)
            {
                if ((x == 0 && y == 0) || 
                    (x == 1 && y == 1) || (x == -1 && y == -1) || (x == 1 && y == -1) || (x == -1 && y == 1))
                    continue;

                int nx = node.Position.x + x;
                int ny = node.Position.y + y;

                if ((nx >= 0) && (ny >= 0) && (nx < DungeonGenome.Size) && (ny < DungeonGenome.Size))
                {

                    switch (genome.dungeonMap[nx,ny])
                    {
                        case DungeonTileType.FLOOR:
                            passable = true;
                            break;
                        case DungeonTileType.WALL:
                            passable = false;
                            break;
                        case DungeonTileType.ENEMY:
                            passable = true;
                            break;
                        case DungeonTileType.TREASURE:
                            passable = true;
                            break;
                        case DungeonTileType.ENTRANCE:
                            passable = true;
                            break;
                        case DungeonTileType.EXIT:
                            passable = true;
                            break;
                    }

                    neighbours.Add(new Node(nx, ny, passable));
                }
                    
            }
        }

        return neighbours;
    }
}
