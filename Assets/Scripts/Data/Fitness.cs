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

    public void CalculateFitnesses(DungeonGenome genome)
    {
        Genome = genome;
        CalculateNumberOfTiles();    
        CalculateEnemyCoverage();
        CalculateTreasureCoverage();
        CalculateFractalDimension();

        FitnessValue = FractalDimensionFitness;
       

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
                if (Genome.DungeonMap[i, j] != DungeonTileType.WALL)
                {
                    NumberPassableTiles++;
                    target.x = i;
                    target.y = j;
                    validPath = PathFinder.FindPath(Genome.EntranceLocation, target, Genome, out path);
                    if (validPath) NumberReachableTiles++;
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
    

}
