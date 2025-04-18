using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public static class MarbleEvents
{
    // some stuff 
    public static event Action OnMarbleSpawned;
    public static void OnMarbleSpawn()
    {
        OnMarbleSpawned?.Invoke();
    }
    public static event Action<MarbleTeam, MarbleData, Vector3, float, Vector3, bool> OnMarbleReadyToLaunch;
    public static void MarbleReadyToLaunch(MarbleTeam Team, MarbleData Type, Vector3 Direction, float Force, Vector3 Location, bool bOverrideWaiting)
    {
        OnMarbleReadyToLaunch?.Invoke(Team, Type, Direction, Force, Location, bOverrideWaiting);
    }
    public static event Action<MarbleTeam> OnScoreChange;
    public static void OnScoreChanged(MarbleTeam Team)
    {
        OnScoreChange?.Invoke(Team);
    }
    public static event Action<int, int> OnRoundsWonChanged;
    public static void OnRoundsWonChange(int RoundNum, int RoundsWon)
    {
        OnRoundsWonChanged?.Invoke(RoundNum, RoundsWon);
    }
}
