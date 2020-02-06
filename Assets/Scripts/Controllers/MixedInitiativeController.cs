using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MixedInitiativeController : MonoBehaviour
{
    public KeeperBehaviour Keepers;

    [SerializeField]
    private GameObject phase1Objects;

    [SerializeField]
    private DungeonEditor initialEditor;


    private void Start()
    {
        phase1Objects.SetActive(true);
    }

    public void SubmitFirstDungeon()
    {
        Keepers.AddKeeper(initialEditor.Genome);
    }
}
