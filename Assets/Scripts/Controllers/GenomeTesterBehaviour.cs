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

    private GeneticAlgorithm geneticAlgorithm;

    private void Awake()
    {
        geneticAlgorithm = gameObject.GetComponent<GeneticAlgorithm>();
    }

    public void AdvanceGeneration()
    {
        geneticAlgorithm.NextGeneration();
        DungeonDrawer.DrawGeneration(geneticAlgorithm.LastGeneration, new Vector3(-1f * DungeonGenome.Size, 0f, 0f));
    }


    public void CreateTwoParentGenomesAndDraw()
    {
        parent1.RandomlyInitialise();
        parent1.CalculateFitnesses();
        parent2.RandomlyInitialise();
        parent2.CalculateFitnesses();

        DungeonDrawer.DrawDungeonFromGenome(parent1, new Vector3(-1f*DungeonGenome.Size, 0f, 0f));
        DungeonDrawer.DrawDungeonFromGenome(parent2, new Vector3(0.1f * DungeonGenome.Size, 0f, 0f));

    }

    public void CreateChildAndDraw()
    {
        child = DungeonGenome.CrossOver(parent1, parent2);
        child.CalculateFitnesses();
        DungeonDrawer.DrawDungeonFromGenome(child, new Vector3(1.2f * DungeonGenome.Size, 0f, 0f));
    }

    public void CreateNewGenomeAndDraw()
    {
        genome.RandomlyInitialise();
        genome.CalculateFitnesses();
        DungeonDrawer.DrawDungeonFromGenome(genome, Vector3.zero);
    }

    public void MutateAndDraw()
    {
        genome.Mutate();
        genome.CalculateFitnesses();
        DungeonDrawer.DrawDungeonFromGenome(genome, Vector3.zero);
    }

}
