
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

public class GeneticAlgorithmController : MonoBehaviour
{

    public Experiment[] Experiments;

    public DungeonEditor InitialDungeonEditor;
    public DungeonEditor[] TopFeasibleDungeonEditors;
    public DungeonEditor[] TopInfeasibleDungeonEditors;
    public Button StartOptimisationButton;

    public int NumberOfSubGenerations;
    public int NumberOfBenchmarks;

    public List<GALog> BenchmarkLogs = new List<GALog>();

    private GeneticAlgorithm geneticAlgorithm;
    private Generation lastGen;
    private Generation lastGenInfeasible;
    private int numGenerationsUntilStop;
    private int numberSubIterations;
    private int numberBenchmarksRun;
    private int currentExperiment;

    private void Awake()
    {
        geneticAlgorithm = gameObject.GetComponent<GeneticAlgorithm>();
        InitialDungeonEditor.SetToggleActive(false);
        numberSubIterations = 0;
        numberBenchmarksRun = 0;
        currentExperiment = 0;
    }

    private void Start()
    {
        for (int i = 0; i < TopFeasibleDungeonEditors.Length; i++)
        {
            TopFeasibleDungeonEditors[i].gameObject.SetActive(false);
        }

        for (int i = 0; i < TopInfeasibleDungeonEditors.Length; i++)
        {
            TopInfeasibleDungeonEditors[i].gameObject.SetActive(false);
        }
    }

    private void NextExperiment()
    {
        currentExperiment++;
        numberSubIterations = 0;
        numberBenchmarksRun = 0;
        BenchmarkLogs = new List<GALog>();

        if (currentExperiment < Experiments.Length)
        {
            StartOptimisingBenchmark();
        }
        else
        {
            Debug.Log("EXPERIMENTS COMPLETE");
        }
    }

    //Starts optimising after reseting optimiser
    public void StartOptimisingBenchmark()
    {
        Debug.Log("EXPERIMENT: "+ Experiments[currentExperiment].Name + " RUN: "+ numberBenchmarksRun.ToString());
        StartOptimisationButton.gameObject.SetActive(false);

        Random.InitState(System.DateTime.Now.Hour + System.DateTime.Now.Minute + System.DateTime.Now.Second);
        geneticAlgorithm.ResetOptimiser();
        geneticAlgorithm.ExperimentName = Experiments[currentExperiment].Name;
        geneticAlgorithm.MutationRate = Experiments[currentExperiment].MutationRate;
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
            for (int i = 0; i < TopFeasibleDungeonEditors.Length; i++)
            {
                if (TopFeasibleDungeonEditors[i].Liked.isOn)
                {
                    Fitness.UpdateTargetMetricsFromGenome(TopFeasibleDungeonEditors[i].Genome);
                }
            }

            for (int i = 0; i < TopInfeasibleDungeonEditors.Length; i++)
            {
                if (TopInfeasibleDungeonEditors[i].Liked.isOn)
                {
                    Fitness.UpdateTargetMetricsFromGenome(TopInfeasibleDungeonEditors[i].Genome);
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
        lastGenInfeasible = geneticAlgorithm.LastInfeasibleGeneration;

        for (int i = 0; i < TopFeasibleDungeonEditors.Length; i++)
        {
            if (i < lastGen.NumberOfIndividuals)
            {
                TopFeasibleDungeonEditors[i].gameObject.SetActive(true);
                TopFeasibleDungeonEditors[i].SetGenome(lastGen.Individuals[i]);
                TopFeasibleDungeonEditors[i].SetToggleActive(false);
            }
            else
            {
                TopFeasibleDungeonEditors[i].gameObject.SetActive(false);
            }
            
        }

        for (int i = 0; i < TopInfeasibleDungeonEditors.Length; i++)
        {
            if (i < lastGenInfeasible.NumberOfIndividuals)
            {
                TopInfeasibleDungeonEditors[i].gameObject.SetActive(true);
                TopInfeasibleDungeonEditors[i].SetGenome(lastGenInfeasible.Individuals[i]);
                TopInfeasibleDungeonEditors[i].SetToggleActive(false);
            }
            else
            {
                TopInfeasibleDungeonEditors[i].gameObject.SetActive(false);
            }

        }

        numGenerationsUntilStop--;

        if (numGenerationsUntilStop == 0)
        {
            numberSubIterations++;
            StartOptimisationButton.gameObject.SetActive(true);
            for (int i = 0; i < TopFeasibleDungeonEditors.Length; i++)
            {
                if (i < lastGen.NumberOfIndividuals)
                {
                    TopFeasibleDungeonEditors[i].SetToggleActive(true);
                    TopFeasibleDungeonEditors[i].Liked.isOn = false;
                }
                else
                {
                    TopFeasibleDungeonEditors[i].gameObject.SetActive(false);
                    TopFeasibleDungeonEditors[i].Liked.isOn = false;
                }

            }

            for (int i = 0; i < TopInfeasibleDungeonEditors.Length; i++)
            {
                if (i < lastGen.NumberOfIndividuals)
                {
                    TopInfeasibleDungeonEditors[i].SetToggleActive(true);
                    TopInfeasibleDungeonEditors[i].Liked.isOn = false;
                }
                else
                {
                    TopInfeasibleDungeonEditors[i].gameObject.SetActive(false);
                    TopInfeasibleDungeonEditors[i].Liked.isOn = false;
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
            WriteBenchmarkData();
            NextExperiment();
        }

    }

    private void WriteBenchmarkData()
    {
        Debug.Log("Writing benchmark data...");
        WriteBestFitness();
        WriteMeanFitness();
    }

    private void WriteBestFitness()
    {
        List<string[]> rowData = new List<string[]>();

        for (int i = 0; i < BenchmarkLogs.Count; i++)
        {
            string[] rowDataTemp = new string[BenchmarkLogs[i].FeasibleLog.Count];
            for (int j = 0; j < BenchmarkLogs[i].FeasibleLog.Count; j++)
            {
                rowDataTemp[j] = BenchmarkLogs[i].FeasibleLog[j].BestFitness.ToString();
            }

            rowData.Add(rowDataTemp);
        }

        string[][] output = new string[rowData.Count][];

        for (int i = 0; i < output.Length; i++)
        {
            output[i] = rowData[i];
        }

        int length = output.GetLength(0);
        string delimiter = ",";

        StringBuilder sb = new StringBuilder();

        for (int index = 0; index < length; index++)
            sb.AppendLine(string.Join(delimiter, output[index]));

        string filePath = Application.persistentDataPath + "/" + geneticAlgorithm.ExperimentName + "_"
            + System.DateTime.Now.Day.ToString() + System.DateTime.Now.Month.ToString() +
            System.DateTime.Now.Hour.ToString() + System.DateTime.Now.Minute.ToString() +
            "bestFitnessBenchmarks.csv";

        StreamWriter outStream = System.IO.File.CreateText(filePath);
        outStream.WriteLine(sb);
        outStream.Close();
        Debug.Log(filePath);
    }

    private void WriteMeanFitness()
    {
        List<string[]> rowData = new List<string[]>();

        for (int i = 0; i < BenchmarkLogs.Count; i++)
        {
            string[] rowDataTemp = new string[BenchmarkLogs[i].FeasibleLog.Count];
            for (int j = 0; j < BenchmarkLogs[i].FeasibleLog.Count; j++)
            {
                rowDataTemp[j] = BenchmarkLogs[i].FeasibleLog[j].AverageFitness.ToString();
            }

            rowData.Add(rowDataTemp);
        }

        string[][] output = new string[rowData.Count][];

        for (int i = 0; i < output.Length; i++)
        {
            output[i] = rowData[i];
        }

        int length = output.GetLength(0);
        string delimiter = ",";

        StringBuilder sb = new StringBuilder();

        for (int index = 0; index < length; index++)
            sb.AppendLine(string.Join(delimiter, output[index]));

        string filePath = Application.persistentDataPath + "/" + geneticAlgorithm.ExperimentName + "_"
            + System.DateTime.Now.Day.ToString() + System.DateTime.Now.Month.ToString() +
            System.DateTime.Now.Hour.ToString() + System.DateTime.Now.Minute.ToString() +
            "meanFitnessBenchmarks.csv";

        StreamWriter outStream = System.IO.File.CreateText(filePath);
        outStream.WriteLine(sb);
        outStream.Close();
        Debug.Log(filePath);
    }
}
