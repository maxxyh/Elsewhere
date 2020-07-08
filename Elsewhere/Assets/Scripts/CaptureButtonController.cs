using UnityEngine;
using System.Collections;

public class CaptureButtonController : MonoBehaviour
{
    public GameObject CaptureButton;

    // Use this for initialization
    void Awake()
    {
        Unit.ToggleCaptureButton += ToggleState;
        CaptureButton.SetActive(false);
    }

    private void ToggleState(bool show)
    {
        CaptureButton.SetActive(show);
    }

    private void OnDestroy()
    {
        Unit.ToggleCaptureButton -= ToggleState;
    }
}
