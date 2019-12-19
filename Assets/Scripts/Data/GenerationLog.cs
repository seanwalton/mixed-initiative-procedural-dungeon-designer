using UnityEngine;
using System;

[Serializable]
public class GenerationLog 
{
    public int GenerationNumber;
    public float BestFitness;
    public float AverageFitness;

    public GenerationLog(Generation generation, int generationNumber)
    {
        GenerationNumber = generationNumber;
        BestFitness = generation.GetBestFitness();
        AverageFitness = generation.GetAverageFitness();
    }
}
