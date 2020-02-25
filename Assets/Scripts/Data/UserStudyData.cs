using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class UserStudyData 
{
    public int ParticipantID;

    public bool IsRandom;

    public int Likes;
    public int Keeps;

    public int NumberOfLikesOwnDesign;
    public int NumberOfKeepsOwnDesign;

    public List<int> EditDistanceLiked = new List<int>();

    public List<int> EditDistanceKeep = new List<int>();

    public UserStudyData(int participantID, bool isRandom)
    {
        ParticipantID = participantID;
        IsRandom = isRandom;

        Likes = 0;
        Keeps = 0;

        NumberOfKeepsOwnDesign = 0;
        NumberOfKeepsOwnDesign = 0;

        EditDistanceLiked.Clear();
        EditDistanceKeep.Clear();

    }
}
