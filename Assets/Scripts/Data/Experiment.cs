
[System.Serializable]
public class Experiment 
{
    public string Name;

    public MutationMethod MutationMethod;

    public CrossoverMethod CrossoverMethod;

    public float MutationRate = 0.5f;

    public int TournamentSize;
    public int NumberOfElite;

    public int PopulationSize;
    public int NumberOfGenerations;
   
}

public enum MutationMethod
{
    REPLACE,
    SWAP,
    ROTATE,
    RANDOM_METHOD
}

public enum CrossoverMethod
{
    RANDOM, //0
    FIXED_POINT, //1
    EDIT_DISTANCE, //2
    RANDOM_METHOD
}