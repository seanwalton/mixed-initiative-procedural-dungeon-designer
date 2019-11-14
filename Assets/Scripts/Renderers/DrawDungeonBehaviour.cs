using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class DrawDungeonBehaviour : MonoBehaviour
{

    public Color PathColor;

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


    public void DrawGeneration(Generation gen, Vector3 position)
    {
        for (int i = 0; i < gen.NumberOfIndividuals; i++)
        {
            DrawDungeonFromGenome(gen.Individuals[i], new Vector3(position.x + i * 1.1f * DungeonGenome.Size,
                position.y, position.z));
        }
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
                DrawTileAtLocation(genome.DungeonMap[i, j], currentCell);
                myMap.SetTileFlags(currentCell, TileFlags.None);
                myMap.SetColor(currentCell, Color.white);

                //if (genome.MyFitness.ConnectorFlag[i, j] > 0)
                //{
                //    myMap.SetColor(currentCell, PathColor);
                //}

                //if (genome.MyFitness.ChamberFlag[i, j] > 0)
                //{
                //    myMap.SetColor(currentCell, PathColor);
                //}

                //if (genome.MyFitness.CorridorFlag[i, j] > 0)
                //{
                //    myMap.SetColor(currentCell, PathColor);
                //}

            }
        }

        if (genome.ValidPath)
        {
            for (int i = 0; i < genome.PathFromEntranceToExit.Count; i++)
            {
                currentCell.x = firstCell.x + genome.PathFromEntranceToExit[i].Position.x;
                currentCell.y = firstCell.y + genome.PathFromEntranceToExit[i].Position.y;
                currentCell.z = firstCell.z;

                myMap.SetTileFlags(currentCell, TileFlags.None);
                myMap.SetColor(currentCell, PathColor);
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
