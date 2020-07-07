using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public class FileReadWrite
{
	public static void WriteToBinaryFile<T>(string filePath, T objectToWrite)
	{
		using (Stream stream = File.Open(filePath, FileMode.Create))
		{
			var binaryFormatter = new BinaryFormatter();
			binaryFormatter.Serialize(stream, objectToWrite);
		}
	}

	public static T ReadFromBinaryFile<T>(string filePath)
	{
		using (Stream stream = File.Open(filePath, FileMode.Open))
		{
			var binaryFormatter = new BinaryFormatter();
			return (T)binaryFormatter.Deserialize(stream);
		}
	}
}
