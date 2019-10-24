using UnityEngine;

public class Fitness 
{

    public DungeonGenome Genome;

    public int NumberPassableTiles = 0;
    public int NumberEnemyTiles = 0;
    public int NumberTreasureTiles = 0;

    public float EnemyCoverage = 0.0f;
    public float TreasureCoverage = 0.0f;

    public void CalculateFitnesses(DungeonGenome genome)
    {
        Genome = genome;
        CalculateNumberOfTiles();
        Debug.Log("Number of Passable Tiles : " + NumberPassableTiles);
        Debug.Log("Number of Treasure Tiles : " + NumberTreasureTiles);
        Debug.Log("Number of Enemy Tiles : " + NumberEnemyTiles);

        CalculateEnemyCoverage();
        CalculateTreasureCoverage();
        Debug.Log("Enemy Coverage : " + EnemyCoverage);
        Debug.Log("Treasure Coverage : " + TreasureCoverage);

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
                if (Genome.DungeonMap[i, j] != DungeonTileType.WALL) NumberPassableTiles++;
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

}
