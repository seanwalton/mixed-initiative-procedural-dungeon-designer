using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenomeTesterBehaviour : MonoBehaviour
{
    public DrawDungeonBehaviour DungeonDrawer;

    private DungeonGenome genome = new DungeonGenome();

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
