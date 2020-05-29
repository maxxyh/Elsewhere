using UnityEngine;
using System.Collections;

public class Singleton : MonoBehaviour
{
    private static Singleton instance; 

    public static Singleton MyInstance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<Singleton>();
            }
            return instance; 
        }
    }
    
    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
}
