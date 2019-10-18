using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class DungeonGenome {

    public static int Size = 12;
    public static float WallCoverage = 0.2f;
    public static float EnemyCoverage = 0.05f;
    public static float TreasureCoverage = 0.05f;

    public static float MutateSwapRate = 0.05f;
    public static float MutateReplaceRate = 0.01f;

    public DungeonTileType[,] dungeonMap = new DungeonTileType[Size, Size];


    public void RandomlyInitialise()
    {
        //First set everything to floor
        for (int i = 0; i < Size; i++)
        {
            for (int j = 0; j < Size; j++)
            {
                dungeonMap[i, j] = DungeonTileType.FLOOR;
            }
        }

        //Now walls
        for (int i = 0; i < Size; i++)
        {
            for (int j = 0; j < Size; j++)
            {
                if (Random.value < WallCoverage) dungeonMap[i, j] = DungeonTileType.WALL;
            }
        }

        //Now enemies
        for (int i = 0; i < Size; i++)
        {
            for (int j = 0; j < Size; j++)
            {
                if (Random.value < EnemyCoverage) dungeonMap[i, j] = DungeonTileType.ENEMY;
            }
        }

        //Now enemies
        for (int i = 0; i < Size; i++)
        {
            for (int j = 0; j < Size; j++)
            {
                if (Random.value < TreasureCoverage) dungeonMap[i, j] = DungeonTileType.TREASURE;
            }
        }

        //Need to set an entrance and exit
        dungeonMap[Random.Range(0, Size), Random.Range(0, Size)] = DungeonTileType.ENTRANCE;

        bool flag = true;
        int x = 0;
        int y = 0;
        while (flag)
        {
            x = Random.Range(0, Size);
            y = Random.Range(0, Size);
            if (dungeonMap[x, y] != DungeonTileType.ENTRANCE)
            {
                dungeonMap[x, y] = DungeonTileType.EXIT;
                flag = false;
            }
        }
    }

    public void Mutate()
    {
        MutateReplace();
    }

    private void MutateReplace()
    {
        int tileInt = 0;
        for (int i = 0; i < Size; i++)
        {
            for (int j = 0; j < Size; j++)
            {

                if (Random.value < MutateReplaceRate)
                {
                    if ((dungeonMap[i, j] != DungeonTileType.ENTRANCE) && (dungeonMap[i, j] != DungeonTileType.EXIT))
                    {
                        tileInt = Random.Range(0, 4);
                        switch (tileInt)
                        {
                            case 0:
                                dungeonMap[i, j] = DungeonTileType.FLOOR;
                                break;
                            case 1:
                                dungeonMap[i, j] = DungeonTileType.WALL;
                                break;
                            case 2:
                                dungeonMap[i, j] = DungeonTileType.ENEMY;
                                break;
                            case 3:
                                dungeonMap[i, j] = DungeonTileType.TREASURE;
                                break;
                            default:
                                break;
                        }
                    }
                }
                    
            }
        }
    }


}
