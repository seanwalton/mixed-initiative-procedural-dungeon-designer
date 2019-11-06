using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Generation 
{
    public List<DungeonGenome> Individuals = new List<DungeonGenome>();

    public void AddIndividual(DungeonGenome genome)
    {
        Individuals.Add(genome);
    }



    public float GetAverageFitness()
    {
        float fitness = 0f;

        for (int i = 0; i < Individuals.Count; i++)
        {
            fitness += Individuals[i].MyFitness.FitnessValue;
        }

        return (fitness / Individuals.Count);
    }

    public void MutateRandomIndividual()
    {
        int i = Random.Range(0, Individuals.Count);
        Individuals[i].Mutate();
    }

    public DungeonGenome GetRandomIndividual()
    {
        int i = Random.Range(0, Individuals.Count);
        return Individuals[i];
    }

    public DungeonGenome GetBestIndividual()
    {
        float bestFitness = Individuals[0].MyFitness.FitnessValue;
        int bestIndividual = 0;

        for (int i = 1; i < Individuals.Count; i++)
        {
            if (Individuals[i].MyFitness.FitnessValue > bestFitness)
            {
                bestIndividual = i;
                bestFitness = Individuals[i].MyFitness.FitnessValue;
            }
        }

        return Individuals[bestIndividual];
    }

    public int GetBestIndividualLocation()
    {
        float bestFitness = Individuals[0].MyFitness.FitnessValue;
        int bestIndividual = 0;

        for (int i = 1; i < Individuals.Count; i++)
        {
            if (Individuals[i].MyFitness.FitnessValue > bestFitness)
            {
                bestIndividual = i;
                bestFitness = Individuals[i].MyFitness.FitnessValue;
            }
        }

        return bestIndividual;
    }

    public DungeonGenome GetRandomAboveAverageIndividual()
    {
        float averageFitness = GetAverageFitness();

        
        
        int numTrys = 0;

        while (numTrys < (Individuals.Count*10))
        {
            int i = Random.Range(0, NumberOfIndividuals);

            if (Individuals[i].ValidPath)
            {
                if (Individuals[i].MyFitness.FitnessValue > averageFitness)
                {
                    return Individuals[i];
                }
            }

            numTrys++;
        }

        return null;

    }


    public int NumberOfIndividuals => Individuals.Count;


}
