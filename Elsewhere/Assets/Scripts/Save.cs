using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

[System.Serializable]
public class Save
{
    public List<Vector3Int> livingPlayerPositions;
    public List<Vector3Int> livingEnemyPositions;

    // player identifiers
    // current stats
    // abilities?
    // Status effects



    // For the real save file
    Dictionary<int, bool> completedLevels;
    // no score for levels yet 
    // Characters in inventory 
    // 



    // put in gameManager
    public void SaveGame()
    {
        // 1
        //Save save = CreateSaveGameObject(); // This function in gameManager creates a save file

        // 2 perhaps can deserialize a scriptableObject into ScriptableObject
        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Create(Application.persistentDataPath + "/gamesave.save"); // use .dat
        //bf.Serialize(file, save); 
        file.Close();

        Debug.Log("Game Saved");
    }

    public void SaveAsJSON()
    {
        //Save save = CreateSaveGameObject();
        //string json = JsonUtility.ToJson(save);

        //Debug.Log("Saving as JSON: " + json);
    }

    public void LoadGame()
    {
        // 1
        if (File.Exists(Application.persistentDataPath + "/gamesave.save"))
        {
            // clear everything 
            /* Your code here */

            // 2
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(Application.persistentDataPath + "/gamesave.save", FileMode.Open);
            Save save = (Save)bf.Deserialize(file);
            file.Close();

            // 3
            // Load data from file 
            /* Your code here*/

            Debug.Log("Game Loaded");
        }
        else
        {
            Debug.Log("No game saved!");
        }
    }
}
