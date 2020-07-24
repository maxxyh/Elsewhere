using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class JsonSaveLoadIO
{
    private static readonly string baseSavePath;
    private static readonly string firstPlayPathUnit;
    private static readonly string firstPlayPathInventory;

    static JsonSaveLoadIO()
    {
        baseSavePath = Application.persistentDataPath;
        firstPlayPathUnit = $"{Application.streamingAssetsPath}/defaultUnitConfigSaveData.json";
        firstPlayPathInventory = $"{Application.streamingAssetsPath}/defaultCommonInventorySaveData.json";
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
        else if (File.Exists(firstPlayPathInventory))
        {
            return JSONReadWrite.ReadFromJsonFile<ItemContainerSaveData>(firstPlayPathInventory);
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
        else if (File.Exists(firstPlayPathUnit))
        {
            Debug.Log("First play json used");
            return JSONReadWrite.ReadFromJsonFile<Dictionary<string,UnitSaveData>>(firstPlayPathUnit);
        }
        return null;
    }
    
    public static Dictionary<string, UnitSaveData> LoadAllEnemyUnits(string path)
    {
        string filePath = Application.streamingAssetsPath + "/" + path + ".json";
        if (File.Exists(filePath))
        {
            return JSONReadWrite.ReadFromJsonFile<Dictionary<string,UnitSaveData>>(filePath);
        }
        else if (File.Exists(firstPlayPathUnit))
        {
            Debug.Log("First play json used");
            return JSONReadWrite.ReadFromJsonFile<Dictionary<string,UnitSaveData>>(firstPlayPathUnit);
        }
        return null;
    }

    public static void SaveAllUnits(Dictionary<string, UnitSaveData> allUnitSaveData, string path)
    {
        JSONReadWrite.WriteToJsonFile(baseSavePath + "/" + path + ".json", allUnitSaveData);    }
}
