using System;
using System.Collections.Generic;
using UnityEngine;

public class Fitness
{
    public static int TargetCorridorLength = 4;
    public static int TargetChamberArea = 25;
    public static float TargetPathLength = 0.5f;
    public static float TargetCorridorRatio = 0.5f;
    public static float TargetChamberRatio = 0.5f;

    public static float JointRatio = 0.5f;
    public static float TurnRatio = 0.5f;

    public static float TargetTreasureDensity = 0.02f;
    public static float TargetEnemyDensity = 0.02f;

    public static float EntranceSafety = 0.01f;
    public static float EntranceGreed = 0.01f;

    public static float TargetTreasureSafety = 0.5f;
    public static float TargetTreasureSafetyVariance = 0.2f;

    public static float TargetFractalIndex = 1.35f;

    public static float TargetPassableToImpassableRatio = 0.7f;

    public static float TargetUpDownWallRatio = 0f;
    public static float TargetLeftRightWallRatio = 0f;

    public DungeonGenome Genome;

    public int NumberPassableTiles = 0;
    public int NumberEnemyTiles = 0;
    public int NumberTreasureTiles = 0;
    public int NumberWallTiles = 0;

    public int NumberWallTiles_Left = 0;
    public int NumberWallTiles_Right = 0;
    public int NumberWallTiles_Top = 0;
    public int NumberWallTiles_Bottom = 0;


    public float EnemyDensity = 0.0f;
    public float TreasureDensity = 0.0f;
    public float FractalDimension = 0.0f;
    public float FractalDimensionFitness = 0.0f;

    public float FitnessValue = 0.0f;
    public List<float> FitnessValues = new List<float>();

    private bool validPath;
    private List<Node> path;
    private Vector2Int target;

    private bool[,] passable = new bool[DungeonGenome.Size, DungeonGenome.Size];

    public List<Corridor> Corridors = new List<Corridor>();
    public float CorridorRatio;
    public int[,] CorridorFlag = new int[DungeonGenome.Size, DungeonGenome.Size];

    public List<Chamber> Chambers = new List<Chamber>();
    public float ChamberRatio;
    public int[,] ChamberFlag = new int[DungeonGenome.Size, DungeonGenome.Size];

    public List<Connector> Connectors = new List<Connector>();
    public int[,] ConnectorFlag = new int[DungeonGenome.Size, DungeonGenome.Size];

    public int EntranceSafetyArea;
    public int EntranceGreedArea;

    private List<Vector2Int> treasures = new List<Vector2Int>();
    private List<Vector2Int> enemys = new List<Vector2Int>();

    private List<float> treasureSafety = new List<float>();
    public float MeanTreasureSafety;
    public float TreasureSafetyVariance;

    public float LeftRightWallRatio = 0f;
    public float UpDownWallRatio = 0f;

    public static void SetTargetMetricsFromGenome(DungeonGenome genome)
    {
        genome.CalculateFitnesses();

        TargetFractalIndex = genome.MyFitness.FractalDimension;
        Debug.Log("Target Fractal Dimension " + TargetFractalIndex);

        TargetPathLength = genome.PathFromEntranceToExit.Count / (float)genome.MyFitness.NumberPassableTiles;

        //Debug.Log("TargetPathLength " + TargetPathLength);

        TargetCorridorLength = 0;
        foreach (Corridor c in genome.MyFitness.Corridors)
        {
            TargetCorridorLength += c.Length;
        }
        if (genome.MyFitness.Corridors.Count > 0) TargetCorridorLength /= genome.MyFitness.Corridors.Count;

        //Debug.Log("TargetCorridorLength " + TargetCorridorLength);

        TargetChamberArea = 0;
        foreach (Chamber c in genome.MyFitness.Chambers)
        {
            TargetChamberArea += c.Area;
        }
        if (genome.MyFitness.Chambers.Count > 0) TargetChamberArea /= genome.MyFitness.Chambers.Count;

        //Debug.Log("TargetChamberArea " + TargetChamberArea);

        TargetCorridorRatio = genome.MyFitness.CorridorRatio;
        TargetChamberRatio = genome.MyFitness.ChamberRatio;

        //Debug.Log("TargetCorridorRatio " + TargetCorridorRatio);
        //Debug.Log("TargetChamberRatio " + TargetChamberRatio);


        float numJoints = 0f;
        float numTurns = 0f;

        foreach (Connector c in genome.MyFitness.Connectors)
        {
            if (c.Type == ConnectorType.JOINT) numJoints += 1.0f;
            if (c.Type == ConnectorType.TURN) numTurns += 1.0f;
        }

        if ((numJoints+numTurns) > 0f)
        {
            JointRatio = numJoints / (numJoints + numTurns);
            TurnRatio = numTurns / (numJoints + numTurns);
        }
        else
        {
            JointRatio = 0.5f;
            TurnRatio = 0.5f;
        }

        //Debug.Log("JointRatio " + JointRatio);
        //Debug.Log("TurnRatio " + TurnRatio);

        TargetTreasureDensity = genome.MyFitness.TreasureDensity;
        TargetEnemyDensity = genome.MyFitness.EnemyDensity;

        //Debug.Log("TargetTreasureDensity " + TargetTreasureDensity);
        //Debug.Log("TargetEnemyDensity " + TargetEnemyDensity);

        EntranceSafety = genome.MyFitness.EntranceSafetyArea / (float)genome.MyFitness.NumberPassableTiles;
        EntranceGreed = genome.MyFitness.EntranceGreedArea / (float)genome.MyFitness.NumberPassableTiles;

        //Debug.Log("EntranceSafety " + EntranceSafety);
        //Debug.Log("EntranceGreed " + EntranceGreed);

        TargetTreasureSafety = genome.MyFitness.MeanTreasureSafety;
        TargetTreasureSafetyVariance = genome.MyFitness.TreasureSafetyVariance;

        //Debug.Log("TargetTreasureSafety " + TargetTreasureSafety);
        //Debug.Log("TargetTreasureSafetyVariance " + TargetTreasureSafetyVariance);

        TargetPassableToImpassableRatio = genome.MyFitness.NumberPassableTiles / ((float) genome.MyFitness.NumberWallTiles);

        TargetUpDownWallRatio = genome.MyFitness.UpDownWallRatio;
        TargetLeftRightWallRatio = genome.MyFitness.LeftRightWallRatio;

    }


    public void CalculateFitnesses(DungeonGenome genome)
    {
        Genome = genome;
        CalculateNumberOfTiles();    
        CalculateEnemyCoverage();
        CalculateTreasureCoverage();
        CalculateFractalDimension();
        FindCorridors();
        FindConnectors();
        FindChambers();
        CalculateChamberQualities();
        CalculateCorridorQualities();
        CalculateEntranceSafetyAndGreed();
        CalculateTreasureSafety();

        FitnessValues.Clear();

        float chamberFitness = 1.0f - Mathf.Abs((TargetChamberRatio - ChamberRatio) /
            Mathf.Max(TargetChamberRatio, 1.0f - TargetChamberRatio));
        FitnessValues.Add(chamberFitness);

        float corridorFitness = 1.0f - Mathf.Abs((TargetCorridorRatio - CorridorRatio) /
            Mathf.Max(TargetCorridorRatio, 1.0f - TargetCorridorRatio));
        FitnessValues.Add(corridorFitness);

        


        float safeEntranceFitness = 1.0f - Mathf.Abs(( EntranceSafetyArea / (float)NumberPassableTiles) - EntranceSafety);
        FitnessValues.Add(safeEntranceFitness);

        float greedEntranceFitness = 1.0f - Mathf.Abs((EntranceGreedArea / (float)NumberPassableTiles) - EntranceGreed);
        FitnessValues.Add(greedEntranceFitness);

        float enemyFitness = 1.0f - Mathf.Abs((NumberEnemyTiles / (float)NumberPassableTiles) - TargetEnemyDensity);
        FitnessValues.Add(enemyFitness);

        float treasureFitness = 1.0f - Mathf.Abs((NumberTreasureTiles / (float)NumberPassableTiles) - TargetTreasureDensity);
        FitnessValues.Add(treasureFitness);

        float treasureSafetyFitness = 1.0f - Mathf.Abs(MeanTreasureSafety - TargetTreasureSafety);
        FitnessValues.Add(treasureSafetyFitness);

        float treasureSafetyVarFitness = 1.0f - Mathf.Abs(TreasureSafetyVariance - TargetTreasureSafetyVariance);
        FitnessValues.Add(treasureSafetyVarFitness);

        float difficultyFitness = 1.0f - ((0.1f * safeEntranceFitness) + (0.1f * greedEntranceFitness) +
            (0.3f * enemyFitness) + (0.1f * treasureFitness) + (0.2f * treasureSafetyFitness) + (0.2f * treasureSafetyVarFitness));

        

        //Visual fitnesses
        float numberOfPassableFitness = 1.0f -
            Mathf.Abs((NumberPassableTiles / (float)NumberWallTiles) - TargetPassableToImpassableRatio);
        FitnessValues.Add(numberOfPassableFitness);

        float pathFitness = 1.0f - Mathf.Abs(TargetPathLength - (genome.PathFromEntranceToExit.Count /
            (float)(DungeonGenome.Size * DungeonGenome.Size)));
        FitnessValues.Add(pathFitness);

        float fractalFitness = 1.0f - Mathf.Abs((TargetFractalIndex - FractalDimension) / TargetFractalIndex);
        FitnessValues.Add(fractalFitness);

        float leftRightWallFitness = 1.0f - Mathf.Abs(TargetLeftRightWallRatio - LeftRightWallRatio);
        FitnessValues.Add(leftRightWallFitness);

        float upDownWallFitness = 1.0f - Mathf.Abs(TargetUpDownWallRatio - UpDownWallRatio);
        FitnessValues.Add(upDownWallFitness);

        float patternFitness = (0.25f * chamberFitness) + (0.5f * corridorFitness) + (0.25f * pathFitness);

        FitnessValue = (difficultyFitness + patternFitness + fractalFitness)/3.0f;



    }

    private void CalculateNumberOfTiles()
    {
        NumberPassableTiles = 0;
        NumberEnemyTiles = 0;
        NumberTreasureTiles = 0;
        NumberWallTiles = 0;
        NumberWallTiles_Left = 0;
        NumberWallTiles_Right = 0;
        NumberWallTiles_Top = 0;
        NumberWallTiles_Bottom = 0;

        for (int i = 0; i < DungeonGenome.Size; i++)
        {
            for (int j = 0; j < DungeonGenome.Size; j++)
            {
                passable[i, j] = false;
                if (Genome.DungeonMap[i, j] != DungeonTileType.WALL)
                {
                    NumberPassableTiles++;
                    target.x = i;
                    target.y = j;
                    passable[i, j] = true;

                }
                else
                {
                    NumberWallTiles++;
                    if (i < DungeonGenome.Size/2)
                    {
                        NumberWallTiles_Left++;
                    }
                    else
                    {
                        NumberWallTiles_Right++;
                    }

                    if (j < DungeonGenome.Size / 2)
                    {
                        NumberWallTiles_Bottom++;
                    }
                    else
                    {
                        NumberWallTiles_Top++;
                    }

                }
                if (Genome.DungeonMap[i, j] == DungeonTileType.ENEMY)
                {
                    NumberEnemyTiles++;
                    Vector2Int location = new Vector2Int(i, j);
                    enemys.Add(location);
                }
                if (Genome.DungeonMap[i, j] == DungeonTileType.TREASURE)
                {
                    NumberTreasureTiles++;
                    Vector2Int location = new Vector2Int(i, j);
                    treasures.Add(location);
                }
            }
        }

        UpDownWallRatio = 0f;
        LeftRightWallRatio = 0f;

        if (NumberWallTiles > 0)
        {
            UpDownWallRatio = Mathf.Abs((NumberWallTiles_Top - NumberWallTiles_Bottom) / (float)NumberWallTiles);
            LeftRightWallRatio = Mathf.Abs((NumberWallTiles_Left - NumberWallTiles_Right) / (float)NumberWallTiles);
        }
    }

    private void CalculateEnemyCoverage()
    {
        EnemyDensity = NumberEnemyTiles / (float) NumberPassableTiles;
    }

    private void CalculateTreasureCoverage()
    {
        TreasureDensity = NumberTreasureTiles / (float) NumberPassableTiles;
    }

    private void CalculateFractalDimension()
    {
        


        //We need to calculate the fractal dimension at 3 points
        int[] d = { 1, DungeonGenome.Size / 6, DungeonGenome.Size / 4, DungeonGenome.Size / 2 };
        int[] numWall = { 0, 0, 0, 0 };

        for (int k = 0; k < d.Length; k++)
        {
            for (int i_start = 0; i_start < DungeonGenome.Size; i_start += d[k])
            {
                for (int j_start = 0; j_start < DungeonGenome.Size; j_start += d[k])
                {
                    bool wallFound = false;

                    for (int i = 0; i < d[k]; i++)
                    {
                        for (int j = 0; j < d[k]; j++)
                        {

                            if ((i_start + i < DungeonGenome.Size) && (j_start + j < DungeonGenome.Size) &&
                                    (Genome.DungeonMap[i_start+i, j_start+j] == DungeonTileType.WALL))
                            {
                                wallFound = true;
                            }
                        }
                    }

                    if (wallFound) numWall[k]++;

                }
            }
        }

        float f1 = (Mathf.Log10(numWall[0]) - Mathf.Log10(numWall[1])) /
            (Mathf.Log10(1.0f / d[0]) - Mathf.Log10(1.0f / d[1]));

        float f2 = (Mathf.Log10(numWall[1]) - Mathf.Log10(numWall[2])) /
            (Mathf.Log10(1.0f / d[1]) - Mathf.Log10(1.0f / d[2]));

        float f3 = (Mathf.Log10(numWall[2]) - Mathf.Log10(numWall[3])) /
            (Mathf.Log10(1.0f / d[2]) - Mathf.Log10(1.0f / d[3]));

        FractalDimension = (f1 + f2 + f3)/3.0f;
    }

    private void CalculateEntranceSafetyAndGreed()
    {
        int e_x = Genome.EntranceLocation.x;
        int e_y = Genome.EntranceLocation.y;

        bool foundEnemy = false;
        bool foundTreasure = false;

        EntranceGreedArea = 1;
        EntranceSafetyArea = 1;

        int currentArea = 0;

        while ((!foundEnemy) || (!foundTreasure))
        {
            currentArea++;

            bool foundEnemyThisLoop = false;
            bool foundTreasureThisLoop = false;

            if (!foundEnemy) EntranceSafetyArea = 1;
            if (!foundTreasure) EntranceGreedArea = 1;

            for (int i = -currentArea; i < currentArea; i++)
            {
                for (int j = -currentArea; j < currentArea; j++)
                {
                    int test_x = e_x + i;
                    int test_y = e_y + j;

                    if ((test_x < DungeonGenome.Size) && (test_y < DungeonGenome.Size) &&
                            (test_x >= 0) && (test_y >= 0))
                    {
                        if (!foundEnemy)
                        {
                            if (Genome.DungeonMap[test_x, test_y] == DungeonTileType.ENEMY)
                            {
                                foundEnemyThisLoop = true;
                            }    
                            else if (Genome.DungeonMap[test_x, test_y] == DungeonTileType.FLOOR)
                            {
                                EntranceSafetyArea++;
                            }

                        }
                        

                        if (!foundTreasure)
                        {
                            if (Genome.DungeonMap[test_x, test_y] == DungeonTileType.TREASURE)
                            {
                                foundTreasureThisLoop = true;
                            }
                            else if (Genome.DungeonMap[test_x, test_y] == DungeonTileType.FLOOR)
                            {
                                EntranceGreedArea++;
                            }
                        }
                        
                    }
                }
            }

            if (foundEnemyThisLoop) foundEnemy = true;
            if (foundTreasureThisLoop) foundTreasure = true;

            if (currentArea > DungeonGenome.Size)
            {
                if (!foundEnemy)
                {
                    EntranceSafetyArea = NumberPassableTiles;
                    foundEnemy = true;
                }

                if (!foundTreasure)
                {
                    EntranceGreedArea = NumberPassableTiles;
                    foundTreasure = true;
                }
            }
        }
    }

    private void FindChambers()
    {
        for (int i = 0; i < DungeonGenome.Size; i++)
        {
            for (int j = 0; j < DungeonGenome.Size; j++)
            {
                ChamberFlag[i, j] = -1;
            }
        }

        //Start searching through the dungeon
        for (int i = 0; i < DungeonGenome.Size; i++)
        {
            for (int j = 0; j < DungeonGenome.Size; j++)
            {
                if ((ChamberFlag[i, j] < 0) && (passable[i, j]))
                {
                    //This isn't already assigned to a chamber and it's passable
                    //Since the loop is forward facing we only need to look forward in i and j

                    Vector2Int startCorner = new Vector2Int(i, j);
                    Vector2Int endCorner = new Vector2Int(i, j);

                    int sizeOfChamberSquare = 0;

                    bool endChamber = false;

                    while (!endChamber)
                    {
                        int testSize = sizeOfChamberSquare + 1;

                        bool valid = true;

                        for (int ii = 0; ii <= testSize; ii++)
                        {
                            for (int jj = 0; jj <= testSize; jj++)
                            {
                                int testi = i + ii;
                                int testj = j + jj;

                                if ((testi >= DungeonGenome.Size) || (testj >= DungeonGenome.Size)
                                    || (ChamberFlag[testi, testj] > 0) || (!passable[testi, testj])) valid = false;
                            }
                        }

                        if (valid)
                        {
                            sizeOfChamberSquare++;
                        }
                        else
                        {
                            endChamber = true;
                        }
                    }

                    if (sizeOfChamberSquare > 0)
                    {

                        int size_i = sizeOfChamberSquare;
                        bool endChamber_i = false;

                        while (!endChamber_i)
                        {
                            bool valid = true;

                            int testSize_i = size_i + 1;

                            for (int ii = 0; ii <= testSize_i; ii++)
                            {
                                for (int jj = 0; jj <= sizeOfChamberSquare; jj++)
                                {
                                    int testi = i + ii;
                                    int testj = j + jj;

                                    if ((testi >= DungeonGenome.Size) || (testj >= DungeonGenome.Size)
                                        || (ChamberFlag[testi, testj] > 0) || (!passable[testi, testj])) valid = false;
                                }
                            }

                            if (valid)
                            {
                                size_i++;
                            }
                            else
                            {
                                endChamber_i = true;
                            }
                        }

                        int size_j = sizeOfChamberSquare;
                        bool endChamber_j = false;
                        while (!endChamber_j)
                        {
                            bool valid = true;

                            int testSize_j = size_j + 1;

                            for (int ii = 0; ii <= size_i; ii++)
                            {
                                for (int jj = 0; jj <= testSize_j; jj++)
                                {
                                    int testi = i + ii;
                                    int testj = j + jj;

                                    if ((testi >= DungeonGenome.Size) || (testj >= DungeonGenome.Size)
                                        || (ChamberFlag[testi, testj] > 0) || (!passable[testi, testj])) valid = false;
                                }
                            }

                            if (valid)
                            {
                                size_j++;
                            }
                            else
                            {
                                endChamber_j = true;
                            }
                        }

                        //Enter this Chamber
                        endCorner.Set(i + size_i, j + size_j);
                        Chamber chamber = new Chamber(startCorner, endCorner);
                        Chambers.Add(chamber);
                        for (int ii = 0; ii <= size_i; ii++)
                        {
                            for (int jj = 0; jj <= size_j; jj++)
                            {
                                int testi = i + ii;
                                int testj = j + jj;

                                ChamberFlag[testi, testj] = Chambers.Count;
                            }
                        }


                    }

                }
            }
        }
    }


    private void FindCorridors()
    {
        for (int i = 0; i < DungeonGenome.Size; i++)
        {
            for (int j = 0; j < DungeonGenome.Size; j++)
            {
                CorridorFlag[i, j] = -1;
            }
        }

        //Start searching through the dungeon
        for (int i = 0; i < DungeonGenome.Size; i++)
        {
            for (int j = 0; j < DungeonGenome.Size; j++)
            {
                if ((CorridorFlag[i,j] < 0) && (passable[i,j]))
                {
                    //This isn't already assigned to a corridor and it's passable
                    //Since the loop is forward facing we only need to look forward in i and j

                    //First check in i direction
                    if (AreAboveAndBelowImpassable(i,j))
                    {
                        Corridor corridor = new Corridor();
                        corridor.Type = CorridorType.HORIZONTAL;
                        corridor.Add(new Vector2Int(i, j));
                        CorridorFlag[i, j] = Corridors.Count;
                        int ii = i;
                        bool endCorridor = false;

                        while (!endCorridor)
                        {
                            ii++;
                            if ((ii < DungeonGenome.Size) && passable[ii, j] &&
                                (AreAboveAndBelowImpassable(ii, j)))
                            {
                                corridor.Add(new Vector2Int(ii, j));
                                CorridorFlag[ii, j] = Corridors.Count;
                            }
                            else
                            {
                                endCorridor = true;
                            }
                        }

                        if (corridor.Length > 1) Corridors.Add(corridor);
                    }

                    if (AreLeftAndRightImpassable(i, j))
                    {
                        Corridor corridor = new Corridor();
                        corridor.Type = CorridorType.VERTICAL;
                        corridor.Add(new Vector2Int(i, j));
                        CorridorFlag[i, j] = Corridors.Count;
                        int jj = j;
                        bool endCorridor = false;

                        while (!endCorridor)
                        {
                            jj++;
                            if ((jj < DungeonGenome.Size) && passable[i,jj] &&
                                (AreLeftAndRightImpassable(i, jj)))
                            {
                                corridor.Add(new Vector2Int(i, jj));
                                CorridorFlag[i, jj] = Corridors.Count;
                            }
                            else
                            {
                                endCorridor = true;
                            }
                        }

                        if (corridor.Length > 1) Corridors.Add(corridor);
                    }

                }
            }
        }
    }


    //Finds connectors between corridors and assigns a quality
    private void FindConnectors()
    {
        for (int i = 0; i < DungeonGenome.Size; i++)
        {
            for (int j = 0; j < DungeonGenome.Size; j++)
            {
                ConnectorFlag[i, j] = -1;
            }
        }

        //Loop through corridors
        for (int c = 0; c < Corridors.Count; c++)
        {
            Vector2Int connCentre = new Vector2Int();
            //Look at corridor entrance first
            if (Corridors[c].Type == CorridorType.HORIZONTAL)
            {
                connCentre.Set(Corridors[c].Entrance.x - 1, Corridors[c].Entrance.y);
            }
            else
            {
                connCentre.Set(Corridors[c].Entrance.x, Corridors[c].Entrance.y - 1);
            }

            CheckForConnectorAt(connCentre);

            //Then look at corridor exit
            //Look at corridor entrance first
            if (Corridors[c].Type == CorridorType.HORIZONTAL)
            {
                connCentre.Set(Corridors[c].Exit.x + 1, Corridors[c].Exit.y);
            }
            else
            {
                connCentre.Set(Corridors[c].Exit.x, Corridors[c].Exit.y + 1);
            }

            CheckForConnectorAt(connCentre);
        }
    }

    private void CheckForConnectorAt(Vector2Int connCentre)
    {
        //This can be extracted into its own method
        //Check this is a valid set of coordinates and not already a connector
        if ((connCentre.x >= 1) && (connCentre.y >= 1) &&
            (connCentre.x < DungeonGenome.Size - 1) && (connCentre.y < DungeonGenome.Size - 1) &&
            (ConnectorFlag[connCentre.x, connCentre.y] < 0))
        {
            //Then create Connector object, by adding all orthogonal passables, check it's a valid connector
            //add to global list if it is and update connectorflag

            Connector connector = new Connector();
            connector.Add(connCentre);

            if (passable[connCentre.x + 1, connCentre.y]) connector.Add(new Vector2Int(connCentre.x + 1, connCentre.y));
            if (passable[connCentre.x - 1, connCentre.y]) connector.Add(new Vector2Int(connCentre.x - 1, connCentre.y));
            if (passable[connCentre.x, connCentre.y + 1]) connector.Add(new Vector2Int(connCentre.x, connCentre.y + 1));
            if (passable[connCentre.x, connCentre.y - 1]) connector.Add(new Vector2Int(connCentre.x, connCentre.y - 1));

            if (connector.IsValidConnector())
            {
                connector.SetType();
                Connectors.Add(connector);
                ConnectorFlag[connCentre.x, connCentre.y] = 1;
            }

        }
    }

    private bool AreAboveAndBelowImpassable(int i, int j)
    {
        //Debug.Log(i.ToString() + " " + j.ToString());
        return (((j == 0) || (!passable[i, j - 1]))
                        && ((j == DungeonGenome.Size - 1) || (!passable[i, j + 1])));
    }

    private bool AreLeftAndRightImpassable(int i, int j)
    {
        //Debug.Log(i.ToString() + " " + j.ToString());
        return (((i == 0) || (!passable[i-1,j]))
                        && ((i == DungeonGenome.Size - 1) || (!passable[i+1,j])));
    }


    private void CalculateChamberQualities()
    {
        ChamberRatio = 0f;
        foreach (Chamber chamber in Chambers)
        {
            float q = (0.5f * (Mathf.Min(1.0f, (chamber.Area) / (float) TargetChamberArea))) +
                (0.5f * chamber.Squareness);
            ChamberRatio += ((q * chamber.Area) / (float) NumberPassableTiles);
        }
    }

    private void CalculateCorridorQualities()
    {
        CorridorRatio = 0f;
        foreach (Corridor corridor in Corridors)
        {
            float q = Mathf.Min(1.0f, corridor.Length / (float) TargetCorridorLength);
            CorridorRatio += ((q * corridor.Length) / (float) NumberPassableTiles);
        }

        foreach (Connector connector in Connectors)
        {
            switch (connector.Type)
            {
                case ConnectorType.JOINT:
                    CorridorRatio += (JointRatio * connector.Area) / (float)NumberPassableTiles;
                    break;
                case ConnectorType.TURN:
                    CorridorRatio += (TurnRatio * connector.Area) / (float)NumberPassableTiles;
                    break;
            }

            
        }
    }

    private void CalculateTreasureSafety()
    {

        foreach (Vector2Int treasure in treasures)
        {
            float safety = 0f;
            bool validPath = PathFinder.FindPath(Genome.EntranceLocation, treasure, Genome, out path);

            if (validPath)
            {
                int d_ent = path.Count;
                safety = 1f;

                foreach (Vector2Int enemy in enemys)
                {
                    bool validPathE = PathFinder.FindPath(enemy, treasure, Genome, out path);

                    if (validPathE)
                    {
                        float safety_e = Mathf.Max(0f, ((path.Count - d_ent) / (float) (d_ent + path.Count)));
                        safety = Mathf.Min(safety, safety_e);
                    }
                }
            }

            treasureSafety.Add(safety);

        }

        //Calculate mean and variance of safety
        MeanTreasureSafety = 0f;
        foreach (float s in treasureSafety)
        {
            MeanTreasureSafety += s;
        }

        if (treasureSafety.Count > 0)
        {
            MeanTreasureSafety /= treasureSafety.Count;

            //Calculate variance here...

            TreasureSafetyVariance = 0f;

            foreach (float s in treasureSafety)
            {
                TreasureSafetyVariance += (s - MeanTreasureSafety) * (s - MeanTreasureSafety);
            }

            TreasureSafetyVariance /= treasureSafety.Count;

            TreasureSafetyVariance = Mathf.Sqrt(TreasureSafetyVariance);

        }

    }

    
}
