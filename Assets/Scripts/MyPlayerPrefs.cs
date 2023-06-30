using System;
using System.IO;
using UnityEngine;

public class MyPlayerPrefs
{
    private static PlayerStats _instance;
    
    private const string JsonFileName = "PlayerStats";
    private const string FileExtension = ".json";
    private static readonly string SaveDirectory = Application.persistentDataPath;

    [System.Serializable]
    public class PlayerStats
    {
        public uint level;

        public PlayerStats()
        {
            
        }

        public void SetStartedValues()
        {
            level = 0;
            Save();
        }

        public void SetNextLevel()
        {
            level++;
            Save();
        }

        public void SetPrevLevel()
        {
            level--;
            Save();
        }

        public uint GetLevel()
        {
            return level;
        }

        public void Reset()
        {
            level = 0;
            Save();
        }
    }

    public static PlayerStats GetInstance()
    {
        if (_instance is null)
        { 
            _instance = Load();
            if (_instance is null)
            {
                _instance = new PlayerStats();
                _instance.SetStartedValues();
            }
            Save();
        }

        return _instance;
    }

    public static void Save()
    {
        var json = JsonUtility.ToJson(_instance);
        File.WriteAllText(SaveDirectory +"/"+ JsonFileName + FileExtension, json);

        // Debug.Log(Green($"Saved {JsonFileName} " + SaveDirectory +"/"+ JsonFileName + FileExtension));

    }
    
    public static PlayerStats Load()
    {
        PlayerStats obj = null;
        try
        {
            var json = File.ReadAllText(SaveDirectory +"/"+ JsonFileName + FileExtension);
            obj = JsonUtility.FromJson<PlayerStats>(json);
            
            Debug.Log(Green($"Loaded {JsonFileName}"));
        }
        catch (Exception e)
        {
            Debug.Log(Green($"Not Loaded " + e.Message));
        }
        
            
        return obj;
    }

    private static string Green(string text) => $"<b><color=green>{text}</color></b>";
}
