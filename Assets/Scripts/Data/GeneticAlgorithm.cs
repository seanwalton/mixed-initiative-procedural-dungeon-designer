using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GeneticAlgorithm : MonoBehaviour
{
    public int PopulationSize;

    public List<Generation> Generations = new List<Generation>();


    private void InitializeFirstGeneration()
    {
        Generation gen1 = new Generation();

        for (int i = 0; i < PopulationSize; i++)
        {
            DungeonGenome genome = new DungeonGenome();
            genome.RandomlyInitialise();
            genome.CalculateFitnesses();
            gen1.AddIndividual(genome);
        }

        Generations.Add(gen1);
    }

    public void NextGeneration()
    {
        if (Generations.Count == 0)
        {
            InitializeFirstGeneration();        
        }
        else
        {
            //New gen
        }

        Debug.Log("Generation " + Generations.Count
            + " Average Fitness: " + Generations[Generations.Count - 1].GetAverageFitness());
    }

    public Generation LastGeneration => Generations[Generations.Count - 1];

}
