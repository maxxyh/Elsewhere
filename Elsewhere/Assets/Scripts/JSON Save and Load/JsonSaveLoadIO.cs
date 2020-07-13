using System.IO;
using UnityEngine;

public class JsonSaveLoadIO
{
    private static readonly string baseSavePath;

    static JsonSaveLoadIO()
    {
        baseSavePath = Application.persistentDataPath;
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
}
