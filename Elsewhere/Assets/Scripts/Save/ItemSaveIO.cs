﻿using System;
using System.IO;
using System.Runtime.CompilerServices;
using UnityEngine;

// connects bw FileReadWrite and ItemSaveData
public static class ItemSaveIO 
{
    private static readonly string baseSavePath;
    
    // static constructor -> auto run when a static variable/method called from the class
    static ItemSaveIO()
    {
        
        baseSavePath = Application.persistentDataPath;
    }

    public static void SaveItems(ItemContainerSaveData items, string path)
    {
        FileReadWrite.WriteToBinaryFile(baseSavePath + "/" + path + ".dat", items);
    }

    public static ItemContainerSaveData LoadItems(string path)
    {
        string filePath = baseSavePath + "/" + path + ".dat";
        if (System.IO.File.Exists(filePath))
        {
            return FileReadWrite.ReadFromBinaryFile<ItemContainerSaveData>(filePath);
        }
        return null;
    }

    public static void Delete(string path)
    {
        string filePath = baseSavePath + "/" + path + ".dat";
        try
        {
            File.Delete(filePath);
        }
        catch (Exception ex)
        {
            Debug.LogException(ex);
        }
    }

    public static void SaveUnit(UnitData data, string path)
    {
        FileReadWrite.WriteToBinaryFile(baseSavePath + "/" + path + ".dat", data);
    }

    public static UnitData LoadUnit(string path)
    {
        string filePath = baseSavePath + "/" + path + ".dat";
        if (System.IO.File.Exists(filePath))
        {
            return FileReadWrite.ReadFromBinaryFile<UnitData>(filePath);
        }
        return null;
    }
}
