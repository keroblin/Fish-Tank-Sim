using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using SerializeTools;

public static class Saving
{
    public static SaveData currentSave;
    static string path = Application.persistentDataPath + ".fishTank";
    public static List<ISaving> savers = new();

    public static void Load()
    {
        if (!System.IO.File.Exists(path))
        {
            currentSave = new SaveData();
            Debug.Log("No save found, creating new one");
        }
        else
        {
            string file = System.IO.File.ReadAllText(path);
            currentSave = JsonUtility.FromJson<SaveData>(file);
        }

        foreach (var s in savers)
        {
            s.Load();
        }
    }
    public static void Save()
    {
        foreach (var s in savers)
        {
            s.Save();
        }
        string json = JsonUtility.ToJson(currentSave);
        System.IO.File.WriteAllText(path, json);
    }

    public static void ResetSave()
    {
        currentSave = new SaveData();
        Save();
        SceneManager.LoadScene("SampleScene");
        Load();
    }
}