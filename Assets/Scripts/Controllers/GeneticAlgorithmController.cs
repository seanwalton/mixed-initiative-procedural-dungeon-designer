
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GeneticAlgorithmController : MonoBehaviour
{

    public DungeonEditor InitialDungeonEditor;
    public DungeonEditor[] TopDungeonEditors;
    public Button StartOptimisationButton;

    public int NumberOfSubGenerations;
    public int NumberOfBenchmarks;

    public List<GALog> BenchmarkLogs = new List<GALog>();

    private GeneticAlgorithm geneticAlgorithm;
    private Generation lastGen;
    private int numGenerationsUntilStop;
    private int numberSubIterations;
    private int numberBenchmarksRun;

    private void Awake()
    {
        geneticAlgorithm = gameObject.GetComponent<GeneticAlgorithm>();
        InitialDungeonEditor.SetToggleActive(false);
        numberSubIterations = 0;
        numberBenchmarksRun = 0;
    }

    private void Start()
    {
        for (int i = 0; i < TopDungeonEditors.Length; i++)
        {
            TopDungeonEditors[i].gameObject.SetActive(false);
        }
    }

    //Starts optimising after reseting optimiser
    public void StartOptimisingBenchmark()
    {
        StartOptimisationButton.gameObject.SetActive(false);

        Random.InitState(System.DateTime.Now.Hour + System.DateTime.Now.Minute + System.DateTime.Now.Second);
        geneticAlgorithm.ResetOptimiser();
        Fitness.SetTargetMetricsFromGenome(InitialDungeonEditor.Genome);
        
        numGenerationsUntilStop = NumberOfSubGenerations;
        InvokeRepeating("AdvanceGeneration", 0f, 0.1f);
    }

    public void StartOptimising()
    {
        StartOptimisationButton.gameObject.SetActive(false);
        if (numberSubIterations == 0)
        {
            Fitness.SetTargetMetricsFromGenome(InitialDungeonEditor.Genome);
        }
        else
        {
            for (int i = 0; i < TopDungeonEditors.Length; i++)
            {
                if (TopDungeonEditors[i].Liked.isOn)
                {
                    Fitness.UpdateTargetMetricsFromGenome(TopDungeonEditors[i].Genome);
                }
            }
        }
        
        numGenerationsUntilStop = NumberOfSubGenerations;
        InvokeRepeating("AdvanceGeneration", 0f, 0.1f);
    }

    private void AdvanceGeneration()
    {
        geneticAlgorithm.NextGeneration();


        //Get the last generation and plot the top individuals
        lastGen = geneticAlgorithm.LastFeasibleGeneration;

        for (int i = 0; i < TopDungeonEditors.Length; i++)
        {
            if (i < lastGen.NumberOfIndividuals)
            {
                TopDungeonEditors[i].gameObject.SetActive(true);
                TopDungeonEditors[i].SetGenome(lastGen.Individuals[i]);
                TopDungeonEditors[i].SetToggleActive(false);
            }
            else
            {
                TopDungeonEditors[i].gameObject.SetActive(false);
            }
            
        }

        numGenerationsUntilStop--;

        if (numGenerationsUntilStop == 0)
        {
            numberSubIterations++;
            StartOptimisationButton.gameObject.SetActive(true);
            for (int i = 0; i < TopDungeonEditors.Length; i++)
            {
                if (i < lastGen.NumberOfIndividuals)
                {
                    TopDungeonEditors[i].SetToggleActive(true);
                    TopDungeonEditors[i].Liked.isOn = false;
                }
                else
                {
                    TopDungeonEditors[i].gameObject.SetActive(false);
                    TopDungeonEditors[i].Liked.isOn = false;
                }

            }

            geneticAlgorithm.SaveLogs();

            CancelInvoke("AdvanceGeneration");
            DoNextBenchmark();
        }

    }

    private void DoNextBenchmark()
    {
        
        BenchmarkLogs.Add(geneticAlgorithm.FeasibleLog);

        numberBenchmarksRun++;
        if (numberBenchmarksRun < NumberOfBenchmarks)
        {
            Debug.Log("Benchmark: " + numberBenchmarksRun.ToString());
            StartOptimisingBenchmark();
        }
        else
        {

        }

    }

}
