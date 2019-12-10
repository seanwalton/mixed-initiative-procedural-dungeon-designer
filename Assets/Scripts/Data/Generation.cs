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

    public void Sort()
    {
        Individuals.Sort();
        //Debug.Log("***");
        //for (int i = 0; i < 5; i++)
        //{
        //    Debug.Log(i.ToString() + " " + string.Join(":",Individuals[i].MyFitness.FitnessValues));
        //}
        //Debug.Log("***");
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
        //Sort();
        float bestFitness = Individuals[0].MyFitness.FitnessValue;
        int bestIndividual = 0;

        return Individuals[bestIndividual];
    }

    public int GetBestIndividualLocation()
    {
        //Sort();
        return 0;
    }

    public DungeonGenome GetRandomAboveAverageIndividual()
    {
        //Sort();
        int i = Random.Range(0, NumberOfIndividuals/2);
        return Individuals[i];      

    }


    public int NumberOfIndividuals => Individuals.Count;


}
