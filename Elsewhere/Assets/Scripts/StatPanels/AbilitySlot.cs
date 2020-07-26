using UnityEngine;
using UnityEngine.UI;

public class AbilitySlot : MonoBehaviour
{
    public Text abilityName;
    public Text abilityDescription;
    public Text manaCost;
    public Image abilitySprite;
    // potential to add sprite in future

    public void Initialize(string name, string description, string cost)
    {
        abilityName.text = name;
        abilityDescription.text = description;
        manaCost.text = cost;
        abilitySprite.sprite = Resources.Load<Sprite>("Sprites/Skills/" + name);
    }
}
