using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GeneticAlgorithm : MonoBehaviour
{
    public int PopulationSize;

    public float MutationRate;

    public List<Generation> Generations = new List<Generation>();

    private float numberToMutate;

    private void Awake()
    {
        numberToMutate = MutationRate * PopulationSize;
    }

    private void InitializeFirstGeneration()
    {
        Generation gen1 = new Generation();

        for (int i = 0; i < PopulationSize; i++)
        {
            DungeonGenome genome = new DungeonGenome();
            genome.RandomlyInitialise();
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
            //New generation
            Generation gen = new Generation();


            //Crossover
            while (gen.NumberOfIndividuals < PopulationSize)
            {

                DungeonGenome parent1 = LastGeneration.GetRandomAboveAverageIndividual();
                if (parent1 is null) parent1 = LastGeneration.GetRandomIndividual();

                DungeonGenome parent2 = LastGeneration.GetRandomIndividual();

                DungeonGenome genome = DungeonGenome.CrossOver(parent1, parent2);
                gen.AddIndividual(genome);
            }
            //Mutation
            for (int i = 0; i < numberToMutate; i++)
            {
                gen.MutateRandomIndividual();
            }

            Generations.Add(gen);
        }

        Debug.Log("Generation " + Generations.Count
            + " Average Fitness: " + Generations[Generations.Count - 1].GetAverageFitness());
    }

    public Generation LastGeneration => Generations[Generations.Count - 1];

    public int NumberOfGenerations => Generations.Count;

}
