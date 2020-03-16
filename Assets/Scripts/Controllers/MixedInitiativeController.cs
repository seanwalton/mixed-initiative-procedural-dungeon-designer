using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class MixedInitiativeController : MonoBehaviour
{
    public KeeperBehaviour Keepers;

    public Experiment GAParameters;

    public DungeonEditor[] TopFeasibleDungeonEditors;
    public DungeonEditor[] BottomDungeonEditors;

    private DungeonGenome[] OriginalFeasibleEditors;
    private DungeonGenome[] OriginalBottomEditors;

    [SerializeField]
    private GameObject phase1Objects;

    [SerializeField]
    private DungeonEditor initialEditor;

    [SerializeField]
    public GameObject phase2Objects;

    [SerializeField]
    public GameObject phase3Objects;

    [SerializeField]
    public GameObject thankYouMessage;


    private GeneticAlgorithm geneticAlgorithm;
    private Generation lastGen;
    private Generation lastGenInfeasible;
    private int numGenerationsUntilStop;
    private bool optimisationRunning = false;

    private UserStudyData studyData;

    private void Awake()
    {
        geneticAlgorithm = gameObject.GetComponent<GeneticAlgorithm>();
        OriginalFeasibleEditors = new DungeonGenome[TopFeasibleDungeonEditors.Length];
        OriginalBottomEditors = new DungeonGenome[BottomDungeonEditors.Length];
    }

    private void Start()
    {
        phase1Objects.SetActive(true);
        phase2Objects.SetActive(false);
        phase3Objects.SetActive(false);
        thankYouMessage.SetActive(false);
        optimisationRunning = false;
        studyData = new UserStudyData(StudyManager.ParticipantID, StudyManager.IsRandom);
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
        geneticAlgorithm.ResetOptimiser(GAParameters.PopulationSize);
        geneticAlgorithm.ExperimentName = StudyManager.ParticipantID.ToString() + "_" + StudyManager.IsRandom.ToString();
        geneticAlgorithm.MutationRate = GAParameters.MutationRate;
        geneticAlgorithm.TournamentSize = GAParameters.TournamentSize;
        geneticAlgorithm.NumberOfElite = GAParameters.NumberOfElite;
        GeneticAlgorithm.MutationMethod = GAParameters.MutationMethod;
        GeneticAlgorithm.CrossoverMethod = GAParameters.CrossoverMethod;
        //geneticAlgorithm.InitializeFirstGeneration(Keepers.GetAllKeepers());

        phase1Objects.SetActive(false);
        phase2Objects.SetActive(true);
        phase3Objects.SetActive(false);
        //InvokeRepeating("AdvanceGeneration", 0f, 0.1f);
        optimisationRunning = true;
        
    }

    private void Update()
    {
        if (optimisationRunning) AdvanceGeneration();
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

        for (int i = 0; i < BottomDungeonEditors.Length; i++)
        {
            int j = lastGen.NumberOfIndividuals - (lastGen.NumberOfIndividuals / (2 * (i + 1)));
            if ((j < lastGen.NumberOfIndividuals) && (j > TopFeasibleDungeonEditors.Length))
            {

                BottomDungeonEditors[i].gameObject.SetActive(true);
                BottomDungeonEditors[i].SetGenome(lastGen.Individuals[j]);
                BottomDungeonEditors[i].SetToggleActive(false);
                BottomDungeonEditors[i].Editable = false;
            }
            else
            {
                BottomDungeonEditors[i].gameObject.SetActive(false);
            }

        }

        numGenerationsUntilStop--;
        if (numGenerationsUntilStop == 0)
        {
            //CancelInvoke("AdvanceGeneration");
            optimisationRunning = false;
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

                OriginalFeasibleEditors[i] = new DungeonGenome();
                OriginalFeasibleEditors[i].CopyFromOtherGenome(lastGen.Individuals[i]);
            }
            else
            {
                TopFeasibleDungeonEditors[i].gameObject.SetActive(false);
            }

        }

        for (int i = 0; i < BottomDungeonEditors.Length; i++)
        {
            int j = lastGen.NumberOfIndividuals - (lastGen.NumberOfIndividuals / (2 * (i + 1)));
            if ((j < lastGen.NumberOfIndividuals) && (j > TopFeasibleDungeonEditors.Length))
            {
                
                BottomDungeonEditors[i].gameObject.SetActive(true);
                BottomDungeonEditors[i].SetGenome(lastGen.Individuals[j]);
                BottomDungeonEditors[i].SetToggleActive(true);
                BottomDungeonEditors[i].Editable = true;

                OriginalBottomEditors[i] = new DungeonGenome();
                OriginalBottomEditors[i].CopyFromOtherGenome(lastGen.Individuals[j]);
            }
            else
            {
                BottomDungeonEditors[i].gameObject.SetActive(false);
            }

        }

        //Make the last editor empty
        DungeonGenome empty = new DungeonGenome();
        empty.SetAllFloor();

        BottomDungeonEditors[BottomDungeonEditors.Length-1].gameObject.SetActive(true);
        BottomDungeonEditors[BottomDungeonEditors.Length - 1].SetGenome(empty);
        BottomDungeonEditors[BottomDungeonEditors.Length - 1].SetToggleActive(true);
        BottomDungeonEditors[BottomDungeonEditors.Length - 1].Editable = true;


    }

    public void FinishPhase3()
    {

        int likes = 0;
        int keeps = 0;

        int ownLikes = 0;
        int ownKeeps = 0;

        for (int i = 0; i < TopFeasibleDungeonEditors.Length; i++)
        {
            if (TopFeasibleDungeonEditors[i].gameObject.activeSelf)
            {

                if (TopFeasibleDungeonEditors[i].Liked.isOn || TopFeasibleDungeonEditors[i].Keep.isOn)
                {
                    Fitness.UpdateTargetMetricsFromGenome(TopFeasibleDungeonEditors[i].Genome);
                    likes++;
                    studyData.EditDistanceLiked.Add(DungeonGenome.EditDistance(TopFeasibleDungeonEditors[i].Genome,
                        OriginalFeasibleEditors[i]));
                }

                if (TopFeasibleDungeonEditors[i].Keep.isOn)
                {
                    Keepers.AddKeeper(TopFeasibleDungeonEditors[i].Genome);
                    keeps++;
                    studyData.EditDistanceKeep.Add(DungeonGenome.EditDistance(TopFeasibleDungeonEditors[i].Genome,
                        OriginalFeasibleEditors[i]));
                }
            }
        }

        for (int i = 0; i < BottomDungeonEditors.Length; i++)
        {
            if (BottomDungeonEditors[i].gameObject.activeSelf)
            {
                if (BottomDungeonEditors[i].Liked.isOn || BottomDungeonEditors[i].Keep.isOn)
                {
                    Fitness.UpdateTargetMetricsFromGenome(BottomDungeonEditors[i].Genome);

                    if (i==BottomDungeonEditors.Length - 1)
                    {
                        ownLikes++;
                    }
                    else
                    {
                        likes++;
                        studyData.EditDistanceLiked.Add(DungeonGenome.EditDistance(BottomDungeonEditors[i].Genome,
                            OriginalBottomEditors[i]));
                    }
                }

                if (BottomDungeonEditors[i].Keep.isOn)
                {
                    Keepers.AddKeeper(BottomDungeonEditors[i].Genome);
                    if (i == BottomDungeonEditors.Length - 1)
                    {
                        ownKeeps++;
                    }
                    else
                    {
                        keeps++;
                        studyData.EditDistanceKeep.Add(DungeonGenome.EditDistance(BottomDungeonEditors[i].Genome,
                            OriginalBottomEditors[i]));
                    }
                }
            }

            
        }


        studyData.Likes.Add(likes);
        studyData.Keeps.Add(keeps);
        studyData.NumberOfKeepsOwnDesign.Add(ownKeeps);
        studyData.NumberOfLikesOwnDesign.Add(ownLikes);

        if (!Keepers.KeepersAreFull())
        {
            StartOptimiser();
        }
        else
        {
            SaveLogs();
            phase3Objects.SetActive(false);
            thankYouMessage.SetActive(true);
        }


    }

    public void SaveLogs()
    {
        string filepath = Application.persistentDataPath + "/" + geneticAlgorithm.ExperimentName + "_"
            + System.DateTime.Now.Day.ToString() + System.DateTime.Now.Month.ToString() +
            System.DateTime.Now.Hour.ToString() + System.DateTime.Now.Minute.ToString() + "/";

        Directory.CreateDirectory(filepath);

        Debug.Log("Saving logs to " + filepath);

        thankYouMessage.GetComponentInChildren<TMPro.TextMeshProUGUI>().text = "Thank you for taking part in this study. Please fill out the questionnaire and " +
            "send the folder\n" + filepath + "\n to s.p.walton@swansea.ac.uk";

        string saveJSON = JsonUtility.ToJson(studyData);
        using (StreamWriter sw = new StreamWriter(filepath + geneticAlgorithm.ExperimentName + ".JSON"))
        {
            sw.WriteLine(saveJSON);
        }

        ScreenCapture.CaptureScreenshot(filepath + geneticAlgorithm.ExperimentName + "_img.png");

    }

}
