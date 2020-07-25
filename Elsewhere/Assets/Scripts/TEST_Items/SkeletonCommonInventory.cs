using System;
using System.Collections.Generic;
using UnityEngine;

public class SkeletonCommonInventory : MonoBehaviour
{
    public List<Item> items = new List<Item>();

    [SerializeField] private UnitSaveManager unitSaveManager;
    
    private static SkeletonCommonInventory instance;
    public static SkeletonCommonInventory Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<SkeletonCommonInventory>();
                if (instance == null)
                {
                    instance = new GameObject("Spawned Convoy", typeof(SkeletonCommonInventory)).GetComponent<SkeletonCommonInventory>();
                }
            }
            return instance;
        }
        private set
        {
            instance = value;
        }
    }
    private void OnValidate()
    {
        unitSaveManager = FindObjectOfType<UnitSaveManager>();
    }
    private void Awake()
    {
        InitializeItems();
    }

    private void OnDestroy()
    {
        unitSaveManager.SaveInventory(items);
    }

    private void InitializeItems()
    {
        Debug.Log($"items list null: {items == null}");
        Debug.Log($"unitSaveManager is null: {unitSaveManager == null}");
        items = unitSaveManager.LoadInventory();
    }

    public void AddItem(Item item)
    {
        items.Add(item);
    }
}