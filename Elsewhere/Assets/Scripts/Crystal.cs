using UnityEngine;
using System.Collections;
using System.Text.RegularExpressions;

public class Crystal : MonoBehaviour
{
    public Team OwnerTeam = Team.BOTH;
    public Team CapturingTeam = Team.BOTH;
    private int CAPTURE_REQUIREMENT = 2;
    private int _captureProgress;
    public Unit CapturingUnit;
    public Unit OwnerUnit;

    private void Awake()
    {
        Unit.OnCaptureCrystal += Capture;
    }

    public void Capture(Unit capturingUnit)
    {
        // must be at the same location
        if (!this.transform.position.Equals(capturingUnit.transform.position))
        {
            return;
        }

        // fresh capture
        if (CapturingUnit == null || !CapturingUnit.Equals(capturingUnit))
        {
            _captureProgress = 0;
        } 
        
        CapturingUnit = capturingUnit;
        CapturingTeam = (CapturingUnit is PlayerUnit) ? Team.PLAYER : Team.ENEMY;

        _captureProgress++;

        if (_captureProgress >= CAPTURE_REQUIREMENT)
        {
            OwnerTeam = CapturingTeam;
            // change crystal colour 
            // give stat boost to the capturing unit
            OwnerUnit.ToggleCrystalBoost(false); // Abstract out to ChangeOwner
            CapturingUnit.ToggleCrystalBoost(true);
            OwnerUnit = CapturingUnit;
            _captureProgress = 0;
            // invoke event to trigger cutscenes
        }
        Debug.Log($"Attempted capture. Crystal owner Team = {OwnerTeam}");
    }

    // Need to implement cannot walk-off

    // Need to implement stat bonus


    private void OnDestroy()
    {
        Unit.OnCaptureCrystal -= Capture;
    }
}
