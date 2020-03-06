using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class StudyManager : MonoBehaviour
{
    public static int ParticipantID = 0;
    public static bool IsRandom = false;

    public TMP_InputField IDInput;

    public void SetID()
    {
        if (IDInput.text == "") return;

        ParticipantID = int.Parse(IDInput.text);
        Debug.Log("Participant ID:" + ParticipantID.ToString());

        if (ParticipantID < 0)
        {
            IsRandom = false;
        }
        else
        {
            Random.InitState(1984);

            int randInt = 0;

            for (int i = 0; i < ParticipantID; i++)
            {
                randInt = Random.Range(0, 2);
            }

            if (randInt == 0)
            {
                IsRandom = false;
            }
            else
            {
                IsRandom = true;
            }
        }
       
        Debug.Log("Is Random :" + IsRandom.ToString());

        SceneManager.LoadScene("MainEditor");

    }

}
