using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class PanelManager : MonoBehaviour
{
    public int panelCounter = 0;
    public List<GameObject> panelList;
    public List<DialogueDisplay> dialogueDisplays = new List<DialogueDisplay>();

    private void Start()
    {
        foreach(GameObject go in panelList)
        {
            DialogueDisplay dp = go.GetComponent<DialogueDisplay>();
            dialogueDisplays.Add(dp);
        }
    }

    public void ActivatePanel()
    {
        panelList[panelCounter].SetActive(true);
    }

    public void DeactivatePanel()
    {
        panelList[panelCounter].SetActive(false);
        panelCounter++;
    }
}
