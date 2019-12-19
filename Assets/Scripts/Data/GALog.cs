
using System.Collections.Generic;

[System.Serializable]
public class GALog
{
    public List<GenerationLog> FeasibleLog = new List<GenerationLog>();

    public void Add(GenerationLog log)
    {
        FeasibleLog.Add(log);
    }
}
