using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class DungeonGenome {

    public static int Size = 64;

    public DungeonTileType[,] dungeonMap = new DungeonTileType[Size, Size];

}
