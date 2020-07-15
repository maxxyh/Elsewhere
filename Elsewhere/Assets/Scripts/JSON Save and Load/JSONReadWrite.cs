using Newtonsoft.Json;
using System.Diagnostics;
using System.IO;
using UnityEngine;
using Debug = UnityEngine.Debug;

public class JSONReadWrite
{
    public static void WriteToJsonFile<T>(string filePath, T objectToWrite)
    {
        string jsonString = JsonConvert.SerializeObject(objectToWrite, Formatting.Indented, new JsonSerializerSettings
        {
            TypeNameHandling = TypeNameHandling.Auto
        });
        System.IO.File.WriteAllText(filePath, jsonString);
    }

    public static T ReadFromJsonFile<T>(string filePath)
    {
        string jsonString = File.ReadAllText(filePath);
        return JsonConvert.DeserializeObject<T>(jsonString, new JsonSerializerSettings
        {
            TypeNameHandling = TypeNameHandling.Auto
        });
    }
}
