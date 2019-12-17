using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GeneticAlgorithmController : MonoBehaviour
{

    public DungeonEditor InitialDungeonEditor;
    public DungeonEditor[] TopDungeonEditors;


    private GeneticAlgorithm geneticAlgorithm;
    private Generation lastGen;

    private void Awake()
    {
        geneticAlgorithm = gameObject.GetComponent<GeneticAlgorithm>();
    }

    private void Start()
    {
        for (int i = 0; i < TopDungeonEditors.Length; i++)
        {
            TopDungeonEditors[i].gameObject.SetActive(false);
        }
    }

    public void StartOptimising()
    {
        Fitness.SetTargetMetricsFromGenome(InitialDungeonEditor.Genome);
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
            }
            else
            {
                TopDungeonEditors[i].gameObject.SetActive(false);
            }
            
        }

    }

}
