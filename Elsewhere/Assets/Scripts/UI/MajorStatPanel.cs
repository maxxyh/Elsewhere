using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MajorStatPanel : StatPanel
{
    public Button closeButton;
    
    public void OnCloseButton()
    {
        this.gameObject.SetActive(false);
    }
}
