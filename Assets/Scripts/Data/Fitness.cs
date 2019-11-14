﻿using System.Collections.Generic;
using UnityEngine;

public class Fitness 
{
    public static int TargetCorridorLength = 3;
    public static int TargetChamberArea = 9;
    public static float TargetCorridorRatio = 0.5f;
    public static float TargetChamberRatio = 0.5f;

    public static float JointRatio = 0.5f;
    public static float TurnRatio = 0.5f;

    public DungeonGenome Genome;

    public int NumberPassableTiles = 0;
    public int NumberEnemyTiles = 0;
    public int NumberTreasureTiles = 0;

    public float EnemyCoverage = 0.0f;
    public float TreasureCoverage = 0.0f;
    public float FractalDimension = 0.0f;
    public float FractalDimensionFitness = 0.0f;

    public float FitnessValue = 0.0f;

    private bool validPath;
    private List<Node> path;
    private Vector2Int target;

    private bool[,] passable = new bool[DungeonGenome.Size, DungeonGenome.Size];

    private List<Corridor> corridors = new List<Corridor>();
    private float corridorRatio;
    public int[,] CorridorFlag = new int[DungeonGenome.Size, DungeonGenome.Size];

    private List<Chamber> chambers = new List<Chamber>();
    private float chamberRatio;
    public int[,] ChamberFlag = new int[DungeonGenome.Size, DungeonGenome.Size];

    private List<Connector> connectors = new List<Connector>();
    public int[,] ConnectorFlag = new int[DungeonGenome.Size, DungeonGenome.Size];

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


        float chamberFitness = 1.0f - Mathf.Abs((TargetChamberRatio - chamberRatio) /
            Mathf.Max(TargetChamberRatio, 1.0f - TargetChamberRatio));

        float corridorFitness = 1.0f - Mathf.Abs((TargetCorridorRatio - corridorRatio) /
            Mathf.Max(TargetCorridorRatio, 1.0f - TargetCorridorRatio));

        float patternFitness = (0.25f * chamberFitness) + (0.75f * corridorFitness);

        float pathFitness = genome.PathFromEntranceToExit.Count / 
            (float) (DungeonGenome.Size*DungeonGenome.Size);

        FitnessValue = patternFitness + patternFitness;

        //FitnessValue *= FractalDimensionFitness * genome.PathFromEntranceToExit.Count;
       

    }

    private void CalculateNumberOfTiles()
    {
        NumberPassableTiles = 0;
        NumberEnemyTiles = 0;
        NumberTreasureTiles = 0;

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
                if (Genome.DungeonMap[i, j] == DungeonTileType.ENEMY) NumberEnemyTiles++;
                if (Genome.DungeonMap[i, j] == DungeonTileType.TREASURE) NumberTreasureTiles++;
            }
        }
    }

    private void CalculateEnemyCoverage()
    {
        EnemyCoverage = NumberEnemyTiles / (float) NumberPassableTiles;
    }

    private void CalculateTreasureCoverage()
    {
        TreasureCoverage = NumberTreasureTiles / (float) NumberPassableTiles;
    }

    private void CalculateFractalDimension()
    {
        FractalDimension = -1f*(Mathf.Log10(NumberPassableTiles) / Mathf.Log10(1.0f / DungeonGenome.Size));
        FractalDimensionFitness = Mathf.Max(0, 1.0f - Mathf.Abs(1.35f - FractalDimension));
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

                    int sizeOfChamber = 0;

                    bool endChamber = false;

                    while (!endChamber)
                    {
                        int testSize = sizeOfChamber + 1;

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
                            sizeOfChamber++;
                        }
                        else
                        {
                            endCorner.Set(i+sizeOfChamber, j+sizeOfChamber);
                            endChamber = true;
                        }
                    }

                    if (sizeOfChamber > 0)
                    {
                        //Enter this Chamber
                        Chamber chamber = new Chamber(startCorner, endCorner);
                        chambers.Add(chamber);
                        for (int ii = 0; ii <= sizeOfChamber; ii++)
                        {
                            for (int jj = 0; jj <= sizeOfChamber; jj++)
                            {
                                int testi = i + ii;
                                int testj = j + jj;

                                ChamberFlag[testi, testj] = chambers.Count;
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
                        CorridorFlag[i, j] = corridors.Count;
                        int ii = i;
                        bool endCorridor = false;

                        while (!endCorridor)
                        {
                            ii++;
                            if ((ii < DungeonGenome.Size) && passable[ii, j] &&
                                (AreAboveAndBelowImpassable(ii, j)))
                            {
                                corridor.Add(new Vector2Int(ii, j));
                                CorridorFlag[ii, j] = corridors.Count;
                            }
                            else
                            {
                                endCorridor = true;
                            }
                        }

                        if (corridor.Length > 1) corridors.Add(corridor);
                    }

                    if (AreLeftAndRightImpassable(i, j))
                    {
                        Corridor corridor = new Corridor();
                        corridor.Type = CorridorType.VERTICAL;
                        corridor.Add(new Vector2Int(i, j));
                        CorridorFlag[i, j] = corridors.Count;
                        int jj = j;
                        bool endCorridor = false;

                        while (!endCorridor)
                        {
                            jj++;
                            if ((jj < DungeonGenome.Size) && passable[i,jj] &&
                                (AreLeftAndRightImpassable(i, jj)))
                            {
                                corridor.Add(new Vector2Int(i, jj));
                                CorridorFlag[i, jj] = corridors.Count;
                            }
                            else
                            {
                                endCorridor = true;
                            }
                        }

                        if (corridor.Length > 1) corridors.Add(corridor);
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
        for (int c = 0; c < corridors.Count; c++)
        {
            Vector2Int connCentre = new Vector2Int();
            //Look at corridor entrance first
            if (corridors[c].Type == CorridorType.HORIZONTAL)
            {
                connCentre.Set(corridors[c].Entrance.x - 1, corridors[c].Entrance.y);
            }
            else
            {
                connCentre.Set(corridors[c].Entrance.x, corridors[c].Entrance.y - 1);
            }

            CheckForConnectorAt(connCentre);

            //Then look at corridor exit
            //Look at corridor entrance first
            if (corridors[c].Type == CorridorType.HORIZONTAL)
            {
                connCentre.Set(corridors[c].Exit.x + 1, corridors[c].Exit.y);
            }
            else
            {
                connCentre.Set(corridors[c].Exit.x, corridors[c].Exit.y + 1);
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
                connectors.Add(connector);
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
        chamberRatio = 0f;
        foreach (Chamber chamber in chambers)
        {
            float q = Mathf.Min(1.0f, (chamber.Size * chamber.Size) / (float) TargetChamberArea);
            chamberRatio += ((q * chamber.Size * chamber.Size) / (float) NumberPassableTiles);
        }
    }

    private void CalculateCorridorQualities()
    {
        corridorRatio = 0f;
        foreach (Corridor corridor in corridors)
        {
            float q = Mathf.Min(1.0f, corridor.Length / (float) TargetCorridorLength);
            corridorRatio += ((q * corridor.Length) / (float) NumberPassableTiles);
        }

        foreach (Connector connector in connectors)
        {
            switch (connector.Type)
            {
                case ConnectorType.JOINT:
                    corridorRatio += (JointRatio * connector.Area) / (float)NumberPassableTiles;
                    break;
                case ConnectorType.TURN:
                    corridorRatio += (TurnRatio * connector.Area) / (float)NumberPassableTiles;
                    break;
            }

            
        }
    }


}
