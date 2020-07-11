using Boo.Lang;
using Microsoft.SqlServer.Server;
using System;

[Serializable]
public class SaveData
{
    public TeamData myTeamData { get; set; }
    // or do we ever need List<TeamData> myTeamsData { get; set; } ?
    // need to initialise in SaveData()
    public UNITTTTTTTTTT myUnitData { get; set; }

    public SaveData()
    {

    }
}

[Serializable]
public class UNITTTTTTTTTT
{
    public string unitName;
    public int unitLevel { get; set; }

    public UNITTTTTTTTTT(string name, int level)
    {
        this.unitName = name;
        this.unitLevel = level;
    }
}

[Serializable]
public class TeamData
{
    public string teamName; //? is this needed

    public List<UNITTTTTTTTTT> playerUnits;

    public TeamData(string name)
    {
        this.teamName = name;
        this.playerUnits = new List<UNITTTTTTTTTT>();
    }
}

