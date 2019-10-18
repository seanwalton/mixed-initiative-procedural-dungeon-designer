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

        myMap.SetTile(new Vector3Int(0, 0, 0), floorTile);
    }

    


}
