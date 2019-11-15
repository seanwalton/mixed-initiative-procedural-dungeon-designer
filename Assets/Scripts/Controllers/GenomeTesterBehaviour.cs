using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenomeTesterBehaviour : MonoBehaviour
{
    public int NumberOfGenerations;

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

    public void StartOptimising()
    {
        InvokeRepeating("AdvanceGeneration", 0f, 0.1f);
    }

    public void AdvanceGeneration()
    {
        
            geneticAlgorithm.NextGeneration();

            DungeonGenome best = geneticAlgorithm.LastGeneration.GetBestIndividual();

            DungeonDrawer.DrawDungeonFromGenome(best,
                transform.position);
            Debug.Log("Best fitness " + best.MyFitness.FitnessValue.ToString() + 
                " Entrance Safety Area " + best.MyFitness.EntranceSafetyArea.ToString() +
                " Entrance Greed Area " + best.MyFitness.EntranceGreedArea.ToString());

            Vector3 camPos = Camera.main.transform.position;
            camPos.y = 0.5f * DungeonGenome.Size - (1.1f * 1f * DungeonGenome.Size);

            Camera.main.transform.position = camPos;
            //DungeonDrawer.DrawGeneration(geneticAlgorithm.LastGeneration, 
            //    new Vector3(-1f * DungeonGenome.Size, -1.1f * geneticAlgorithm.NumberOfGenerations * DungeonGenome.Size, 0f));
        

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
