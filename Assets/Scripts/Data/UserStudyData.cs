using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class UserStudyData 
{
    public int ParticipantID;

    public bool IsRandom;

    public List<int> Likes = new List<int>();
    public List<int> Keeps = new List<int>();

    public List<int> NumberOfLikesOwnDesign = new List<int>();
    public List<int> NumberOfKeepsOwnDesign = new List<int>();

    public List<int> EditDistanceLiked = new List<int>();

    public List<int> EditDistanceKeep = new List<int>();

    public UserStudyData(int participantID, bool isRandom)
    {
        ParticipantID = participantID;
        IsRandom = isRandom;

        Likes.Clear();
        Keeps.Clear();

        NumberOfKeepsOwnDesign.Clear();
        NumberOfKeepsOwnDesign.Clear();

        EditDistanceLiked.Clear();
        EditDistanceKeep.Clear();

    }
}
