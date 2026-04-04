using System;

[Serializable]
public class SaveData {
    public string profileName;
    public Difficulty difficulty = Difficulty.Normal;
    public int currentSceneIndex = 1;
    public bool shootUnlocked = false;
    public bool canDash = false;
    public string lastSaveSpotID = "";
}

public enum Difficulty
{
    Easy = 0,
    Normal = 1,
    Hard = 2,
}