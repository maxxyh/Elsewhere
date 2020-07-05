using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu]
public class TEST_Item : ScriptableObject
{
    // To support multiple copies of the same scriptable object
    [SerializeField] string id;
    public string ID { get { return id; } }
    public string itemName;
    public Sprite itemIcon;
    public int itemNumUses;

    protected static readonly StringBuilder sb = new StringBuilder();

    [Range(1, 20)]
    public int maxStack = 1;

    private void OnValidate()
    {
        string path = AssetDatabase.GetAssetPath(this);
        id = AssetDatabase.AssetPathToGUID(path);
    }

    public virtual TEST_Item GetCopy()
    {
        return this;
    }

    public virtual void Destroy()
    {

    }

    public virtual string GetItemType()
    {
        return "";
    }

    public virtual string GetDescription()
    {
        return "";
    }
}
