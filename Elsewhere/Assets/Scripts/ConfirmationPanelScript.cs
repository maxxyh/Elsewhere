using UnityEngine;
using System.Collections;

public class ConfirmationPanelScript : MonoBehaviour
{
    private bool _isConfirmed = false;

    private void Awake()
    {
        gameObject.SetActive(false);
    }

    public bool IsConfirmed()
    {
        return _isConfirmed;
    }

    public void OnYesButton()
    {
        _isConfirmed = true;
        gameObject.SetActive(false);
    }    

    public void OnNoButton()
    {
        _isConfirmed = false;
        gameObject.SetActive(false);
    }
}
