﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DungeonEditor : MonoBehaviour
{

    public DungeonGenome Genome = new DungeonGenome();
    public DrawDungeonBehaviour DungeonDrawer;
    public bool Editable = true;
    public Toggle Liked;
    public Toggle Keep;

    public void SetGenome(DungeonGenome genome)
    {
        Genome = genome;
        DrawGenome();
    }

    public void SetToggleActive(bool active)
    {
        Liked.gameObject.SetActive(active);
        Keep.gameObject.SetActive(active);
        Liked.isOn = false;
        Keep.isOn = false;
    }

    private void Start()
    {
        Genome.SetAllFloor();
        SetToggleActive(false);
        DrawGenome();
    }

    private void DrawGenome()
    {
        Genome.CalculateFitnesses();
        DungeonDrawer.DrawDungeonFromGenome(Genome, transform.position);
    }

    private void Update()
    {
        if ((Input.GetMouseButtonUp(0) || Input.GetMouseButtonUp(1)) && Editable)
        {
            Vector2Int node = GetTileCoordinatesOfCursor();

            if (((node.x < DungeonGenome.Size) && (node.y < DungeonGenome.Size)) &&
                ((node.x >= 0) && (node.y >= 0)))
            {

                if (Input.GetMouseButtonUp(1))
                {
                    Genome.DungeonMap[node.x, node.y] = NextTileType(Genome.DungeonMap[node.x, node.y]);
                }
                else
                {
                    Genome.DungeonMap[node.x, node.y] = LastTileType(Genome.DungeonMap[node.x, node.y]);
                }

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

    private DungeonTileType LastTileType(DungeonTileType oldTile)
    {
        switch (oldTile)
        {
            case DungeonTileType.FLOOR:
                return DungeonTileType.WALL;
            case DungeonTileType.ENEMY:
                return DungeonTileType.FLOOR;
            case DungeonTileType.TREASURE:
                return DungeonTileType.ENEMY;
            case DungeonTileType.ENTRANCE:
                return DungeonTileType.TREASURE;
            case DungeonTileType.EXIT:
                return DungeonTileType.ENTRANCE;
            case DungeonTileType.WALL:
                return DungeonTileType.EXIT;
            default:
                return oldTile;
        }
    }

    private DungeonTileType NextTileType(DungeonTileType oldTile)
    {
        switch (oldTile)
        {
            case DungeonTileType.FLOOR:
                return DungeonTileType.ENEMY;
            case DungeonTileType.ENEMY:
                return DungeonTileType.TREASURE;
            case DungeonTileType.TREASURE:
                return DungeonTileType.ENTRANCE;
            case DungeonTileType.ENTRANCE:
                return DungeonTileType.EXIT;
            case DungeonTileType.EXIT:
                return DungeonTileType.WALL;
            case DungeonTileType.WALL:
                return DungeonTileType.FLOOR;          
            default:
                return oldTile;
        }
    }

}
