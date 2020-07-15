using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class JsonSaveLoadIO
{
    private static readonly string baseSavePath;
    private static readonly string firstPlayPath;

    static JsonSaveLoadIO()
    {
        baseSavePath = Application.persistentDataPath;
        firstPlayPath = $"{Application.streamingAssetsPath}/UnitConfigSave.json";
    }

    public static void SaveItems(ItemContainerSaveData itemContainerSaveData, string path)
    {
        JSONReadWrite.WriteToJsonFile(baseSavePath + "/" + path + ".json", itemContainerSaveData);
    }

    public static ItemContainerSaveData LoadItems(string path)
    {
        string filePath = baseSavePath + "/" + path + ".json";
        if (File.Exists(filePath))
        {
            return JSONReadWrite.ReadFromJsonFile<ItemContainerSaveData>(filePath);
        }
        return null;
    }

    public static void SaveUnit(UnitSaveData unitSaveData, string path)
    {
        JSONReadWrite.WriteToJsonFile(baseSavePath + "/" + path + ".json", unitSaveData);
    }

    public static UnitSaveData LoadUnit(string path)
    {
        string filePath = baseSavePath + "/" + path + ".json";
        if (File.Exists(filePath))
        {
            return JSONReadWrite.ReadFromJsonFile<UnitSaveData>(filePath);
        }
        return null;
    }

    public static Dictionary<string, UnitSaveData> LoadAllUnits(string path)
    {
        string filePath = baseSavePath + "/" + path + ".json";
        if (File.Exists(filePath))
        {
            return JSONReadWrite.ReadFromJsonFile<Dictionary<string,UnitSaveData>>(filePath);
        }
        else if (File.Exists(firstPlayPath))
        {
            Debug.Log("First play json used");
            return JSONReadWrite.ReadFromJsonFile<Dictionary<string,UnitSaveData>>(firstPlayPath);
        }
        return null;
    }

    public static void SaveAllUnits(Dictionary<string, UnitSaveData> allUnitSaveData, string path)
    {
        JSONReadWrite.WriteToJsonFile(baseSavePath + "/" + path + ".json", allUnitSaveData);    }
}
