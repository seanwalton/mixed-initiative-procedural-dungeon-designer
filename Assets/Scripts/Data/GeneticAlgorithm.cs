using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

[System.Serializable]
public class GeneticAlgorithm : MonoBehaviour
{

    public static MutationMethod MutationMethod;
    public static CrossoverMethod CrossoverMethod;

    public string ExperimentName;
    public int PopulationSize;

    //TournamentSize = 2 is binary
    public int TournamentSize;
    public int NumberOfElite;

    public float MutationRate;


    public List<Generation> GenerationsFeasiblePop = new List<Generation>();
    public List<Generation> GenerationsInfeasiblePop = new List<Generation>();

    
    public GALog FeasibleLog = new GALog();



    public void ResetOptimiser(int popSize)
    {
        PopulationSize = popSize;
        GenerationsFeasiblePop = new List<Generation>();
        GenerationsInfeasiblePop = new List<Generation>();
        FeasibleLog = new GALog();
    }

    public void InitializeFirstGeneration(DungeonGenome[] masterGenome)
    {
        Generation gen1F = new Generation();
        Generation gen1IF = new Generation();

        //Add a mutated version of the masterGenome
        DungeonGenome genome = new DungeonGenome();
        for (int i = 0; i < masterGenome.Length; i++)
        {
            genome.CopyFromOtherGenome(masterGenome[i]);
            for (int j = 0; j < DungeonGenome.Size; j++)
            {
                genome.Mutate();
            }

            DungeonGenome genome2 = new DungeonGenome();
            if (gen1F.NumberOfIndividuals > 0)
            {
                genome2 = DungeonGenome.CrossOver(genome, gen1F.GetRandomIndividual());
            }
            else if (gen1IF.NumberOfIndividuals > 0)
            {
                genome2 = DungeonGenome.CrossOver(genome, gen1IF.GetRandomIndividual());
            }
            else
            {
                genome2.CopyFromOtherGenome(genome);
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
        


        while ((gen1F.NumberOfIndividuals + gen1IF.NumberOfIndividuals) < PopulationSize)
        {
            DungeonGenome randomGenome = new DungeonGenome();
            randomGenome.RandomlyInitialise();

            DungeonGenome genome2 = new DungeonGenome();
            if (gen1F.NumberOfIndividuals > 0)
            {
                genome2 = DungeonGenome.CrossOver(randomGenome, gen1F.GetRandomIndividual());
            }
            else
            {
                genome2 = DungeonGenome.CrossOver(randomGenome, gen1IF.GetRandomIndividual());
            }

            genome2.Mutate();

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

    public void InitializeFirstGeneration(DungeonGenome masterGenome)
    {
        Generation gen1F = new Generation();
        Generation gen1IF = new Generation();

        //Add a mutated version of the masterGenome
        DungeonGenome genome = new DungeonGenome();
        genome.CopyFromOtherGenome(masterGenome);
        for (int i = 0; i < DungeonGenome.Size; i++)
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
            DungeonGenome randomGenome = new DungeonGenome();
            randomGenome.RandomlyInitialise();

            DungeonGenome genome2 = new DungeonGenome();
            if (gen1F.NumberOfIndividuals > 0)
            {
                genome2 = DungeonGenome.CrossOver(randomGenome, gen1F.GetRandomIndividual());
            }
            else
            {
                genome2 = DungeonGenome.CrossOver(randomGenome, gen1IF.GetRandomIndividual());
            }
                         
            genome2.Mutate();
            
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

            //First elitism
            for (int i = 0; i < NumberOfElite; i++)
            {
                genF.AddIndividual(LastFeasibleGeneration.Individuals[i]);
            }

            int k = 0;

            while ((genF.NumberOfIndividuals + genIF.NumberOfIndividuals) < PopulationSize)
            {
                DungeonGenome parent1;
                DungeonGenome parent2;

                if (k < LastFeasibleGeneration.NumberOfIndividuals)
                {
                    parent1 = TournamentSelection(LastFeasibleGeneration, TournamentSize);
                    parent2 = TournamentSelection(LastFeasibleGeneration, TournamentSize);
                }
                else
                {
                    parent1 = TournamentSelection(LastInfeasibleGeneration, TournamentSize);
                    parent2 = TournamentSelection(LastInfeasibleGeneration, TournamentSize);
                }
                
                DungeonGenome child1 = DungeonGenome.CrossOver(parent1, parent2);
                DungeonGenome child2 = DungeonGenome.CrossOver(parent1, parent2);

                if (Random.value < MutationRate) child1.Mutate();
                if (Random.value < MutationRate) child2.Mutate();

                DungeonGenome genome;
                if (child1.CompareTo(child2) > 0)
                {
                    genome = child2;
                }
                else
                {
                    genome = child1;
                }

                if (genome.ValidPath)
                {
                    genF.AddIndividual(genome);
                }
                else
                {
                    genIF.AddIndividual(genome);
                }

                k++;
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

    //k = number of possible parents to compete
    private DungeonGenome TournamentSelection(Generation gen, int k)
    {

        k = Mathf.Min(k, gen.NumberOfIndividuals);

        DungeonGenome best = gen.GetRandomIndividual();

        for (int i = 2; i < k; i++)
        {
            DungeonGenome ind = gen.GetRandomIndividual();
            if (best.CompareTo(ind) > 0)
            {
                best = ind;
            }
        }

        return best;
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
