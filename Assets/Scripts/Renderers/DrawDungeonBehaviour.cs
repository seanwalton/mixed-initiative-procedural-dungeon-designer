using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class DrawDungeonBehaviour : MonoBehaviour
{

    public Tile floorTile;
    public Tile wallTile;
    public Tile enemyTile;
    public Tile treasureTile;
    public Tile entranceTile;
    public Tile exitTile;

    private Tilemap myMap;

    private void Awake()
    {
        myMap = gameObject.GetComponent<Tilemap>();
    }


    //Draws a dungeon starting at position
    public void DrawDungeonFromGenome(DungeonGenome genome, Vector3 position)
    {
        Vector3Int firstCell = myMap.WorldToCell(position);
        Vector3Int currentCell = new Vector3Int();

        for (int i = 0; i < DungeonGenome.Size; i++)
        {
            for (int j = 0; j < DungeonGenome.Size; j++)
            {
                currentCell.x = firstCell.x + i;
                currentCell.y = firstCell.y + j;
                currentCell.z = firstCell.z;
                DrawTileAtLocation(genome.dungeonMap[i, j], currentCell);
            }
        }
    }

    private void DrawTileAtLocation(DungeonTileType type, Vector3Int location)
    {
        switch (type)
        {
            case DungeonTileType.FLOOR:
                myMap.SetTile(location, floorTile);
                break;
            case DungeonTileType.WALL:
                myMap.SetTile(location, wallTile);
                break;
            case DungeonTileType.ENEMY:
                myMap.SetTile(location, enemyTile);
                break;
            case DungeonTileType.TREASURE:
                myMap.SetTile(location, treasureTile);
                break;
            case DungeonTileType.ENTRANCE:
                myMap.SetTile(location, entranceTile);
                break;
            case DungeonTileType.EXIT:
                myMap.SetTile(location, exitTile);
                break;
            
        }
    }


}
