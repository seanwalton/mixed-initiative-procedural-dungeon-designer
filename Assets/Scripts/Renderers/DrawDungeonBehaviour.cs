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

    public Tilemap MyMap;


    private void Awake()
    {
        MyMap = gameObject.GetComponent<Tilemap>();
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
        Vector3Int firstCell = MyMap.WorldToCell(position);
        Vector3Int currentCell = new Vector3Int();

        for (int i = 0; i < DungeonGenome.Size; i++)
        {
            for (int j = 0; j < DungeonGenome.Size; j++)
            {
                currentCell.x = firstCell.x + i;
                currentCell.y = firstCell.y + j;
                currentCell.z = firstCell.z;
                DrawTileAtLocation(genome.DungeonMap[i, j], currentCell);
                MyMap.SetTileFlags(currentCell, TileFlags.None);
                MyMap.SetColor(currentCell, Color.white);

                //if (genome.MyFitness.ConnectorFlag[i, j] > 0)
                //{
                //    myMap.SetColor(currentCell, PathColor);
                //}

                //if (genome.MyFitness.ChamberFlag[i, j] > 0)
                //{

                //    float t = (genome.MyFitness.ChamberFlag[i, j]) / 
                //        (float) genome.MyFitness.Chambers.Count;
                //    Color chamberColour = Color.Lerp(PathColor, Color.blue, t);
                //    MyMap.SetColor(currentCell, chamberColour);
                //}

                //if (genome.MyFitness.CorridorFlag[i, j] > 0)
                //{
                //    float t = (genome.MyFitness.CorridorFlag[i, j]) /
                //        (float)genome.MyFitness.Corridors.Count;
                //    Color chamberColour = Color.Lerp(PathColor, Color.blue, t);
                //    MyMap.SetColor(currentCell, chamberColour);
                //}

                //if (genome.MyFitness.DeadFlag[i, j] > 0)
                //{
                //    MyMap.SetColor(currentCell, PathColor);
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

                MyMap.SetTileFlags(currentCell, TileFlags.None);
                MyMap.SetColor(currentCell, PathColor);
            }
        }

    }

    private void DrawTileAtLocation(DungeonTileType type, Vector3Int location)
    {
        switch (type)
        {
            case DungeonTileType.FLOOR:
                MyMap.SetTile(location, floorTile);
                break;
            case DungeonTileType.WALL:
                MyMap.SetTile(location, wallTile);
                break;
            case DungeonTileType.ENEMY:
                MyMap.SetTile(location, enemyTile);
                break;
            case DungeonTileType.TREASURE:
                MyMap.SetTile(location, treasureTile);
                break;
            case DungeonTileType.ENTRANCE:
                MyMap.SetTile(location, entranceTile);
                break;
            case DungeonTileType.EXIT:
                MyMap.SetTile(location, exitTile);
                break;
            
        }
    }


}
