using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

[System.Serializable]
public class GeneticAlgorithm : MonoBehaviour
{
    public string ExperimentName;
    public int PopulationSize;

    public float MutationRate;
    

    public List<Generation> GenerationsFeasiblePop = new List<Generation>();
    public List<Generation> GenerationsInfeasiblePop = new List<Generation>();

    
    public GALog FeasibleLog = new GALog();

    
    public void ResetOptimiser()
    {
        GenerationsFeasiblePop = new List<Generation>();
        GenerationsInfeasiblePop = new List<Generation>();
        FeasibleLog = new GALog();
    }

    public void InitializeFirstGeneration(DungeonGenome masterGenome)
    {
        Generation gen1F = new Generation();
        Generation gen1IF = new Generation();

        //Add a mutated version of the masterGenome
        DungeonGenome genome = new DungeonGenome();
        genome.CopyFromOtherGenome(masterGenome);
        for (int i = 0; i < Random.Range(1, 2*DungeonGenome.Size); i++)
        {
            genome.Mutate();
        }

        if (genome.ValidPath)
        {
            gen1F.AddIndividual(genome);
        }
        else
        {
            gen1IF.AddIndividual(genome);
        }

        
        for (int i = 0; i < PopulationSize; i++)
        {
            DungeonGenome genome2 = new DungeonGenome();
            genome2.CopyFromOtherGenome(gen1F.GetRandomIndividual());
            for (int j = 0; j < Random.Range(1, 2*DungeonGenome.Size); j++)
            {
                genome2.Mutate();
            }
            if (genome2.ValidPath)
            {
                gen1F.AddIndividual(genome2);
            }
            else
            {
                gen1IF.AddIndividual(genome2);
            }
        }
        gen1F.Sort();
        gen1IF.Sort();
        GenerationsFeasiblePop.Add(gen1F);
        GenerationsInfeasiblePop.Add(gen1IF);

    }   

    private void InitializeFirstGeneration()
    {
        Generation gen1F = new Generation();
        Generation gen1IF = new Generation();

        for (int i = 0; i < PopulationSize; i++)
        {
            DungeonGenome genome = new DungeonGenome();
            genome.RandomlyInitialise();
            if (genome.ValidPath)
            {
                gen1F.AddIndividual(genome);
            }
            else
            {
                gen1IF.AddIndividual(genome);
            }
        }
        gen1F.Sort();
        gen1IF.Sort();
        GenerationsFeasiblePop.Add(gen1F);
        GenerationsInfeasiblePop.Add(gen1IF);
    }

    public void NextGeneration()
    {
        if ((GenerationsFeasiblePop.Count+GenerationsInfeasiblePop.Count) == 0)
        {
            InitializeFirstGeneration();        
        }
        else
        {
            //New generation
            Generation genF = new Generation();
            Generation genIF = new Generation();

            for (int i = 0; i < LastFeasibleGeneration.NumberOfIndividuals; i++)
            {
                DungeonGenome parent1 = LastFeasibleGeneration.GetRandomIndividual();

                DungeonGenome parent2 = LastFeasibleGeneration.Individuals[i];

                DungeonGenome genome = DungeonGenome.CrossOver(parent1, parent2);

                if (Random.value < MutationRate) genome.Mutate();
                
                if ((parent2.CompareTo(genome) > 0) || 
                    (i > (MutationRate * LastFeasibleGeneration.NumberOfIndividuals)))
                {
                    if (genome.ValidPath)
                    {
                        genF.AddIndividual(genome);
                    }
                    else
                    {
                        genIF.AddIndividual(genome);
                    }
                    
                }
                else
                {
                    if (parent2.ValidPath)
                    {
                        genF.AddIndividual(parent2);
                    }
                    else
                    {
                        genIF.AddIndividual(parent2);
                    }
                }
                               
            }

            for (int i = 0; i < LastInfeasibleGeneration.NumberOfIndividuals; i++)
            {
                DungeonGenome parent1 = LastInfeasibleGeneration.GetRandomIndividual();

                DungeonGenome parent2 = LastInfeasibleGeneration.Individuals[i];

                DungeonGenome genome = DungeonGenome.CrossOver(parent1, parent2);

                if (Random.value < MutationRate) genome.Mutate();

                if ((parent2.CompareTo(genome) > 0) ||
                    (i > (MutationRate * LastInfeasibleGeneration.NumberOfIndividuals)))
                {
                    if (genome.ValidPath)
                    {
                        genF.AddIndividual(genome);
                    }
                    else
                    {
                        genIF.AddIndividual(genome);
                    }

                }
                else
                {
                    if (parent2.ValidPath)
                    {
                        genF.AddIndividual(parent2);
                    }
                    else
                    {
                        genIF.AddIndividual(parent2);
                    }
                }

            }

            genF.Sort();
            genIF.Sort();
            GenerationsFeasiblePop.Add(genF);
            GenerationsInfeasiblePop.Add(genIF);
        }

        FeasibleLog.Add(new GenerationLog(LastFeasibleGeneration, GenerationsFeasiblePop.Count));

        Debug.Log("Generation " + GenerationsFeasiblePop.Count
            + " Feasible Average: " + LastFeasibleGeneration.GetAverageFitness()
            + " Feasible Best: " + LastFeasibleGeneration.GetBestFitness());
    }


    public void SaveLogs()
    {
        string filepath = Application.persistentDataPath + "/" + ExperimentName + "_"
            + System.DateTime.Now.Day.ToString() + System.DateTime.Now.Month.ToString() +
            System.DateTime.Now.Hour.ToString() + System.DateTime.Now.Minute.ToString() + "/";

        Directory.CreateDirectory(filepath);

        Debug.Log("Saving logs to " + filepath);
        

        string saveJSON = JsonUtility.ToJson(FeasibleLog);
        using (StreamWriter sw = new StreamWriter(filepath+"logs.JSON"))
        {
            sw.WriteLine(saveJSON);
        }

        ScreenCapture.CaptureScreenshot(filepath+"img.png");

    }

    public Generation LastFeasibleGeneration => GenerationsFeasiblePop[GenerationsFeasiblePop.Count - 1];
    public Generation LastInfeasibleGeneration => GenerationsInfeasiblePop[GenerationsInfeasiblePop.Count - 1];

    public int NumberOfGenerations => GenerationsFeasiblePop.Count;

}
