using UnityEngine;
using System;
using System.Collections.Generic;

[Serializable]
public class GenerationLog 
{
    public int GenerationNumber;
    public float BestFitness;
    public float AverageFitness;
    public List<float> BestFitnessValues;

    public GenerationLog(Generation generation, int generationNumber)
    {
        GenerationNumber = generationNumber;
        BestFitness = generation.GetBestFitness();
        AverageFitness = generation.GetAverageFitness();
        BestFitnessValues = generation.GetBestIndividual().MyFitness.FitnessValues;
    }
}
