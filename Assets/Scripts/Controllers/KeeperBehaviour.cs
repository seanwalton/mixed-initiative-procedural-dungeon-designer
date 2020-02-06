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

}
