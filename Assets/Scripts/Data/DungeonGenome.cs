using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class DungeonGenome {

    public static int Size = 12;
    public static float WallCoverage = 0.3f;
    public static float EnemyCoverage = 0.02f;
    public static float TreasureCoverage = 0.01f;

    public static float MutateSwapRate = 0.05f;

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
        MutateSwap();
        MutateReplace();
    }

    private void MutateReplace()
    {
        int i = Random.Range(0, Size);
        int j = Random.Range(0, Size);
        if ((dungeonMap[i, j] != DungeonTileType.ENTRANCE) && (dungeonMap[i, j] != DungeonTileType.EXIT))
        {
            float value = Random.value;
            
            if (value < TreasureCoverage)
            {
                dungeonMap[i, j] = DungeonTileType.TREASURE;
            }
            else if (value < EnemyCoverage)
            {
                dungeonMap[i, j] = DungeonTileType.ENEMY;
            }
            else if (value < WallCoverage)
            {
                dungeonMap[i, j] = DungeonTileType.WALL;
            }
            else
            {
                dungeonMap[i, j] = DungeonTileType.FLOOR;
            }         
        }
          
    }

    private void MutateSwap()
    {
        int swapDirection = 0;
        DungeonTileType swappedTile;
        int iNew = 0;
        int jNew = 0;

        for (int i = 0; i < Size; i++)
        {
            for (int j = 0; j < Size; j++)
            {

                if (Random.value < MutateSwapRate)
                {

                    swapDirection = Random.Range(0, 4);
                    swappedTile = dungeonMap[i, j];
                    switch (swapDirection)
                    {
                        case 0:
                            //up
                            if (j == Size - 1)
                            {
                                jNew = 0;
                            }
                            else
                            {
                                jNew = j + 1;
                            }
                            dungeonMap[i, j] = dungeonMap[i, jNew];
                            dungeonMap[i, jNew] = swappedTile;
                            break;
                        case 1:
                            //down
                            if (j == 0)
                            {
                                jNew = Size - 1;
                            }
                            else
                            {
                                jNew = j - 1;
                            }
                            dungeonMap[i, j] = dungeonMap[i, jNew];
                            dungeonMap[i, jNew] = swappedTile;
                            break;
                        case 2:
                            //left
                            if (i == 0)
                            {
                                iNew = Size - 1;
                            }
                            else
                            {
                                iNew = i - 1;
                            }
                            dungeonMap[i, j] = dungeonMap[iNew, j];
                            dungeonMap[iNew, j] = swappedTile;
                            break;
                        case 3:
                            //right
                            if (i == Size - 1)
                            {
                                iNew = 0;
                            }
                            else
                            {
                                iNew = i + 1;
                            }
                            dungeonMap[i, j] = dungeonMap[iNew, j];
                            dungeonMap[iNew, j] = swappedTile;
                            break;
                        default:
                            break;
                    }
                    
                }

            }
        }
    }


}
