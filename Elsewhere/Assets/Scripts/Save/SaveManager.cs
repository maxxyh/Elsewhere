using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public class SaveManager : MonoBehaviour
{
    private Unit[] team;

    private void Awake()
    {
        team = FindObjectsOfType<PlayerUnit>();
    }

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        // for testing only
        if (Input.GetKeyDown(KeyCode.S))
        {
            Save();
        }
        if (Input.GetKeyDown(KeyCode.L))
        {
            Load();
        }
    }

    private void Save()
    {
        try
        {
            BinaryFormatter bf = new BinaryFormatter();
            
            FileStream file = File.Open(Application.persistentDataPath + "/" + "SaveTest.dat", FileMode.OpenOrCreate);
            // C:/ Users / manle / AppData / LocalLow / DefaultCompany / Elsewhere

            SaveData data = new SaveData();

            SaveUnit(data);

            bf.Serialize(file, data);

            file.Close();
        }
        catch(System.Exception)
        {
            // Handling errors
        }
    }

    private void SaveUnit(SaveData data)
    {
        // data.MyUnitData = new UnitData(Unit.unitLevel);
    }

    private void Load()
    {
        try
        {
            BinaryFormatter bf = new BinaryFormatter();

            FileStream file = File.Open(Application.persistentDataPath + "/" + "SaveTest.dat", FileMode.Open);
            // C:/ Users / manle / AppData / LocalLow / DefaultCompany / Elsewhere

            SaveData data = (SaveData) bf.Deserialize(file);

            file.Close();

            LoadUnit(data);
        }
        catch (System.Exception)
        {
            // Handling errors
        }
    }

    private void LoadUnit(SaveData data)
    {
        // Unit.unitLevel = data.MyUnitData.unitLevel;
        // Update UI levelText or sth
        // UpdateLvel() {
        //    levelText.text = unitLevel.ToString();
        // }
        // then inside here Unit.UpdateLevel()
    }
}
