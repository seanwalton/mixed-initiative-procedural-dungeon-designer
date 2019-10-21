using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class DungeonGenome {

    public static int Size = 12;
    public static float WallCoverage = 0.3f;
    public static float EnemyCoverage = 0.02f;
    public static float TreasureCoverage = 0.01f;

    public DungeonTileType[,] dungeonMap = new DungeonTileType[Size, Size];

    public static DungeonGenome CrossOver(DungeonGenome parent1, DungeonGenome parent2)
    {
        DungeonGenome child = new DungeonGenome();

        //The new child must have exactly one entrance and one exit, so lets pick where those come from now
        int exit = Random.Range(0, 2);
        int entrance = Random.Range(0, 2);
        int parent = 0;

        for (int i = 0; i < Size; i++)
        {
            for (int j = 0; j < Size; j++)
            {
                //Check for entrance
                if (( parent1.dungeonMap[i,j] == DungeonTileType.ENTRANCE ) || (parent2.dungeonMap[i, j] == DungeonTileType.ENTRANCE))
                {
                    switch (entrance)
                    {
                        case 0:
                            child.dungeonMap[i, j] = parent1.dungeonMap[i, j];
                            break;
                        case 1:
                            child.dungeonMap[i, j] = parent2.dungeonMap[i, j];
                            break;

                    }
                }
                else if ((parent1.dungeonMap[i, j] == DungeonTileType.EXIT) || (parent2.dungeonMap[i, j] == DungeonTileType.EXIT))
                {
                    switch (exit)
                    {
                        case 0:
                            child.dungeonMap[i, j] = parent1.dungeonMap[i, j];
                            break;
                        case 1:
                            child.dungeonMap[i, j] = parent2.dungeonMap[i, j];
                            break;

                    }
                }
                else
                {
                    parent = Random.Range(0, 2);
                    switch (parent)
                    {
                        case 0:
                            child.dungeonMap[i, j] = parent1.dungeonMap[i, j];
                            break;
                        case 1:
                            child.dungeonMap[i, j] = parent2.dungeonMap[i, j];
                            break;
                        
                    }
                }
            }
        }


        return child;
    }

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

        int value = Random.Range(0, 4);

        if (value == 0)
        {
            MutateSwap();
        }
        else if (value == 1)
        {
            MutateReplace();
        }
        else
        {
            MutateSwap();
            MutateReplace();
        }

       
    }

    private void MutateReplace()
    {
        int i = Random.Range(0, Size);
        int j = Random.Range(0, Size);
        if ((dungeonMap[i, j] != DungeonTileType.ENTRANCE) && (dungeonMap[i, j] != DungeonTileType.EXIT))
        {
            int value = Random.Range(0, 4);

            if (value == 0)
            {
                dungeonMap[i, j] = DungeonTileType.TREASURE;
            }
            else if (value == 1)
            {
                dungeonMap[i, j] = DungeonTileType.ENEMY;
            }
            else if (value == 2)
            {
                dungeonMap[i, j] = DungeonTileType.WALL;
            }
            else
            {
                dungeonMap[i, j] = DungeonTileType.FLOOR;
            }         
        }
        else
        {
            //Do a swap if the entrance or exit comes up
            MutateSwap();
        }
          
    }

    private void MutateSwap()
    {
        int swapDirection = 0;
        DungeonTileType swappedTile;
        int iNew = 0;
        int jNew = 0;

        int i = Random.Range(0, Size);
        int j = Random.Range(0, Size);

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
                if (dungeonMap[i, j] == dungeonMap[i, jNew]) MutateReplace();
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
                if (dungeonMap[i, j] == dungeonMap[i, jNew]) MutateReplace();
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
                if (dungeonMap[i, j] == dungeonMap[iNew, j]) MutateReplace();
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
                if (dungeonMap[i, j] == dungeonMap[iNew, j]) MutateReplace();
                break;
            default:
                break;
        }
                    
               
    }


}
