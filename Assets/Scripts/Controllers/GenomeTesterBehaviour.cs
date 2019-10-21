using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenomeTesterBehaviour : MonoBehaviour
{
    public DrawDungeonBehaviour DungeonDrawer;

    private DungeonGenome genome = new DungeonGenome();

    private DungeonGenome parent1 = new DungeonGenome();
    private DungeonGenome parent2 = new DungeonGenome();

    private DungeonGenome child = new DungeonGenome();

    public void CreateTwoParentGenomesAndDraw()
    {
        parent1.RandomlyInitialise();
        parent2.RandomlyInitialise();

        DungeonDrawer.DrawDungeonFromGenome(parent1, new Vector3(-1f*DungeonGenome.Size, 0f, 0f));
        DungeonDrawer.DrawDungeonFromGenome(parent2, new Vector3(0.1f * DungeonGenome.Size, 0f, 0f));

    }

    public void CreateChildAndDraw()
    {
        child = DungeonGenome.CrossOver(parent1, parent2);

        DungeonDrawer.DrawDungeonFromGenome(child, new Vector3(1.2f * DungeonGenome.Size, 0f, 0f));
    }

    public void CreateNewGenomeAndDraw()
    {
        genome.RandomlyInitialise();
        DungeonDrawer.DrawDungeonFromGenome(genome, Vector3.zero);
    }

    public void MutateAndDraw()
    {
        genome.Mutate();
        DungeonDrawer.DrawDungeonFromGenome(genome, Vector3.zero);
    }

}
