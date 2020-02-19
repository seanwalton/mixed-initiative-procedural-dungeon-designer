using System;
using System.Collections.Generic;
using UnityEngine;

public class Fitness
{
    public static List<float> TargetMeanCorridorLength = new List<float>();
    public static List<float> TargetMeanChamberArea = new List<float>();
    public static List<float> TargetPathLength = new List<float>();

    public static List<float> TargetMaxCorridorLength = new List<float>();
    public static List<float> TargetMinCorridorLength = new List<float>();

    public static List<float> TargetMaxChamberSize = new List<float>();
    public static List<float> TargetMinChamberSize = new List<float>();

    public static List<float> TargetMaxChamberSquareness = new List<float>();
    public static List<float> TargetMinChamberSquareness = new List<float>();
    public static List<float> TargetMeanChamberSquareness = new List<float>();

    public static List<float> TargetNumberCorridors = new List<float>();
    public static List<float> TargetNumberChambers = new List<float>();

    public static List<float> TargetNumberOfJoints = new List<float>();
    public static List<float> TargetNumberOfTurns = new List<float>();

    public static List<float> TargetTreasureDensity = new List<float>();
    public static List<float> TargetEnemyDensity = new List<float>();

    public static List<float> TargetEntranceSafety = new List<float>();
    public static List<float> TargetEntranceGreed = new List<float>();

    public static List<float> TargetTreasureSafety = new List<float>();
    public static List<float> TargetTreasureSafetyVariance = new List<float>();

    public static List<float> TargetFractalIndex = new List<float>();

    public static List<float> TargetPassableToImpassableRatio = new List<float>();

    public static List<float> TargetDeadTileDensity = new List<float>();

    public static List<float> TargetUpDownWallRatio = new List<float>();
    public static List<float> TargetLeftRightWallRatio = new List<float>();

    public static List<float> TargetUpDownEnemyRatio = new List<float>();
    public static List<float> TargetLeftRightEnemyRatio = new List<float>();

    public static List<float> TargetUpDownTreasureRatio = new List<float>();
    public static List<float> TargetLeftRightTreasureRatio = new List<float>();

    public static List<float> TargetUpDownTreasureToEnemyRatio = new List<float>();
    public static List<float> TargetLeftRightTreasureToEnemyRatio = new List<float>();

    public static List<float> TargetRotationalSymmetry = new List<float>();
    public static List<float> TargetISymmetry = new List<float>();
    public static List<float> TargetJSymmetry = new List<float>();

    public static int NumberOfTargetGenomes = 0;

    public static DungeonGenome InitialUserDesign;

    public DungeonGenome Genome;

    public int NumberPassableTiles = 0;
    public int NumberEnemyTiles = 0;
    public int NumberTreasureTiles = 0;
    public int NumberWallTiles = 0;
    public int NumberDeadTiles = 0;

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
    public float DeadTileDensity = 0.0f;
    public float FractalDimension = 0.0f;
    public float FractalDimensionFitness = 0.0f;

    public float FitnessValue = 0.0f;
    public List<float> FitnessValues = new List<float>();

    private bool validPath;
    private List<Node> path;
    private Vector2Int target;

    private bool[,] passable = new bool[DungeonGenome.Size, DungeonGenome.Size];

    public List<Corridor> Corridors = new List<Corridor>();

    public float MeanCorridorLength { get; private set; }

    public int[,] CorridorFlag = new int[DungeonGenome.Size, DungeonGenome.Size];

    public int[,] DeadFlag = new int[DungeonGenome.Size, DungeonGenome.Size];

    public List<Chamber> Chambers = new List<Chamber>();

    public float MeanChamberArea { get; private set; }
    public float NumJoints { get; private set; }
    public float NumTurns { get; private set; }
    public float PathLength { get; private set; }
    public float PassableToImpassableRatio { get; private set; }


    public float MaxCorridorLength;
    public float MinCorridorLength;

    public float MaxChamberSize;
    public float MinChamberSize;
    public float MeanChamberSquareness;
    public float MaxChamberSquareness;
    public float MinChamberSquareness;

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

    public float RotationalSymmetry = 0f;
    public float ISymmetry = 0f;
    public float JSymmetry = 0f;



    public static void SetTargetMetricsFromGenome(DungeonGenome genome)
    {
        genome.CalculateFitnesses();

        InitialUserDesign = new DungeonGenome();
        InitialUserDesign.CopyFromOtherGenome(genome);

        TargetFractalIndex.Add(genome.MyFitness.FractalDimension);

        TargetPathLength.Add(genome.MyFitness.PathLength);

        TargetMeanCorridorLength.Add(genome.MyFitness.MeanCorridorLength);

        TargetMeanChamberArea.Add(genome.MyFitness.MeanChamberArea);

        TargetNumberChambers.Add(genome.MyFitness.Chambers.Count);

        TargetNumberCorridors.Add(genome.MyFitness.Corridors.Count);

        TargetMaxCorridorLength.Add(genome.MyFitness.MaxCorridorLength);

        TargetMinCorridorLength.Add(genome.MyFitness.MinCorridorLength);

        TargetMaxChamberSize.Add(genome.MyFitness.MaxChamberSize);

        TargetMinChamberSize.Add(genome.MyFitness.MinChamberSize);

        TargetMeanChamberSquareness.Add(genome.MyFitness.MeanChamberSquareness);

        TargetMinChamberSquareness.Add(genome.MyFitness.MinChamberSquareness);

        TargetMaxChamberSquareness.Add(genome.MyFitness.MaxChamberSquareness);

        TargetNumberOfJoints.Add(genome.MyFitness.NumJoints);
        TargetNumberOfTurns.Add(genome.MyFitness.NumTurns);

        

        TargetTreasureDensity.Add(genome.MyFitness.TreasureDensity);
        TargetEnemyDensity.Add(genome.MyFitness.EnemyDensity);
        TargetDeadTileDensity.Add(genome.MyFitness.DeadTileDensity);

        TargetEntranceSafety.Add(genome.MyFitness.EntranceSafetyArea);
        TargetEntranceGreed.Add(genome.MyFitness.EntranceGreedArea);

        TargetTreasureSafety.Add(genome.MyFitness.MeanTreasureSafety);
        TargetTreasureSafetyVariance.Add(genome.MyFitness.TreasureSafetyVariance);

        TargetPassableToImpassableRatio.Add(genome.MyFitness.PassableToImpassableRatio);

        TargetUpDownWallRatio.Add(genome.MyFitness.UpDownWallRatio);
        TargetLeftRightWallRatio.Add(genome.MyFitness.LeftRightWallRatio);

        TargetUpDownEnemyRatio.Add(genome.MyFitness.UpDownEnemyRatio);
        TargetLeftRightEnemyRatio.Add(genome.MyFitness.LeftRightEnemyRatio);

        TargetUpDownTreasureRatio.Add(genome.MyFitness.UpDownTreasureRatio);
        TargetLeftRightTreasureRatio.Add(genome.MyFitness.LeftRightTreasureRatio);

        TargetUpDownTreasureToEnemyRatio.Add(genome.MyFitness.UpDownTreasureToEnemyRatio);
        TargetLeftRightTreasureToEnemyRatio.Add(genome.MyFitness.LeftRightTreasureToEnemyRatio);

        TargetRotationalSymmetry.Add(genome.MyFitness.RotationalSymmetry);
        TargetISymmetry.Add(genome.MyFitness.ISymmetry);
        TargetJSymmetry.Add(genome.MyFitness.JSymmetry);

        NumberOfTargetGenomes = 1;

    }


    public static void UpdateTargetMetricsFromGenome(DungeonGenome genome)
    {
        genome.CalculateFitnesses();

        TargetFractalIndex.Add(genome.MyFitness.FractalDimension);

        TargetPathLength.Add(genome.MyFitness.PathLength);

        TargetMeanCorridorLength.Add(genome.MyFitness.MeanCorridorLength);

        TargetMeanChamberArea.Add(genome.MyFitness.MeanChamberArea);

        TargetNumberChambers.Add(genome.MyFitness.Chambers.Count);

        TargetNumberCorridors.Add(genome.MyFitness.Corridors.Count);

        TargetMaxCorridorLength.Add(genome.MyFitness.MaxCorridorLength);

        TargetMinCorridorLength.Add(genome.MyFitness.MinCorridorLength);

        TargetMaxChamberSize.Add(genome.MyFitness.MaxChamberSize);

        TargetMinChamberSize.Add(genome.MyFitness.MinChamberSize);

        TargetMeanChamberSquareness.Add(genome.MyFitness.MeanChamberSquareness);

        TargetMinChamberSquareness.Add(genome.MyFitness.MinChamberSquareness);

        TargetMaxChamberSquareness.Add(genome.MyFitness.MaxChamberSquareness);

        TargetNumberOfJoints.Add(genome.MyFitness.NumJoints);
        TargetNumberOfTurns.Add(genome.MyFitness.NumTurns);



        TargetTreasureDensity.Add(genome.MyFitness.TreasureDensity);
        TargetEnemyDensity.Add(genome.MyFitness.EnemyDensity);
        TargetDeadTileDensity.Add(genome.MyFitness.DeadTileDensity);

        TargetEntranceSafety.Add(genome.MyFitness.EntranceSafetyArea);
        TargetEntranceGreed.Add(genome.MyFitness.EntranceGreedArea);

        TargetTreasureSafety.Add(genome.MyFitness.MeanTreasureSafety);
        TargetTreasureSafetyVariance.Add(genome.MyFitness.TreasureSafetyVariance);

        TargetPassableToImpassableRatio.Add(genome.MyFitness.PassableToImpassableRatio);

        TargetUpDownWallRatio.Add(genome.MyFitness.UpDownWallRatio);
        TargetLeftRightWallRatio.Add(genome.MyFitness.LeftRightWallRatio);

        TargetUpDownEnemyRatio.Add(genome.MyFitness.UpDownEnemyRatio);
        TargetLeftRightEnemyRatio.Add(genome.MyFitness.LeftRightEnemyRatio);

        TargetUpDownTreasureRatio.Add(genome.MyFitness.UpDownTreasureRatio);
        TargetLeftRightTreasureRatio.Add(genome.MyFitness.LeftRightTreasureRatio);

        TargetUpDownTreasureToEnemyRatio.Add(genome.MyFitness.UpDownTreasureToEnemyRatio);
        TargetLeftRightTreasureToEnemyRatio.Add(genome.MyFitness.LeftRightTreasureToEnemyRatio);

        TargetRotationalSymmetry.Add(genome.MyFitness.RotationalSymmetry);
        TargetISymmetry.Add(genome.MyFitness.ISymmetry);
        TargetJSymmetry.Add(genome.MyFitness.JSymmetry);

        NumberOfTargetGenomes++;

    }


    public void CalculateFitnesses(DungeonGenome genome)
    {
        Genome = genome;
        CalculateNumberOfTiles();    
        CalculateEnemyCoverage();
        CalculateTreasureCoverage();
        FindCorridors();
        FindChambers();
        FindConnectors();
        FindDeadTiles();
        CalculateChamberQualities();
        CalculateCorridorQualities();
        CalculateEntranceSafetyAndGreed();
        CalculateTreasureSafety();
        CalculateSymmetryMeasures();
        //CalculateConnectorFitness();

        PathLength = genome.PathFromEntranceToExit.Count / (float) (DungeonGenome.Size * DungeonGenome.Size);
        PassableToImpassableRatio = NumberWallTiles / (float) NumberPassableTiles;
            
        FitnessValues.Clear();
        FitnessValue = 0f;
        float numFitnesses = 0f;

        if (NumberOfTargetGenomes < 1) return;

        float corridorMeanFitness = Mathf.Abs(TargetMeanCorridorLength[0] - MeanCorridorLength) /
            (DungeonGenome.Size * DungeonGenome.Size);

        for (int i = 0; i < NumberOfTargetGenomes-1; i++)
        {
            corridorMeanFitness = Mathf.Min(corridorMeanFitness, Mathf.Abs(TargetMeanCorridorLength[i] - MeanCorridorLength) /
            (DungeonGenome.Size * DungeonGenome.Size));
        }

        FitnessValues.Add(corridorMeanFitness);
        FitnessValue += corridorMeanFitness;
        numFitnesses += 1f;

        float corridorMaxFitness = Mathf.Abs(TargetMaxCorridorLength[0] - MaxCorridorLength) /
            (DungeonGenome.Size * DungeonGenome.Size);

        for (int i = 0; i < NumberOfTargetGenomes - 1; i++)
        {
            corridorMaxFitness = Mathf.Min(corridorMaxFitness, Mathf.Abs(TargetMaxCorridorLength[i] - MaxCorridorLength) /
            (DungeonGenome.Size * DungeonGenome.Size));
        }

        FitnessValues.Add(corridorMaxFitness);
        FitnessValue += corridorMaxFitness;
        numFitnesses += 1f;

        float corridorMinFitness = Mathf.Abs(TargetMinCorridorLength[0] - MinCorridorLength) /
            (DungeonGenome.Size * DungeonGenome.Size);
        for (int i = 0; i < NumberOfTargetGenomes - 1; i++)
        {
            corridorMinFitness = Mathf.Min(corridorMinFitness, Mathf.Abs(TargetMinCorridorLength[i] - MinCorridorLength) /
            (DungeonGenome.Size * DungeonGenome.Size));
        }
        FitnessValues.Add(corridorMinFitness);
        FitnessValue += corridorMinFitness;
        numFitnesses += 1f;

        float chamberMeanFitness = Mathf.Abs(TargetMeanChamberArea[0] - MeanChamberArea) /
            (DungeonGenome.Size * DungeonGenome.Size);
        for (int i = 0; i < NumberOfTargetGenomes - 1; i++)
        {
            chamberMeanFitness = Mathf.Min(chamberMeanFitness, Mathf.Abs(TargetMeanChamberArea[i] - MeanChamberArea) /
            (DungeonGenome.Size * DungeonGenome.Size));
        }
        FitnessValues.Add(chamberMeanFitness);
        FitnessValue += chamberMeanFitness;
        numFitnesses += 1f;

        float chamberMaxFitness = Mathf.Abs(TargetMaxChamberSize[0] - MaxChamberSize) /
            (DungeonGenome.Size * DungeonGenome.Size);
        for (int i = 0; i < NumberOfTargetGenomes - 1; i++)
        {
            chamberMaxFitness = Mathf.Min(chamberMaxFitness, Mathf.Abs(TargetMaxChamberSize[i] - MaxChamberSize) /
            (DungeonGenome.Size * DungeonGenome.Size));
        }
        FitnessValues.Add(chamberMaxFitness);
        FitnessValue += chamberMaxFitness;
        numFitnesses += 1f;

        float chamberMinFitness = Mathf.Abs(TargetMinChamberSize[0] - MinChamberSize) /
            (DungeonGenome.Size * DungeonGenome.Size);
        for (int i = 0; i < NumberOfTargetGenomes - 1; i++)
        {
            chamberMinFitness = Mathf.Min(chamberMinFitness, Mathf.Abs(TargetMinChamberSize[i] - MinChamberSize) /
            (DungeonGenome.Size * DungeonGenome.Size));
        }
        FitnessValues.Add(chamberMinFitness);
        FitnessValue += chamberMinFitness;
        numFitnesses += 1f;

        float chamberSquareFitness = Mathf.Abs(TargetMeanChamberSquareness[0] - MeanChamberSquareness);
        for (int i = 0; i < NumberOfTargetGenomes - 1; i++)
        {
            chamberSquareFitness = Mathf.Min(chamberSquareFitness, 
                Mathf.Abs(TargetMeanChamberSquareness[i] - MeanChamberSquareness)
            );
        }
        FitnessValues.Add(chamberSquareFitness);
        FitnessValue += chamberSquareFitness;
        numFitnesses += 1f;

        float chamberMinSquareFitness = Mathf.Abs(TargetMinChamberSquareness[0] - MinChamberSquareness);
        
        for (int i = 0; i < NumberOfTargetGenomes - 1; i++)
        {
            chamberMinSquareFitness = Mathf.Min(chamberMinSquareFitness,
                Mathf.Abs(TargetMinChamberSquareness[i] - MinChamberSquareness));
        }
        FitnessValues.Add(chamberMinSquareFitness);
        FitnessValue += chamberMinSquareFitness;
        numFitnesses += 1f;

        float chamberMaxSquareFitness = Mathf.Abs(TargetMaxChamberSquareness[0] - MaxChamberSquareness);
        
        for (int i = 0; i < NumberOfTargetGenomes - 1; i++)
        {
            chamberMaxSquareFitness = Mathf.Min(chamberMaxSquareFitness,
                Mathf.Abs(TargetMaxChamberSquareness[i] - MaxChamberSquareness));
        }
        FitnessValues.Add(chamberMaxSquareFitness);
        FitnessValue += chamberMaxSquareFitness;
        numFitnesses += 1f;

        float numChamberFitness = Mathf.Abs(TargetNumberChambers[0] - Chambers.Count)/
            (DungeonGenome.Size * DungeonGenome.Size);
        for (int i = 0; i < NumberOfTargetGenomes - 1; i++)
        {
            numChamberFitness = Mathf.Min(numChamberFitness, Mathf.Abs(TargetNumberChambers[i] - Chambers.Count) /
            (DungeonGenome.Size * DungeonGenome.Size));
        }
        FitnessValues.Add(numChamberFitness);
        FitnessValue += numChamberFitness;
        numFitnesses += 1f;

        float numCorridorFitness = Mathf.Abs(TargetNumberCorridors[0] - Corridors.Count) /
            (DungeonGenome.Size * DungeonGenome.Size);
        for (int i = 0; i < NumberOfTargetGenomes - 1; i++)
        {
            numCorridorFitness = Mathf.Min(numCorridorFitness,
                Mathf.Abs(TargetNumberCorridors[i] - Corridors.Count) /
            (DungeonGenome.Size * DungeonGenome.Size));
        }
        FitnessValues.Add(numCorridorFitness);
        FitnessValue += numCorridorFitness;
        numFitnesses += 1f;

        float deadTileFitness = Mathf.Abs(DeadTileDensity - TargetDeadTileDensity[0]);
        for (int i = 0; i < NumberOfTargetGenomes - 1; i++)
        {
            deadTileFitness = Mathf.Min(deadTileFitness, Mathf.Abs(DeadTileDensity - TargetDeadTileDensity[i]));
        }
        FitnessValues.Add(deadTileFitness);
        FitnessValue += deadTileFitness;
        numFitnesses += 1f;

        float safeEntranceFitness = Mathf.Abs( EntranceSafetyArea - TargetEntranceSafety[0]);
        for (int i = 0; i < NumberOfTargetGenomes - 1; i++)
        {
            safeEntranceFitness = Mathf.Min(safeEntranceFitness, Mathf.Abs(EntranceSafetyArea - TargetEntranceSafety[i]));
        }
        FitnessValues.Add(safeEntranceFitness);
        FitnessValue += safeEntranceFitness;
        numFitnesses += 1f;

        float greedEntranceFitness = Mathf.Abs(EntranceGreedArea - TargetEntranceGreed[0]);
        for (int i = 0; i < NumberOfTargetGenomes - 1; i++)
        {
            greedEntranceFitness = Mathf.Min(greedEntranceFitness, Mathf.Abs(EntranceGreedArea - TargetEntranceGreed[i]));
        }
        FitnessValues.Add(greedEntranceFitness);
        FitnessValue += greedEntranceFitness;
        numFitnesses += 1f;

        float enemyFitness = Mathf.Abs(EnemyDensity - TargetEnemyDensity[0]);
        for (int i = 0; i < NumberOfTargetGenomes - 1; i++)
        {
            enemyFitness = Mathf.Min(enemyFitness, Mathf.Abs(EnemyDensity - TargetEnemyDensity[i]));
        }
        FitnessValues.Add(enemyFitness);
        FitnessValue += enemyFitness;
        numFitnesses += 1f;

        float treasureFitness = Mathf.Abs(TreasureDensity - TargetTreasureDensity[0]);
        for (int i = 0; i < NumberOfTargetGenomes - 1; i++)
        {
            treasureFitness = Mathf.Min(treasureFitness, Mathf.Abs(TreasureDensity - TargetTreasureDensity[i]));
        }
        FitnessValues.Add(treasureFitness);
        FitnessValue += treasureFitness;
        numFitnesses += 1f;

        float treasureSafetyFitness = Mathf.Abs(MeanTreasureSafety - TargetTreasureSafety[0]);
        for (int i = 0; i < NumberOfTargetGenomes - 1; i++)
        {
            treasureSafetyFitness = Mathf.Min(treasureSafetyFitness, Mathf.Abs(MeanTreasureSafety - TargetTreasureSafety[i]));
        }
        FitnessValues.Add(treasureSafetyFitness);
        FitnessValue += treasureSafetyFitness;
        numFitnesses += 1f;

        float treasureSafetyVarFitness = Mathf.Abs(TreasureSafetyVariance - TargetTreasureSafetyVariance[0]);
        for (int i = 0; i < NumberOfTargetGenomes - 1; i++)
        {
            treasureSafetyVarFitness = Mathf.Min(treasureSafetyVarFitness,
                Mathf.Abs(TreasureSafetyVariance - TargetTreasureSafetyVariance[i]));
        }
        FitnessValues.Add(treasureSafetyVarFitness);
        FitnessValue += treasureSafetyVarFitness;
        numFitnesses += 1f;

        //Visual fitnesses
        float numberOfPassableFitness = Mathf.Abs(PassableToImpassableRatio - TargetPassableToImpassableRatio[0]);
        for (int i = 0; i < NumberOfTargetGenomes - 1; i++)
        {
            numberOfPassableFitness = Mathf.Min(numberOfPassableFitness,
                Mathf.Abs(PassableToImpassableRatio - TargetPassableToImpassableRatio[i]));
        }
        FitnessValues.Add(numberOfPassableFitness);
        FitnessValue += numberOfPassableFitness;
        numFitnesses += 1f;

        float pathFitness = Mathf.Abs(TargetPathLength[0] - PathLength);
        for (int i = 0; i < NumberOfTargetGenomes - 1; i++)
        {
            pathFitness = Mathf.Min(pathFitness, Mathf.Abs(TargetPathLength[i] - PathLength));
        }
        FitnessValues.Add(pathFitness);
        FitnessValue += pathFitness;
        numFitnesses += 1f;

        

        float leftRightWallFitness = Mathf.Abs(TargetLeftRightWallRatio[0] - LeftRightWallRatio);
        for (int i = 0; i < NumberOfTargetGenomes - 1; i++)
        {
            leftRightWallFitness = Mathf.Min(leftRightWallFitness,
                Mathf.Abs(TargetLeftRightWallRatio[i] - LeftRightWallRatio));
        }
        FitnessValues.Add(leftRightWallFitness);
        FitnessValue += leftRightWallFitness;
        numFitnesses += 1f;

        float upDownWallFitness = Mathf.Abs(TargetUpDownWallRatio[0] - UpDownWallRatio);
        for (int i = 0; i < NumberOfTargetGenomes - 1; i++)
        {
            upDownWallFitness = Mathf.Min(upDownWallFitness,
                Mathf.Abs(TargetUpDownWallRatio[i] - UpDownWallRatio));
        }
        FitnessValues.Add(upDownWallFitness);
        FitnessValue += upDownWallFitness;
        numFitnesses += 1f;

        float leftRightEnemyFitness = Mathf.Abs(TargetLeftRightEnemyRatio[0] - LeftRightEnemyRatio);
        for (int i = 0; i < NumberOfTargetGenomes - 1; i++)
        {
            leftRightEnemyFitness = Mathf.Min(leftRightEnemyFitness,
                 Mathf.Abs(TargetLeftRightEnemyRatio[i] - LeftRightEnemyRatio));
        }
        FitnessValues.Add(leftRightEnemyFitness);
        FitnessValue += leftRightEnemyFitness;
        numFitnesses += 1f;

        float upDownEnemyFitness = Mathf.Abs(TargetUpDownEnemyRatio[0] - UpDownEnemyRatio);
        for (int i = 0; i < NumberOfTargetGenomes - 1; i++)
        {
            upDownEnemyFitness = Mathf.Min(upDownEnemyFitness,
                Mathf.Abs(TargetUpDownEnemyRatio[i] - UpDownEnemyRatio));
        }
        FitnessValues.Add(upDownEnemyFitness);
        FitnessValue += upDownEnemyFitness;
        numFitnesses += 1f;

        float leftRightTreasureFitness = Mathf.Abs(TargetLeftRightTreasureRatio[0] - LeftRightTreasureRatio);
        for (int i = 0; i < NumberOfTargetGenomes - 1; i++)
        {
            leftRightTreasureFitness = Mathf.Min(leftRightTreasureFitness,
                Mathf.Abs(TargetLeftRightTreasureRatio[i] - LeftRightTreasureRatio));
        }
        FitnessValues.Add(leftRightTreasureFitness);
        FitnessValue += leftRightTreasureFitness;
        numFitnesses += 1f;

        float upDownTreasureFitness = Mathf.Abs(TargetUpDownTreasureRatio[0] - UpDownTreasureRatio);
        for (int i = 0; i < NumberOfTargetGenomes - 1; i++)
        {
            upDownTreasureFitness = Mathf.Min(upDownTreasureFitness,
                Mathf.Abs(TargetUpDownTreasureRatio[i] - UpDownTreasureRatio));
        }
        FitnessValues.Add(upDownTreasureFitness);
        FitnessValue += upDownTreasureFitness;
        numFitnesses += 1f;

        float leftRightTreasureToEnemyFitness = 
            Mathf.Abs(TargetLeftRightTreasureToEnemyRatio[0] - LeftRightTreasureToEnemyRatio);
        for (int i = 0; i < NumberOfTargetGenomes - 1; i++)
        {
            leftRightTreasureToEnemyFitness = Mathf.Min(leftRightTreasureToEnemyFitness,
                Mathf.Abs(TargetLeftRightTreasureToEnemyRatio[i] - LeftRightTreasureToEnemyRatio));
        }
        FitnessValues.Add(leftRightTreasureToEnemyFitness);
        FitnessValue += leftRightTreasureToEnemyFitness;
        numFitnesses += 1f;

        float upDownTreasureToEnemyFitness = Mathf.Abs(TargetUpDownTreasureToEnemyRatio[0] - UpDownTreasureToEnemyRatio);
        for (int i = 0; i < NumberOfTargetGenomes - 1; i++)
        {
            upDownTreasureToEnemyFitness = Mathf.Min(upDownTreasureToEnemyFitness,
                Mathf.Abs(TargetUpDownTreasureToEnemyRatio[i] - UpDownTreasureToEnemyRatio));
        }
        FitnessValues.Add(upDownTreasureToEnemyFitness);
        FitnessValue += upDownTreasureToEnemyFitness;
        numFitnesses += 1f;

        float rotSymFitness = Mathf.Abs(TargetRotationalSymmetry[0] - RotationalSymmetry);
        for (int i = 0; i < NumberOfTargetGenomes - 1; i++)
        {
            rotSymFitness = Mathf.Min(rotSymFitness, Mathf.Abs(TargetRotationalSymmetry[i] - RotationalSymmetry));
        }
        FitnessValues.Add(rotSymFitness);
        FitnessValue += rotSymFitness;
        numFitnesses += 1f;

        float ISymFitness = Mathf.Abs(TargetISymmetry[0] - ISymmetry);
        for (int i = 0; i < NumberOfTargetGenomes - 1; i++)
        {
            ISymFitness = Mathf.Min(ISymFitness, Mathf.Abs(TargetISymmetry[i] - ISymmetry));
        }
        FitnessValues.Add(ISymFitness);
        FitnessValue += ISymFitness;
        numFitnesses += 1f;

        float JSymFitness = Mathf.Abs(TargetJSymmetry[0] - JSymmetry);
        for (int i = 0; i < NumberOfTargetGenomes - 1; i++)
        {
            JSymFitness = Mathf.Min(JSymFitness, Mathf.Abs(TargetJSymmetry[i] - JSymmetry));
        }
        FitnessValues.Add(JSymFitness);
        FitnessValue += JSymFitness;
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

        if ((NumberTreasureTiles + NumberEnemyTiles) > 0)
        {
            UpDownTreasureToEnemyRatio = Mathf.Abs((NumberTreasureTiles_Top - NumberEnemyTiles_Bottom) / (float)(NumberTreasureTiles + NumberEnemyTiles));
            LeftRightTreasureToEnemyRatio = Mathf.Abs((NumberTreasureTiles_Left - NumberEnemyTiles_Right) / (float)(NumberTreasureTiles + NumberEnemyTiles));
        }


    }

    private void CalculateEnemyCoverage()
    {
        EnemyDensity = NumberEnemyTiles / (float)(DungeonGenome.Size * DungeonGenome.Size);
    }

    private void CalculateTreasureCoverage()
    {
        TreasureDensity = NumberTreasureTiles / (float)(DungeonGenome.Size * DungeonGenome.Size);
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

        EntranceSafetyArea /= (DungeonGenome.Size * DungeonGenome.Size);
        EntranceGreedArea /= (DungeonGenome.Size * DungeonGenome.Size);


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


                    //First march from start in i
                    int chamberWidth = 0;
                    bool endWidth = false;
                    while (!endWidth)
                    {
                        int testSize = chamberWidth + 1;
                        bool valid = true;
                        for (int ii = 0; ii <= testSize; ii++)
                        {
                            int testi = i + ii;
                            int testj = j;
                            if ((testi >= DungeonGenome.Size) || (testj >= DungeonGenome.Size) || (CorridorFlag[testi, testj] > 0)
                                    || (ChamberFlag[testi, testj] > 0) || (!passable[testi, testj])) valid = false;
                        }

                        if (valid)
                        {
                            chamberWidth++;
                        }
                        else
                        {
                            endWidth = true;
                        }
                    }
             

                    if (chamberWidth > 0)
                    {
                        endCorner.x = i + chamberWidth;

                        //March in j
                        int chamberHeight = 0;
                        bool endHeight = false;
                        while (!endHeight)
                        {
                            int testSize = chamberHeight + 1;
                            bool valid = true;
                            for (int ii = startCorner.x; ii <= endCorner.x; ii++)
                            {
                                for (int jj = 0; jj <= testSize; jj++)
                                {
                                    int testi = ii;
                                    int testj = j + jj;
                                    if ((testi >= DungeonGenome.Size) || (testj >= DungeonGenome.Size) || (CorridorFlag[testi, testj] > 0)
                                        || (ChamberFlag[testi, testj] > 0) || (!passable[testi, testj])) valid = false;
                                }
                            }

                            if (valid)
                            {
                                chamberHeight++;
                            }
                            else
                            {
                                endHeight = true;
                            }
                        }

                        if (chamberHeight > 0)
                        {
                            //Enter this Chamber
                            endCorner.Set(i + chamberWidth, j + chamberHeight);
                            Chamber chamber = new Chamber(startCorner, endCorner);
                            Chambers.Add(chamber);
                            for (int ii = 0; ii <= chamberWidth; ii++)
                            {
                                for (int jj = 0; jj <= chamberHeight; jj++)
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

                        if (corridor.Length > 0)
                        {
                            Corridors.Add(corridor);
                            for (int k = 0; k < corridor.Length; k++)
                            {
                                CorridorFlag[i + k, j] = Corridors.Count;
                            }
                        }
                    }

                    if (AreLeftAndRightImpassable(i, j))
                    {
                        Corridor corridor = new Corridor();
                        corridor.Type = CorridorType.VERTICAL;
                        corridor.Add(new Vector2Int(i, j));
                        int jj = j;
                        bool endCorridor = false;

                        while (!endCorridor)
                        {
                            jj++;
                            if ((jj < DungeonGenome.Size) && passable[i,jj] &&
                                (AreLeftAndRightImpassable(i, jj)))
                            {
                                corridor.Add(new Vector2Int(i, jj));
                            }
                            else
                            {
                                endCorridor = true;
                            }
                        }

                        if (corridor.Length > 0)
                        {
                            Corridors.Add(corridor);
                            for (int k = 0; k < corridor.Length; k++)
                            {
                                CorridorFlag[i, j+k] = Corridors.Count;
                            }
                        }
                    }

                }
            }
        }
    }


    private void FindDeadTiles()
    {
        NumberDeadTiles = 0;

        for (int i = 0; i < DungeonGenome.Size; i++)
        {
            for (int j = 0; j < DungeonGenome.Size; j++)
            {
                if (passable[i,j] && CorridorFlag[i,j] <=0 && ChamberFlag[i,j] <= 0 && ConnectorFlag[i, j] <= 0)
                {
                    NumberDeadTiles++;
                    DeadFlag[i, j] = 1;
                }
                else
                {
                    DeadFlag[i, j] = -1;
                }
            }

        }

        DeadTileDensity = NumberDeadTiles / (float)(DungeonGenome.Size * DungeonGenome.Size);

    }

    private void CalculateConnectorFitness()
    {
        NumJoints = 0f;
        NumTurns = 0f;

        foreach (Connector c in Connectors)
        {
            if (c.Type == ConnectorType.JOINT) NumJoints += 1.0f;
            if (c.Type == ConnectorType.TURN) NumTurns += 1.0f;
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
        MinChamberSize = DungeonGenome.Size * DungeonGenome.Size;
        MaxChamberSize = 0f;
        MeanChamberSquareness = 0f;
        MeanChamberArea = 0f;
        MaxChamberSquareness = 0f;
        MinChamberSquareness = DungeonGenome.Size * DungeonGenome.Size;
        foreach (Chamber chamber in Chambers)
        {
            MeanChamberArea += chamber.Area;
            MinChamberSize = Mathf.Min(MinChamberSize, chamber.Area);
            MaxChamberSize = Mathf.Max(MaxChamberSize, chamber.Area);
            MeanChamberSquareness += 0.5f*chamber.Squareness;
            MaxChamberSquareness = Mathf.Max(MaxChamberSquareness, 0.5f * chamber.Squareness);
            MinChamberSquareness = Mathf.Min(MinChamberSquareness, 0.5f * chamber.Squareness);
        }

        if (Chambers.Count > 0)
        {
            MeanChamberArea /= Chambers.Count;
            MeanChamberSquareness /= Chambers.Count;
        }
        else
        {
            MinChamberSize = 0f;
            MaxChamberSize = 0f;
            MaxChamberSquareness = 0f;
            MinChamberSquareness = 0f;
        }

       
    }

    private void CalculateCorridorQualities()
    {
        MeanCorridorLength = 0f;
        MaxCorridorLength = 0f;
        MinCorridorLength = (DungeonGenome.Size * DungeonGenome.Size);
        foreach (Corridor corridor in Corridors)
        {
            
            MeanCorridorLength += corridor.Length;
            MaxCorridorLength = Mathf.Max(MaxCorridorLength, corridor.Length);
            MinCorridorLength = Mathf.Min(MinCorridorLength, corridor.Length);
        }
        if (Corridors.Count > 0)
        {
            MeanCorridorLength /= Corridors.Count;
        }
        else
        {
            MaxCorridorLength = 0f;
            MinCorridorLength = 0f;
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

    private void CalculateSymmetryMeasures()
    {
        int numRotationalSymmetry = 0;
        int numISymmetry = 0;
        int numJSymmetry = 0;

        for (int i = 0; i < DungeonGenome.Size; i++)
        {
            for (int j = 0; j < DungeonGenome.Size; j++)
            {
                if (Genome.DungeonMap[i, j] == Genome.DungeonMap[j, i]) numRotationalSymmetry++;
                if (Genome.DungeonMap[i, j] == Genome.DungeonMap[(DungeonGenome.Size - i) - 1, j]) numISymmetry++;
                if (Genome.DungeonMap[i, j] == Genome.DungeonMap[i, (DungeonGenome.Size - j) - 1]) numJSymmetry++;
            }
        }

        RotationalSymmetry = numRotationalSymmetry / (float)(DungeonGenome.Size * DungeonGenome.Size);
        ISymmetry = numISymmetry / (float)(DungeonGenome.Size * DungeonGenome.Size);
        JSymmetry = numJSymmetry / (float)(DungeonGenome.Size * DungeonGenome.Size);

    }
    
}
