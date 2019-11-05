using System.Collections.Generic;
using UnityEngine;

public class Fitness 
{

    public DungeonGenome Genome;

    public int NumberPassableTiles = 0;
    public int NumberEnemyTiles = 0;
    public int NumberTreasureTiles = 0;
    public int NumberReachableTiles = 0;

    public float EnemyCoverage = 0.0f;
    public float TreasureCoverage = 0.0f;
    public float FractalDimension = 0.0f;
    public float FractalDimensionFitness = 0.0f;

    public float FitnessValue = 0.0f;

    private bool validPath;
    private List<Node> path;
    private Vector2Int target;

    private bool[,] reachable = new bool[DungeonGenome.Size, DungeonGenome.Size];

    private List<Corridor> corridors = new List<Corridor>();
    public int[,] CorridorFlag = new int[DungeonGenome.Size, DungeonGenome.Size];

    public void CalculateFitnesses(DungeonGenome genome)
    {
        Genome = genome;
        CalculateNumberOfTiles();    
        CalculateEnemyCoverage();
        CalculateTreasureCoverage();
        CalculateFractalDimension();
        FindCorridors();

        FitnessValue = 0f;
        for (int i = 0; i < corridors.Count; i++)
        {
            FitnessValue = Mathf.Max(FitnessValue, corridors[i].Length);
        }

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
                reachable[i, j] = false;
                if (Genome.DungeonMap[i, j] != DungeonTileType.WALL)
                {
                    NumberPassableTiles++;
                    target.x = i;
                    target.y = j;
                    validPath = PathFinder.FindPath(Genome.EntranceLocation, target, Genome, out path);
                    if (validPath)
                    {
                        NumberReachableTiles++;
                        reachable[i, j] = true;
                    }
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
        FractalDimension = -1f*(Mathf.Log10(NumberReachableTiles) / Mathf.Log10(1.0f / DungeonGenome.Size));
        FractalDimensionFitness = Mathf.Max(0, 1.0f - Mathf.Abs(1.35f - FractalDimension));
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
                if ((CorridorFlag[i,j] < 0) && (reachable[i,j]))
                {
                    //This isn't already assigned to a corridor and it's reachable
                    //Since the loop is forward facing we only need to look forward in i and j

                    //First check in i direction
                    if (AreAboveAndBelowImpassable(i,j))
                    {
                        Corridor corridor = new Corridor();
                        corridor.Add(new Vector2Int(i, j));
                        CorridorFlag[i, j] = corridors.Count;
                        int ii = i;
                        bool endCorridor = false;

                        while (!endCorridor)
                        {
                            ii++;
                            if ((ii < DungeonGenome.Size) && reachable[ii, j] &&
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

                        corridors.Add(corridor);
                    }

                    if (AreLeftAndRightImpassable(i, j))
                    {
                        Corridor corridor = new Corridor();
                        corridor.Add(new Vector2Int(i, j));
                        CorridorFlag[i, j] = corridors.Count;
                        int jj = j;
                        bool endCorridor = false;

                        while (!endCorridor)
                        {
                            jj++;
                            if ((jj < DungeonGenome.Size) && reachable[i,jj] &&
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

                        corridors.Add(corridor);
                    }

                }
            }
        }
    }

    private bool AreAboveAndBelowImpassable(int i, int j)
    {
        //Debug.Log(i.ToString() + " " + j.ToString());
        return (((j == 0) || (!reachable[i, j - 1]))
                        && ((j == DungeonGenome.Size - 1) || (!reachable[i, j + 1])));
    }

    private bool AreLeftAndRightImpassable(int i, int j)
    {
        //Debug.Log(i.ToString() + " " + j.ToString());
        return (((i == 0) || (!reachable[i-1,j]))
                        && ((i == DungeonGenome.Size - 1) || (!reachable[i+1,j])));
    }


}
