using UnityEngine;

public class Fitness 
{

    public DungeonGenome Genome;

    public int NumberPassableTiles = 0;


    public void CalculateFitnesses(DungeonGenome genome)
    {
        Genome = genome;
        CalculateNumberOfPassableTiles();
        Debug.Log("Number of Passable Tiles : " + NumberPassableTiles);
    }

    private void CalculateNumberOfPassableTiles()
    {
        NumberPassableTiles = 0;
        for (int i = 0; i < DungeonGenome.Size; i++)
        {
            for (int j = 0; j < DungeonGenome.Size; j++)
            {
                if (Genome.DungeonMap[i, j] != DungeonTileType.WALL) NumberPassableTiles++;
            }
        }
    }

}
