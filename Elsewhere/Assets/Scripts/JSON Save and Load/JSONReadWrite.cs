using Newtonsoft.Json;
using System.Diagnostics;
using System.IO;

public class JSONReadWrite
{
    public static void WriteToJsonFile<T>(string filePath, T objectToWrite)
    {
        /*FileStream stream = new FileStream(filePath, FileMode.Create, FileAccess.Write);*/
        // StreamWriter writer = new StreamWriter(stream);
        // string jsonString = JsonConvert.SerializeObject(objectToWrite);
        string jsonString = JsonConvert.SerializeObject(objectToWrite, Formatting.Indented, new JsonSerializerSettings
        {
            TypeNameHandling = TypeNameHandling.Auto
        });
        System.IO.File.WriteAllText(filePath, jsonString);
    }

    public static T ReadFromJsonFile<T>(string filePath)
    {
        /*FileStream stream = new FileStream(filePath, FileMode.Open, FileAccess.Read);
        StreamReader reader = new StreamReader(stream);*/
        string jsonString = File.ReadAllText(filePath);
        //return (T) JsonConvert.DeserializeObject<T>(jsonString);
        return JsonConvert.DeserializeObject<T>(jsonString, new JsonSerializerSettings
        {
            TypeNameHandling = TypeNameHandling.Auto
        });
    }
}
