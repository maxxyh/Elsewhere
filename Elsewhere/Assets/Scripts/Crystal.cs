using UnityEngine;
using System.Collections;
using System.Text.RegularExpressions;
using System;

public class Crystal : MonoBehaviour
{
    public Team OwnerTeam = Team.BOTH;
    public Team CapturingTeam = Team.BOTH;
    private readonly int CAPTURE_REQUIREMENT = 2;
    private int _captureProgress;
    public Unit CapturingUnit;
    public Unit OwnerUnit;
    public static Action<Crystal> OnPlayerCrystalCollected;
    public static Action ReturnControlToState;

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
            // change crystal sprite 
            OwnerUnit?.ToggleCrystalBoost(false); 
            CapturingUnit.ToggleCrystalBoost(true);
            OwnerUnit = CapturingUnit;
            _captureProgress = 0;
            if (OwnerTeam == Team.PLAYER)
            {
                OnPlayerCrystalCollected?.Invoke(this);
            }
            else
            {
                ReturnControlToState?.Invoke();
            }
        }
        else
        {
            ReturnControlToState?.Invoke();
        }
        Debug.Log($"Attempted capture. Crystal owner Team = {OwnerTeam}");
    }

    // Need to implement cannot walk-off

    private void OnDestroy()
    {
        Unit.OnCaptureCrystal -= Capture;
        OnPlayerCrystalCollected = null;
    }
}
