using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeeperBehaviour : MonoBehaviour
{
    public DungeonEditor[] dungeonEditors;
    private int numberKeepers;

    private void Start()
    {
        numberKeepers = 0;
    }

    public void AddKeeper(DungeonGenome genome)
    {
        if (numberKeepers < dungeonEditors.Length)
        {
            numberKeepers++;
            dungeonEditors[numberKeepers - 1].SetGenome(genome);
        }
    }

    public bool KeepersAreFull()
    {
        if (numberKeepers == dungeonEditors.Length)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public DungeonGenome[] GetAllKeepers()
    {
        DungeonGenome[] genomes = new DungeonGenome[numberKeepers];
        for (int i = 0; i < genomes.Length; i++)
        {
            genomes[i] = new DungeonGenome();
            genomes[i].CopyFromOtherGenome(dungeonEditors[i].Genome);
        }
        return genomes;
    }

}
