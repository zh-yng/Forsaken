using System.Collections.Generic;
using System.IO;
using UnityEditor.Overlays;
using UnityEngine;

public static class SaveManager {
    private static string loadedSave = "";
    public static void Save(SaveData data){
        loadedSave = data.profileName;
        string savePath = Path.Combine(Application.persistentDataPath, $"{data.profileName}_save.json");
        string json = JsonUtility.ToJson(data, true);
        File.WriteAllText(savePath, json);
        Debug.Log("[SaveManager] File saved to: " + savePath);
        Debug.Log("[SaveManager] Save contents:\n" + json);
    }

    public static SaveData Load(string profileName) {
        loadedSave = profileName;
        string savePath = Path.Combine(Application.persistentDataPath, $"{profileName}_save.json");
        if (!File.Exists(savePath)){
            Debug.Log("[SaveManager] No save file found at: " + savePath + " — loading defaults");
            return CreateNewSave();
            //return new SaveData();
        }
        string json = File.ReadAllText(savePath);
        Debug.Log("[SaveManager] File loaded from: " + savePath);
        Debug.Log("[SaveManager] Load contents:\n" + json);
        return JsonUtility.FromJson<SaveData>(json);
    }
    public static SaveData Load()
    {
        if (loadedSave == "") {
            loadedSave = "TEST";
        }
        return Load(loadedSave);
    }

    public static void DeleteData(string profileName)
    {
        string path = Path.Combine(Application.persistentDataPath, $"{profileName}_save.json");
        if (loadedSave == profileName) loadedSave = "";
        if (!File.Exists(path)){
            Debug.Log("[SaveManager] No save file found at: " + path + " — returning");
            return;
        }
        File.Delete(path);
    }

    public static List<SaveData> LoadAllProfiles()
    {
        List<SaveData> saveProfiles = new List<SaveData>();

        DirectoryInfo d = new DirectoryInfo(Application.persistentDataPath);

        foreach (FileInfo fi in d.GetFiles())
        {
            string json = File.ReadAllText(fi.FullName);
            saveProfiles.Add(JsonUtility.FromJson<SaveData>(json));
        }

        return saveProfiles;
    }

    public static SaveData CreateNewSave()
    {
        Debug.Log("creating a temporary save");
        SaveData saveData = new SaveData();
        saveData.profileName = "TEST";
        saveData.difficulty = Difficulty.Normal;
        return saveData;
    }
}