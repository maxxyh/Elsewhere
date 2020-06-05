using UnityEngine;
using System.Collections;

public class GameAssets : MonoBehaviour
{
    private static GameAssets instance; 

    public static GameAssets MyInstance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<GameAssets>();
            }
            return instance; 
        }
    }

    public Transform pfDamagePopUp; 

}
