using System.Text;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

[CreateAssetMenu]
public class Item : ScriptableObject
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

    #if UNITY_EDITOR
    private void OnValidate()
    {
        string path = AssetDatabase.GetAssetPath(this);
        id = AssetDatabase.AssetPathToGUID(path);
    }
    #endif

    public virtual Item GetCopy()
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
