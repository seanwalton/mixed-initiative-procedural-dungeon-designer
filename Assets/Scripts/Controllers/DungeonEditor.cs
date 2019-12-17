using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DungeonEditor : MonoBehaviour
{

    public DungeonGenome Genome = new DungeonGenome();
    public DrawDungeonBehaviour DungeonDrawer;

    public Toggle Liked;

    public void SetGenome(DungeonGenome genome)
    {
        Genome = genome;
        DrawGenome();
    }

    public void SetToggleActive(bool active)
    {
        Liked.gameObject.SetActive(active);
    }

    private void Start()
    {
        Genome.SetAllFloor();
        DrawGenome();
    }

    private void DrawGenome()
    {
        Genome.CalculateFitnesses();
        DungeonDrawer.DrawDungeonFromGenome(Genome, transform.position);
    }

    private void Update()
    {
        if (Input.GetMouseButtonUp(0))
        {
            Vector2Int node = GetTileCoordinatesOfCursor();

            if (((node.x < DungeonGenome.Size) && (node.y < DungeonGenome.Size)) &&
                ((node.x >= 0) && (node.y >= 0)))
            {
                Genome.DungeonMap[node.x, node.y] = NextTileType(Genome.DungeonMap[node.x, node.y]);
                DrawGenome();             
            }
           
        }
    }


    private Vector2Int GetTileCoordinatesOfCursor()
    {
        Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        Vector3Int firstCell = DungeonDrawer.MyMap.WorldToCell(transform.position);

        Vector3Int coordinate = DungeonDrawer.MyMap.WorldToCell(mouseWorldPos) - firstCell;

        return new Vector2Int(coordinate.x, coordinate.y);
    }


    private DungeonTileType NextTileType(DungeonTileType oldTile)
    {
        switch (oldTile)
        {
            case DungeonTileType.FLOOR:
                return DungeonTileType.WALL;
            case DungeonTileType.WALL:
                return DungeonTileType.ENEMY;
            case DungeonTileType.ENEMY:
                return DungeonTileType.TREASURE;
            case DungeonTileType.TREASURE:
                return DungeonTileType.ENTRANCE;
            case DungeonTileType.ENTRANCE:
                return DungeonTileType.EXIT;
            case DungeonTileType.EXIT:
                return DungeonTileType.FLOOR;
            default:
                return DungeonTileType.FLOOR;
        }
    }

}
