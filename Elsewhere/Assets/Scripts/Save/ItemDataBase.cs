#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

[CreateAssetMenu]
public class ItemDataBase : ScriptableObject
{
    [SerializeField] Item[] items;

    public Item GetItemReference(string itemID)
    {
        foreach(Item item in items)
        {
            if (item.ID == itemID)
            {
                return item;
            }
        }
        return null;
    }

    public Item GetItemCopy(string itemID)
    {
        Item item = GetItemReference(itemID);
        if (item == null)
        {
            return null;
        }
        else
        {
            return item.GetCopy();
        }
    }

    // only compile when in Unity Editor, if build, not compiled
    #if UNITY_EDITOR
    private void OnValidate()
    {
        LoadItems();
    }

    private void OnEnable()
    {
        EditorApplication.projectChanged -= LoadItems;
        EditorApplication.projectChanged += LoadItems;
    }

    private void OnDisable()
    {
        EditorApplication.projectChanged -= LoadItems;
    }

    private void LoadItems()
    {
        items = FindAssetsByType<Item>("Assets/ScriptableObjects/TEST_Items");
    }

    public static T[] FindAssetsByType<T>(params string[] folders) where T : Object
    {
        string type = typeof(T).Name;

        string[] guids;
        if (folders == null || folders.Length == 0)
        {
            guids = AssetDatabase.FindAssets("t:" + type);
        }
        else
        {
            guids = AssetDatabase.FindAssets("t:" + type, folders);
        }

        T[] assets = new T[guids.Length];

        for (int i = 0; i < guids.Length; i++)
        {
            string assetPath = AssetDatabase.GUIDToAssetPath(guids[i]);
            assets[i] = AssetDatabase.LoadAssetAtPath<T>(assetPath);
        }
        return assets;
    }
    #endif
}
