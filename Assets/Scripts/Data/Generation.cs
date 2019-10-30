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
            fitness += Individuals[i].MyFitness.FractalDimensionFitness;
        }

        return (fitness / Individuals.Count);
    }

}
