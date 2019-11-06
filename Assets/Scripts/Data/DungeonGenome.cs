using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class DungeonGenome {

    public static int Size = 12;
    public static float WallCoverage = 0.3f;
    public static float EnemyCoverage = 0.02f;
    public static float TreasureCoverage = 0.01f;

    public DungeonTileType[,] DungeonMap = new DungeonTileType[Size, Size];
    public List<Node> PathFromEntranceToExit;
    public bool ValidPath;
    public Vector2Int EntranceLocation;
    public Vector2Int ExitLocation;
    public Fitness MyFitness;

    public static DungeonGenome CrossOver(DungeonGenome parent1, DungeonGenome parent2)
    {
        DungeonGenome child = new DungeonGenome();

        //The new child must have exactly one entrance and one exit, so lets pick where those come from now
        int exit = Random.Range(0, 2);
        int entrance = Random.Range(0, 2);
        int parent = 0;
        int crossoverPointI = Random.Range(0, Size);
        int crossoverPointJ = Random.Range(0, Size);

        for (int i = 0; i < Size; i++)
        {
            for (int j = 0; j < Size; j++)
            {
                //Check for entrance
                if (( parent1.DungeonMap[i,j] == DungeonTileType.ENTRANCE ) || (parent2.DungeonMap[i, j] == DungeonTileType.ENTRANCE))
                {
                    switch (entrance)
                    {
                        case 0:
                            child.DungeonMap[i, j] = parent1.DungeonMap[i, j];
                            break;
                        case 1:
                            child.DungeonMap[i, j] = parent2.DungeonMap[i, j];
                            break;

                    }
                }
                else if ((parent1.DungeonMap[i, j] == DungeonTileType.EXIT) || (parent2.DungeonMap[i, j] == DungeonTileType.EXIT))
                {
                    switch (exit)
                    {
                        case 0:
                            child.DungeonMap[i, j] = parent1.DungeonMap[i, j];
                            break;
                        case 1:
                            child.DungeonMap[i, j] = parent2.DungeonMap[i, j];
                            break;

                    }
                }
                else
                {
                    //parent = Random.Range(0, 2);

                    if ((i > crossoverPointI) && (j > crossoverPointJ))
                    {
                        parent = 0;
                    }
                    else
                    {
                        parent = 1;
                    }

                    switch (parent)
                    {
                        case 0:
                            child.DungeonMap[i, j] = parent1.DungeonMap[i, j];
                            break;
                        case 1:
                            child.DungeonMap[i, j] = parent2.DungeonMap[i, j];
                            break;
                        
                    }
                }
            }
        }

        child.CalculateFitnesses();
        return child;
    }


    public void CopyFromOtherGenome(DungeonGenome genome)
    {
        //First set everything to floor
        for (int i = 0; i < Size; i++)
        {
            for (int j = 0; j < Size; j++)
            {
                DungeonMap[i, j] = genome.DungeonMap[i,j];
            }
        }
        CalculateFitnesses();
    }

    public void RandomlyInitialise()
    {
        //First set everything to floor
        for (int i = 0; i < Size; i++)
        {
            for (int j = 0; j < Size; j++)
            {
                DungeonMap[i, j] = DungeonTileType.FLOOR;
            }
        }

        //Now walls
        for (int i = 0; i < Size; i++)
        {
            for (int j = 0; j < Size; j++)
            {
                if (Random.value < WallCoverage) DungeonMap[i, j] = DungeonTileType.WALL;
            }
        }

        //Now enemies
        for (int i = 0; i < Size; i++)
        {
            for (int j = 0; j < Size; j++)
            {
                if (Random.value < EnemyCoverage) DungeonMap[i, j] = DungeonTileType.ENEMY;
            }
        }

        //Now enemies
        for (int i = 0; i < Size; i++)
        {
            for (int j = 0; j < Size; j++)
            {
                if (Random.value < TreasureCoverage) DungeonMap[i, j] = DungeonTileType.TREASURE;
            }
        }

        //Need to set an entrance and exit
        DungeonMap[Random.Range(0, Size), Random.Range(0, Size)] = DungeonTileType.ENTRANCE;

        bool flag = true;
        int x = 0;
        int y = 0;
        while (flag)
        {
            x = Random.Range(0, Size);
            y = Random.Range(0, Size);
            if (DungeonMap[x, y] != DungeonTileType.ENTRANCE)
            {
                DungeonMap[x, y] = DungeonTileType.EXIT;
                flag = false;
            }
        }

        CalculateFitnesses();
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

        CalculateFitnesses();

    }

    private void MutateReplace()
    {
        int i = Random.Range(0, Size);
        int j = Random.Range(0, Size);
        if ((DungeonMap[i, j] != DungeonTileType.ENTRANCE) && (DungeonMap[i, j] != DungeonTileType.EXIT))
        {
            int value = Random.Range(0, 4);

            if (value == 0)
            {
                DungeonMap[i, j] = DungeonTileType.TREASURE;
            }
            else if (value == 1)
            {
                DungeonMap[i, j] = DungeonTileType.ENEMY;
            }
            else if (value == 2)
            {
                DungeonMap[i, j] = DungeonTileType.WALL;
            }
            else
            {
                DungeonMap[i, j] = DungeonTileType.FLOOR;
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
        swappedTile = DungeonMap[i, j];
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
                DungeonMap[i, j] = DungeonMap[i, jNew];
                DungeonMap[i, jNew] = swappedTile;
                if (DungeonMap[i, j] == DungeonMap[i, jNew]) MutateReplace();
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
                DungeonMap[i, j] = DungeonMap[i, jNew];
                DungeonMap[i, jNew] = swappedTile;
                if (DungeonMap[i, j] == DungeonMap[i, jNew]) MutateReplace();
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
                DungeonMap[i, j] = DungeonMap[iNew, j];
                DungeonMap[iNew, j] = swappedTile;
                if (DungeonMap[i, j] == DungeonMap[iNew, j]) MutateReplace();
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
                DungeonMap[i, j] = DungeonMap[iNew, j];
                DungeonMap[iNew, j] = swappedTile;
                if (DungeonMap[i, j] == DungeonMap[iNew, j]) MutateReplace();
                break;
            default:
                break;
        }

        


    }

    private void CheckAndFindPath()
    {
        

        Vector2Int start = new Vector2Int();
        Vector2Int target = new Vector2Int();

        for (int i = 0; i < Size; i++)
        {
            for (int j = 0; j < Size; j++)
            {
                switch (DungeonMap[i,j])
                {
                    
                    case DungeonTileType.ENTRANCE:
                        start.x = i;
                        start.y = j;
                        EntranceLocation.x = i;
                        EntranceLocation.y = j;
                        break;
                    case DungeonTileType.EXIT:
                        target.x = i;
                        target.y = j;
                        ExitLocation.x = i;
                        ExitLocation.y = j;
                        break;
                    default:
                        break;
                }
            }
        }

        ValidPath = PathFinder.FindPath(start, target, this, out PathFromEntranceToExit);
    }

    public void CalculateFitnesses()
    {

        CheckAndFindPath();
        MyFitness = new Fitness();
        MyFitness.CalculateFitnesses(this);
    }

}
