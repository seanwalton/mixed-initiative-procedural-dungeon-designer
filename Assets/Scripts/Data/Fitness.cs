﻿using System;
using System.Collections.Generic;
using UnityEngine;

public class Fitness
{
    public static float TargetCorridorLength = 4f;
    public static float TargetChamberArea = 25f;
    public static float TargetPathLength = 0.5f;
    public static float TargetCorridorRatio = 0.5f;
    public static float TargetChamberRatio = 0.5f;

    public static float TargetNumberCorridors = 1f;
    public static float TargetNumberChambers = 1f;

    public static float TargetJointRatio = 0.5f;
    public static float TargetTurnRatio = 0.5f;

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

    public static float TargetUpDownEnemyRatio = 0f;
    public static float TargetLeftRightEnemyRatio = 0f;

    public static float TargetUpDownTreasureRatio = 0f;
    public static float TargetLeftRightTreasureRatio = 0f;

    public static float TargetUpDownTreasureToEnemyRatio = 0f;
    public static float TargetLeftRightTreasureToEnemyRatio = 0f;

    public static int NumberOfTargetGenomes = 0;

    public DungeonGenome Genome;

    public int NumberPassableTiles = 0;
    public int NumberEnemyTiles = 0;
    public int NumberTreasureTiles = 0;
    public int NumberWallTiles = 0;

    public int NumberWallTiles_Left = 0;
    public int NumberWallTiles_Right = 0;
    public int NumberWallTiles_Top = 0;
    public int NumberWallTiles_Bottom = 0;

    public int NumberEnemyTiles_Left = 0;
    public int NumberEnemyTiles_Right = 0;
    public int NumberEnemyTiles_Top = 0;
    public int NumberEnemyTiles_Bottom = 0;

    public int NumberTreasureTiles_Left = 0;
    public int NumberTreasureTiles_Right = 0;
    public int NumberTreasureTiles_Top = 0;
    public int NumberTreasureTiles_Bottom = 0;


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

    public float MeanCorridorLength { get; private set; }

    public int[,] CorridorFlag = new int[DungeonGenome.Size, DungeonGenome.Size];

    public List<Chamber> Chambers = new List<Chamber>();
    public float ChamberRatio;

    public float MeanChamberArea { get; private set; }
    public float JointRatio { get; private set; }
    public float TurnRatio { get; private set; }
    public float PathLength { get; private set; }
    public float PassableToImpassableRatio { get; private set; }

    public int[,] ChamberFlag = new int[DungeonGenome.Size, DungeonGenome.Size];

    public List<Connector> Connectors = new List<Connector>();
    public int[,] ConnectorFlag = new int[DungeonGenome.Size, DungeonGenome.Size];

    public float EntranceSafetyArea;
    public float EntranceGreedArea;

    private List<Vector2Int> treasures = new List<Vector2Int>();
    private List<Vector2Int> enemys = new List<Vector2Int>();

    private List<float> treasureSafety = new List<float>();
    public float MeanTreasureSafety;
    public float TreasureSafetyVariance;

    public float LeftRightWallRatio = 0f;
    public float UpDownWallRatio = 0f;

    public float LeftRightEnemyRatio = 0f;
    public float UpDownEnemyRatio = 0f;

    public float LeftRightTreasureRatio = 0f;
    public float UpDownTreasureRatio = 0f;

    public float LeftRightTreasureToEnemyRatio = 0f;
    public float UpDownTreasureToEnemyRatio = 0f;

    public static void SetTargetMetricsFromGenome(DungeonGenome genome)
    {
        genome.CalculateFitnesses();

        TargetFractalIndex = genome.MyFitness.FractalDimension;

        TargetPathLength = genome.MyFitness.PathLength;
        Debug.Log("Target Path Length " + TargetPathLength.ToString());

        TargetCorridorLength = genome.MyFitness.MeanCorridorLength;
        

        TargetChamberArea = genome.MyFitness.MeanChamberArea;

        TargetNumberChambers = genome.MyFitness.Chambers.Count;
        TargetNumberCorridors = genome.MyFitness.Corridors.Count;


        TargetCorridorRatio = genome.MyFitness.CorridorRatio;
        TargetChamberRatio = genome.MyFitness.ChamberRatio;

        TargetJointRatio = genome.MyFitness.JointRatio;
        TargetTurnRatio = genome.MyFitness.TurnRatio;

        

        TargetTreasureDensity = genome.MyFitness.TreasureDensity;
        TargetEnemyDensity = genome.MyFitness.EnemyDensity;

        EntranceSafety = genome.MyFitness.EntranceSafetyArea;
        EntranceGreed = genome.MyFitness.EntranceGreedArea;

        TargetTreasureSafety = genome.MyFitness.MeanTreasureSafety;
        TargetTreasureSafetyVariance = genome.MyFitness.TreasureSafetyVariance;

        TargetPassableToImpassableRatio = genome.MyFitness.PassableToImpassableRatio;

        TargetUpDownWallRatio = genome.MyFitness.UpDownWallRatio;
        TargetLeftRightWallRatio = genome.MyFitness.LeftRightWallRatio;

        TargetUpDownEnemyRatio = genome.MyFitness.UpDownEnemyRatio;
        TargetLeftRightEnemyRatio = genome.MyFitness.LeftRightEnemyRatio;

        TargetUpDownTreasureRatio = genome.MyFitness.UpDownTreasureRatio;
        TargetLeftRightTreasureRatio = genome.MyFitness.LeftRightTreasureRatio;

        TargetUpDownTreasureToEnemyRatio = genome.MyFitness.UpDownTreasureToEnemyRatio;
        TargetLeftRightTreasureToEnemyRatio = genome.MyFitness.LeftRightTreasureToEnemyRatio;

        NumberOfTargetGenomes = 1;

    }


    public static void UpdateTargetMetricsFromGenome(DungeonGenome genome)
    {
        genome.CalculateFitnesses();

        TargetFractalIndex = ((NumberOfTargetGenomes*TargetFractalIndex) + genome.MyFitness.FractalDimension) / (NumberOfTargetGenomes + 1);

    
        TargetPathLength = ((NumberOfTargetGenomes*TargetPathLength) + 
            (genome.MyFitness.PathLength)) / (NumberOfTargetGenomes + 1);

        Debug.Log("Target Path Length " + TargetPathLength.ToString());


        

        TargetCorridorLength = ((NumberOfTargetGenomes * TargetCorridorLength) + genome.MyFitness.MeanCorridorLength) / (NumberOfTargetGenomes + 1);

        TargetChamberArea = ((NumberOfTargetGenomes * TargetChamberArea) + genome.MyFitness.MeanChamberArea) / (NumberOfTargetGenomes + 1);



        TargetCorridorRatio = ((NumberOfTargetGenomes * TargetCorridorRatio) + genome.MyFitness.CorridorRatio)
            / (NumberOfTargetGenomes + 1);
        TargetChamberRatio = ((NumberOfTargetGenomes * TargetChamberRatio) + genome.MyFitness.ChamberRatio)
            / (NumberOfTargetGenomes + 1);


        TargetNumberChambers = ((NumberOfTargetGenomes * TargetNumberChambers) + genome.MyFitness.Chambers.Count)
            / (NumberOfTargetGenomes + 1);
        TargetNumberCorridors = ((NumberOfTargetGenomes * TargetNumberCorridors) + genome.MyFitness.Corridors.Count)
            / (NumberOfTargetGenomes + 1);

        TargetJointRatio = ((NumberOfTargetGenomes * TargetJointRatio) + genome.MyFitness.JointRatio) / (NumberOfTargetGenomes + 1);
        TargetTurnRatio = ((NumberOfTargetGenomes * TargetJointRatio) + genome.MyFitness.TurnRatio) / (NumberOfTargetGenomes + 1);


        TargetTreasureDensity = ((NumberOfTargetGenomes * TargetTreasureDensity) + genome.MyFitness.TreasureDensity) 
            / (NumberOfTargetGenomes + 1);
        TargetEnemyDensity = ((NumberOfTargetGenomes * TargetEnemyDensity) + genome.MyFitness.EnemyDensity)
            / (NumberOfTargetGenomes + 1);

        EntranceSafety = ((NumberOfTargetGenomes * EntranceSafety) + 
            (genome.MyFitness.EntranceSafetyArea)) / (NumberOfTargetGenomes + 1);

        EntranceGreed = ((NumberOfTargetGenomes * EntranceGreed) + 
            (genome.MyFitness.EntranceGreedArea)) / (NumberOfTargetGenomes + 1);

        TargetTreasureSafety = ((NumberOfTargetGenomes * TargetTreasureSafety) + genome.MyFitness.MeanTreasureSafety) / (NumberOfTargetGenomes + 1);

        TargetTreasureSafetyVariance = ((NumberOfTargetGenomes * TargetTreasureSafetyVariance) 
            + genome.MyFitness.TreasureSafetyVariance) / (NumberOfTargetGenomes + 1);

        TargetPassableToImpassableRatio = ((NumberOfTargetGenomes * TargetPassableToImpassableRatio) 
            + (genome.MyFitness.PassableToImpassableRatio)) / (NumberOfTargetGenomes + 1);

        TargetUpDownWallRatio = ((NumberOfTargetGenomes * TargetUpDownWallRatio) + genome.MyFitness.UpDownWallRatio) / (NumberOfTargetGenomes + 1);
        TargetLeftRightWallRatio = ((NumberOfTargetGenomes * TargetLeftRightWallRatio) + genome.MyFitness.LeftRightWallRatio) / (NumberOfTargetGenomes + 1);

        TargetUpDownEnemyRatio = ((NumberOfTargetGenomes * TargetUpDownEnemyRatio) + genome.MyFitness.UpDownEnemyRatio) / (NumberOfTargetGenomes + 1);
        TargetLeftRightEnemyRatio = ((NumberOfTargetGenomes * TargetLeftRightEnemyRatio) + genome.MyFitness.LeftRightEnemyRatio) / (NumberOfTargetGenomes + 1);

        TargetUpDownTreasureRatio = ((NumberOfTargetGenomes * TargetUpDownTreasureRatio) + genome.MyFitness.UpDownTreasureRatio) / (NumberOfTargetGenomes + 1);
        TargetLeftRightTreasureRatio = ((NumberOfTargetGenomes * TargetLeftRightTreasureRatio) + genome.MyFitness.LeftRightTreasureRatio) / (NumberOfTargetGenomes + 1);

        TargetUpDownTreasureToEnemyRatio = ((NumberOfTargetGenomes * TargetUpDownTreasureToEnemyRatio) + genome.MyFitness.UpDownTreasureToEnemyRatio) / (NumberOfTargetGenomes + 1);
        TargetLeftRightTreasureToEnemyRatio = ((NumberOfTargetGenomes * TargetLeftRightTreasureToEnemyRatio) + genome.MyFitness.LeftRightTreasureToEnemyRatio) / (NumberOfTargetGenomes + 1);

        NumberOfTargetGenomes++;

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
        CalculateConnectorFitness();

        PathLength = genome.PathFromEntranceToExit.Count / (float)NumberPassableTiles;
        PassableToImpassableRatio = NumberPassableTiles / ((float)NumberWallTiles);

        FitnessValues.Clear();
        FitnessValue = 0f;
        float numFitnesses = 0f;

        float chamberFitness = Mathf.Abs(TargetChamberRatio - ChamberRatio);
        FitnessValues.Add(chamberFitness);
        FitnessValue += chamberFitness;
        numFitnesses += 1f;

        float corridorFitness = Mathf.Abs(TargetCorridorRatio - CorridorRatio);
        FitnessValues.Add(corridorFitness);
        FitnessValue += corridorFitness;
        numFitnesses += 1f;

        float numChamberFitness = Mathf.Abs(TargetNumberChambers - Chambers.Count)/
            (DungeonGenome.Size * DungeonGenome.Size);
        FitnessValues.Add(numChamberFitness);
        FitnessValue += numChamberFitness;
        numFitnesses += 1f;

        float numCorridorFitness = Mathf.Abs(TargetNumberCorridors - Corridors.Count) /
            (DungeonGenome.Size * DungeonGenome.Size);
        FitnessValues.Add(numCorridorFitness);
        FitnessValue += numCorridorFitness;
        numFitnesses += 1f;

        float safeEntranceFitness = Mathf.Abs( EntranceSafetyArea - EntranceSafety);
        FitnessValues.Add(safeEntranceFitness);
        FitnessValue += safeEntranceFitness;
        numFitnesses += 1f;

        float greedEntranceFitness = Mathf.Abs(EntranceGreedArea - EntranceGreed);
        FitnessValues.Add(greedEntranceFitness);
        FitnessValue += greedEntranceFitness;
        numFitnesses += 1f;

        float enemyFitness = Mathf.Abs(EnemyDensity - TargetEnemyDensity);
        FitnessValues.Add(enemyFitness);
        FitnessValue += enemyFitness;
        numFitnesses += 1f;

        float treasureFitness = Mathf.Abs(TreasureDensity - TargetTreasureDensity);
        FitnessValues.Add(treasureFitness);
        FitnessValue += treasureFitness;
        numFitnesses += 1f;

        float treasureSafetyFitness = Mathf.Abs(MeanTreasureSafety - TargetTreasureSafety);
        FitnessValues.Add(treasureSafetyFitness);
        FitnessValue += treasureSafetyFitness;
        numFitnesses += 1f;

        float treasureSafetyVarFitness = Mathf.Abs(TreasureSafetyVariance - TargetTreasureSafetyVariance);
        FitnessValues.Add(treasureSafetyVarFitness);
        FitnessValue += treasureSafetyVarFitness;
        numFitnesses += 1f;

        //Visual fitnesses
        float numberOfPassableFitness =  Mathf.Abs(PassableToImpassableRatio - TargetPassableToImpassableRatio);
        FitnessValues.Add(numberOfPassableFitness);
        FitnessValue += numberOfPassableFitness;
        numFitnesses += 1f;

        float pathFitness = Mathf.Abs(TargetPathLength - PathLength);
        FitnessValues.Add(pathFitness);
        FitnessValue += pathFitness;
        numFitnesses += 1f;

        //float fractalFitness = 1.0f - Mathf.Abs(TargetFractalIndex - FractalDimension);
        //FitnessValues.Add(fractalFitness);

        float leftRightWallFitness = Mathf.Abs(TargetLeftRightWallRatio - LeftRightWallRatio);
        FitnessValues.Add(leftRightWallFitness);
        FitnessValue += leftRightWallFitness;
        numFitnesses += 1f;

        float upDownWallFitness = Mathf.Abs(TargetUpDownWallRatio - UpDownWallRatio);
        FitnessValues.Add(upDownWallFitness);
        FitnessValue += upDownWallFitness;
        numFitnesses += 1f;

        float leftRightEnemyFitness = Mathf.Abs(TargetLeftRightEnemyRatio - LeftRightEnemyRatio);
        FitnessValues.Add(leftRightEnemyFitness);
        FitnessValue += leftRightEnemyFitness;
        numFitnesses += 1f;

        float upDownEnemyFitness = Mathf.Abs(TargetUpDownEnemyRatio - UpDownEnemyRatio);
        FitnessValues.Add(upDownEnemyFitness);
        FitnessValue += upDownEnemyFitness;
        numFitnesses += 1f;

        float leftRightTreasureFitness = Mathf.Abs(TargetLeftRightTreasureRatio - LeftRightTreasureRatio);
        FitnessValues.Add(leftRightTreasureFitness);
        FitnessValue += leftRightTreasureFitness;
        numFitnesses += 1f;

        float upDownTreasureFitness = Mathf.Abs(TargetUpDownTreasureRatio - UpDownTreasureRatio);
        FitnessValues.Add(upDownTreasureFitness);
        FitnessValue += upDownTreasureFitness;
        numFitnesses += 1f;

        float leftRightTreasureToEnemyFitness = Mathf.Abs(TargetLeftRightTreasureToEnemyRatio - LeftRightTreasureToEnemyRatio);
        FitnessValues.Add(leftRightTreasureToEnemyFitness);
        FitnessValue += leftRightTreasureToEnemyFitness;
        numFitnesses += 1f;

        float upDownTreasureToEnemyFitness = Mathf.Abs(TargetUpDownTreasureToEnemyRatio - UpDownTreasureToEnemyRatio);
        FitnessValues.Add(upDownTreasureToEnemyFitness);
        FitnessValue += upDownTreasureToEnemyFitness;
        numFitnesses += 1f;

        FitnessValue /= numFitnesses;



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
                    if (i < DungeonGenome.Size / 2)
                    {
                        NumberEnemyTiles_Left++;
                    }
                    else
                    {
                        NumberEnemyTiles_Right++;
                    }

                    if (j < DungeonGenome.Size / 2)
                    {
                        NumberEnemyTiles_Bottom++;
                    }
                    else
                    {
                        NumberEnemyTiles_Top++;
                    }

                    Vector2Int location = new Vector2Int(i, j);
                    enemys.Add(location);
                }
                if (Genome.DungeonMap[i, j] == DungeonTileType.TREASURE)
                {
                    NumberTreasureTiles++;
                    if (i < DungeonGenome.Size / 2)
                    {
                        NumberTreasureTiles_Left++;
                    }
                    else
                    {
                        NumberTreasureTiles_Right++;
                    }

                    if (j < DungeonGenome.Size / 2)
                    {
                        NumberTreasureTiles_Bottom++;
                    }
                    else
                    {
                        NumberTreasureTiles_Top++;
                    }
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

        UpDownEnemyRatio = 0f;
        LeftRightEnemyRatio = 0f;

        if (NumberEnemyTiles > 0)
        {
            UpDownEnemyRatio = Mathf.Abs((NumberEnemyTiles_Top - NumberEnemyTiles_Bottom) / (float)NumberEnemyTiles);
            LeftRightEnemyRatio = Mathf.Abs((NumberEnemyTiles_Left - NumberEnemyTiles_Right) / (float)NumberEnemyTiles);
        }

        UpDownTreasureRatio = 0f;
        LeftRightTreasureRatio = 0f;

        if (NumberTreasureTiles > 0)
        {
            UpDownTreasureRatio = Mathf.Abs((NumberTreasureTiles_Top - NumberTreasureTiles_Bottom) / (float)NumberTreasureTiles);
            LeftRightTreasureRatio = Mathf.Abs((NumberTreasureTiles_Left - NumberTreasureTiles_Right) / (float)NumberTreasureTiles);
        }

        UpDownTreasureToEnemyRatio = 0f;
        LeftRightTreasureToEnemyRatio = 0f;

        if ((NumberEnemyTiles+NumberEnemyTiles) > 0)
        {
            UpDownTreasureToEnemyRatio = Mathf.Abs((NumberTreasureTiles_Top - NumberEnemyTiles_Bottom) / (float)(NumberTreasureTiles+NumberEnemyTiles));
            LeftRightTreasureToEnemyRatio = Mathf.Abs((NumberTreasureTiles_Left - NumberEnemyTiles_Right) / (float)(NumberTreasureTiles + NumberEnemyTiles));
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

        EntranceSafetyArea /= (float) NumberPassableTiles;
        EntranceGreedArea /= (float) NumberPassableTiles;


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

    private void CalculateConnectorFitness()
    {
        float numJoints = 0f;
        float numTurns = 0f;

        foreach (Connector c in Connectors)
        {
            if (c.Type == ConnectorType.JOINT) numJoints += 1.0f;
            if (c.Type == ConnectorType.TURN) numTurns += 1.0f;
        }

        

        if ((numJoints + numTurns) > 0f)
        {
            JointRatio = numJoints / (numJoints + numTurns);
            TurnRatio = numTurns / (numJoints + numTurns);
        }
        else
        {
            JointRatio = 0.5f;
            TurnRatio = 0.5f;
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
        MeanChamberArea = 0f;
        foreach (Chamber chamber in Chambers)
        {
            float q = (0.5f * (Mathf.Min(1.0f, (chamber.Area) / (float) TargetChamberArea))) +
                (0.5f * chamber.Squareness);
            ChamberRatio += ((q * chamber.Area) / (float) NumberPassableTiles);
            MeanChamberArea += chamber.Area;
        }

        if (Chambers.Count > 0) MeanChamberArea /= Chambers.Count;

       
    }

    private void CalculateCorridorQualities()
    {
        CorridorRatio = 0f;
        MeanCorridorLength = 0f;
        foreach (Corridor corridor in Corridors)
        {
            float q = Mathf.Min(1.0f, corridor.Length / (float) TargetCorridorLength);
            CorridorRatio += ((q * corridor.Length) / (float) NumberPassableTiles);
            MeanCorridorLength += corridor.Length;
        }
        if (Corridors.Count > 0) MeanCorridorLength /= (float) Corridors.Count;

        foreach (Connector connector in Connectors)
        {
            switch (connector.Type)
            {
                case ConnectorType.JOINT:
                    CorridorRatio += (TargetJointRatio * connector.Area) / (float)NumberPassableTiles;
                    break;
                case ConnectorType.TURN:
                    CorridorRatio += (TargetTurnRatio * connector.Area) / (float)NumberPassableTiles;
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
