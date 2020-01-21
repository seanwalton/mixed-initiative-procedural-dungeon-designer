
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class DungeonGenome : System.IComparable
{

    public static int Size = 12;
    public static float WallCoverage = 0.3f;
    public static float EnemyCoverage = 0.02f;
    public static float TreasureCoverage = 0.02f;

    public static int CrossOverType = 0;

    public DungeonTileType[,] DungeonMap = new DungeonTileType[Size, Size];
    public List<Node> PathFromEntranceToExit;
    public bool ValidPath;
    public Vector2Int EntranceLocation;
    public Vector2Int ExitLocation;
    public Fitness MyFitness;

    private DungeonTileType[,] transposeHelper = new DungeonTileType[Size, Size];


    public static int EditDistance(DungeonGenome g1, DungeonGenome g2)
    {
        int dist = 0;
        for (int i = 0; i < Size; i++)
        {
            for (int j = 0; j < Size; j++)
            {
                if (g1.DungeonMap[i, j] != g2.DungeonMap[i, j]) dist++;
            }
        }
        return dist;
    }

    //There are two crossover types with equal chance of occuring. 
    //1) Random mixed crossover element by element
    //2) A random area (square or rectange) in the map is taken from one parent and the rest from the other
    public static DungeonGenome CrossOver(DungeonGenome parent1, DungeonGenome parent2)
    {
        DungeonGenome child = new DungeonGenome();

        //The new child must have exactly one entrance and one exit, so lets pick where those come from now
        int exit = Random.Range(0, 2);
        int entrance = Random.Range(0, 2);
        int crossoverType = CrossOverType;
        int parent = 0;

        //First set entrance and exit
        switch (entrance)
        {
            case 0:
                child.DungeonMap[parent1.EntranceLocation.x, parent1.EntranceLocation.y] = DungeonTileType.ENTRANCE;
                switch (exit)
                {
                    case 0:
                        child.DungeonMap[parent1.ExitLocation.x, parent1.ExitLocation.y] = DungeonTileType.EXIT;
                        break;
                    case 1:
                        if (Vector2Int.Distance(parent2.ExitLocation, parent1.EntranceLocation) == 0)
                        {
                            child.DungeonMap[parent1.ExitLocation.x, parent1.ExitLocation.y] = DungeonTileType.EXIT;
                        }
                        else
                        {
                            child.DungeonMap[parent2.ExitLocation.x, parent2.ExitLocation.y] = DungeonTileType.EXIT;
                        }
                        break;
                }
                break;
            case 1:
                child.DungeonMap[parent2.EntranceLocation.x, parent2.EntranceLocation.y] = DungeonTileType.ENTRANCE;
                switch (exit)
                {
                    case 1:
                        child.DungeonMap[parent2.ExitLocation.x, parent2.ExitLocation.y] = DungeonTileType.EXIT;
                        break;
                    case 0:
                        if (Vector2Int.Distance(parent1.ExitLocation, parent2.EntranceLocation) == 0)
                        {
                            child.DungeonMap[parent2.ExitLocation.x, parent2.ExitLocation.y] = DungeonTileType.EXIT;
                        }
                        else
                        {
                            child.DungeonMap[parent1.ExitLocation.x, parent1.ExitLocation.y] = DungeonTileType.EXIT;
                        }
                        break;
                }
                break;
        }





        Vector2Int crossover1 = new Vector2Int(Random.Range(0, Size-1), Random.Range(0, Size-1));
        Vector2Int crossover2 = new Vector2Int(Random.Range(crossover1.x, Size), Random.Range(crossover1.y, Size));

        int editDistance = EditDistance(parent1, parent2);
        int halfDist = editDistance / 2;
        int numChanges = 0;

        for (int i = 0; i < Size; i++)
        {
            for (int j = 0; j < Size; j++)
            {
                //Check for entrance
                if ((child.DungeonMap[i,j] != DungeonTileType.ENTRANCE) && (child.DungeonMap[i, j] != DungeonTileType.EXIT))
                {

                    switch (crossoverType)
                    {
                        case 0:
                            //Random
                            parent = Random.Range(0, 2);
                            break;
                        case 1:
                            //Fixed point (two points as square)
                            if ((i >= crossover1.x) && (j >= crossover1.y) && (i < crossover2.x) && (j < crossover2.y))
                            {
                                parent = 0;
                            }
                            else
                            {
                                parent = 1;
                            }
                            break;
                        case 2:
                            //Distance based
                            parent = 0;
                            if (parent1.DungeonMap[i,j] != parent2.DungeonMap[i, j])
                            {
                                numChanges++;
                                if (numChanges < halfDist)
                                {
                                    parent = Random.Range(0, 2);
                                }
                            }
                            break;
                    }
                  
                    switch (parent)
                    {
                        case 0:
                            if ((parent1.DungeonMap[i, j] == DungeonTileType.ENTRANCE) || (parent1.DungeonMap[i, j] == DungeonTileType.EXIT))
                            {
                                child.DungeonMap[i, j] = DungeonTileType.FLOOR;
                            }
                            else
                            {
                                child.DungeonMap[i, j] = parent1.DungeonMap[i, j];
                            }               
                            break;
                        case 1:
                            if ((parent2.DungeonMap[i, j] == DungeonTileType.ENTRANCE) || (parent2.DungeonMap[i, j] == DungeonTileType.EXIT))
                            {
                                child.DungeonMap[i, j] = DungeonTileType.FLOOR;
                            }
                            else
                            {
                                child.DungeonMap[i, j] = parent2.DungeonMap[i, j];
                            }
                            break;

                    }
                }
            }
        }

        child.CalculateFitnesses();
        return child;
    }

    public void SetAllFloor()
    {
        //First set everything to floor
        for (int i = 0; i < Size; i++)
        {
            for (int j = 0; j < Size; j++)
            {
                DungeonMap[i, j] = DungeonTileType.FLOOR;
            }
        }
        CalculateFitnesses();
    }
    

    public void CopyFromOtherGenome(DungeonGenome genome)
    {
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

        int value = Random.Range(0,2);

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
            MutateRotate();
        }

        CalculateFitnesses();

    }

    private void MutateRotate()
    {
        //Find start and end point of rotation
        Vector2Int rotation1 = new Vector2Int(Random.Range(0, Size - 1), Random.Range(0, Size - 1));
        Vector2Int rotation2 = new Vector2Int(Random.Range(rotation1.x, Size), Random.Range(rotation1.y, Size));

        for (int i = rotation1.x; i < rotation2.x; i++)
        {
            for (int j = rotation1.y; j < rotation2.y; j++)
            {
                transposeHelper[i, j] = DungeonMap[i, j];
            }
        }

        for (int i = rotation1.x; i < rotation2.x; i++)
        {
            for (int j = rotation1.y; j < rotation2.y; j++)
            {
                DungeonMap[i, j] = transposeHelper[j, i];
            }
        }
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

        bool startExists = false;
        bool endExists = false;

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
                        startExists = true;
                        break;
                    case DungeonTileType.EXIT:
                        target.x = i;
                        target.y = j;
                        ExitLocation.x = i;
                        ExitLocation.y = j;
                        endExists = true;
                        break;
                    default:
                        break;
                }
            }
        }

        if (startExists && endExists)
        {
            ValidPath = PathFinder.FindPath(start, target, this, out PathFromEntranceToExit);
        }
        else
        {
            ValidPath = false;
            PathFromEntranceToExit = new List<Node>();
        }
        
    }

    public void CalculateFitnesses()
    {
        CheckAndFindPath();
        //CheckTreasuresAndEnemiesReachable();
        MyFitness = new Fitness();
        MyFitness.CalculateFitnesses(this);
    }


    private void CheckTreasuresAndEnemiesReachable()
    {
        Vector2Int target = new Vector2Int();

        for (int i = 0; i < Size; i++)
        {
            for (int j = 0; j < Size; j++)
            {
                List<Node> path;

                if ((DungeonMap[i, j] == DungeonTileType.TREASURE) || (DungeonMap[i, j] == DungeonTileType.ENEMY))
                {
                    target.x = i;
                    target.y = j;
                    bool validPath = PathFinder.FindPath(EntranceLocation, target, this, out path);
                    if (!validPath) DungeonMap[i, j] = DungeonTileType.WALL;
                }
            }
        }
    }

    public int CompareTo(object obj)
    {
        DungeonGenome fitnessIn = (DungeonGenome) obj;

        int numWins = 0;
        int numLose = 0;

        for (int i = 0; i < MyFitness.FitnessValues.Count; i++)
        {
            if (MyFitness.FitnessValues[i] < fitnessIn.MyFitness.FitnessValues[i])
            {
                numWins++;
            }

            if (MyFitness.FitnessValues[i] > fitnessIn.MyFitness.FitnessValues[i])
            {
                numLose++;
            }

        }
        return (numLose-numWins);
    }

}
