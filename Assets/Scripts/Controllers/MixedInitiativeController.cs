using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MixedInitiativeController : MonoBehaviour
{
    public KeeperBehaviour Keepers;

    public Experiment GAParameters;

    public DungeonEditor[] TopFeasibleDungeonEditors;
    public DungeonEditor[] TopInfeasibleDungeonEditors;

    [SerializeField]
    private GameObject phase1Objects;

    [SerializeField]
    private DungeonEditor initialEditor;

    [SerializeField]
    public GameObject phase2Objects;

    [SerializeField]
    public GameObject phase3Objects;


    private GeneticAlgorithm geneticAlgorithm;
    private Generation lastGen;
    private Generation lastGenInfeasible;
    private int numGenerationsUntilStop;

    private void Awake()
    {
        geneticAlgorithm = gameObject.GetComponent<GeneticAlgorithm>();
    }

    private void Start()
    {
        phase1Objects.SetActive(true);
        phase2Objects.SetActive(false);
        phase3Objects.SetActive(false);
    }

    public void SubmitFirstDungeon()
    {
        //Setup fitness
        Keepers.AddKeeper(initialEditor.Genome);
        Fitness.SetTargetMetricsFromGenome(initialEditor.Genome);
        
        StartOptimiser();
    }

    private void StartOptimiser()
    {

        //Setup GA
        Random.InitState(System.DateTime.Now.DayOfYear +
            System.DateTime.Now.Hour + System.DateTime.Now.Minute + System.DateTime.Now.Second + System.DateTime.Now.Millisecond);
        numGenerationsUntilStop = GAParameters.NumberOfGenerations;
        geneticAlgorithm.ResetOptimiser(GAParameters.NumberOfGenerations);
        geneticAlgorithm.ExperimentName = GAParameters.Name;
        geneticAlgorithm.MutationRate = GAParameters.MutationRate;

        phase1Objects.SetActive(false);
        phase2Objects.SetActive(true);
        phase3Objects.SetActive(false);
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
                TopFeasibleDungeonEditors[i].Editable = false;
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
                TopInfeasibleDungeonEditors[i].Editable = false;
            }
            else
            {
                TopInfeasibleDungeonEditors[i].gameObject.SetActive(false);
            }

        }

        numGenerationsUntilStop--;
        if (numGenerationsUntilStop == 0)
        {
            CancelInvoke("AdvanceGeneration");
            SetupPhase3();
        }
    }

    private void SetupPhase3()
    {
        phase3Objects.SetActive(true);

        //Set the dungeons editable
        for (int i = 0; i < TopFeasibleDungeonEditors.Length; i++)
        {
            if (i < lastGen.NumberOfIndividuals)
            {
                TopFeasibleDungeonEditors[i].gameObject.SetActive(true);
                TopFeasibleDungeonEditors[i].SetGenome(lastGen.Individuals[i]);
                TopFeasibleDungeonEditors[i].SetToggleActive(true);
                TopFeasibleDungeonEditors[i].Editable = true;
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
                TopInfeasibleDungeonEditors[i].SetToggleActive(true);
                TopInfeasibleDungeonEditors[i].Editable = true;
            }
            else
            {
                TopInfeasibleDungeonEditors[i].gameObject.SetActive(false);
            }

        }

    }

    public void FinishPhase3()
    {
        for (int i = 0; i < TopFeasibleDungeonEditors.Length; i++)
        {
            if (TopFeasibleDungeonEditors[i].Liked.isOn || TopFeasibleDungeonEditors[i].Keep.isOn)
            {
                Fitness.UpdateTargetMetricsFromGenome(TopFeasibleDungeonEditors[i].Genome);
            }

            if (TopFeasibleDungeonEditors[i].Keep.isOn)
            {
                Keepers.AddKeeper(TopFeasibleDungeonEditors[i].Genome);
            }
        }

        for (int i = 0; i < TopInfeasibleDungeonEditors.Length; i++)
        {
            if (TopInfeasibleDungeonEditors[i].Liked.isOn || TopInfeasibleDungeonEditors[i].Keep.isOn)
            {
                Fitness.UpdateTargetMetricsFromGenome(TopInfeasibleDungeonEditors[i].Genome);
            }

            if (TopInfeasibleDungeonEditors[i].Keep.isOn)
            {
                Keepers.AddKeeper(TopInfeasibleDungeonEditors[i].Genome);
            }
        }

        if (!Keepers.KeepersAreFull())
        {
            StartOptimiser();
        }

    }

}
