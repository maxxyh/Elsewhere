using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PanelScript : MonoBehaviour
{
    public GameObject panel;
    private int counter = 1;
    public void showHidePanel()
    {
        counter++;
        if (counter % 2 == 1)
         {
             panel.SetActive(false);
         }
         else 
         {
             panel.SetActive(true);
         }
    }
}
