using UnityEngine;
using System.Collections;
using UnityEngine.Serialization;

public class CaptureButtonController : MonoBehaviour
{
    public GameObject captureButton;

    // Use this for initialization
    void Awake()
    {
        Unit.ToggleCaptureButton += ToggleState;
        captureButton.SetActive(false);
    }

    private void ToggleState(bool show)
    {
        captureButton.SetActive(show);
    }

    private void OnDestroy()
    {
        Unit.ToggleCaptureButton -= ToggleState;
    }
}
