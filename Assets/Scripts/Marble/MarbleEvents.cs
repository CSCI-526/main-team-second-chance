using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using static UnityEditor.FilePathAttribute;

public static class MarbleEvents
{
    // some stuff 
    public static event Action OnMarbleSpawned;
    public static void OnMarbleSpawn()
    {
        OnMarbleSpawned?.Invoke();
    }
    public static event Action<MarbleTeam, MarbleData, Vector3, float, Vector3> OnMarbleReadyToLaunch;
    public static void MarbleReadyToLaunch(MarbleTeam Team, MarbleData Data, Vector3 Direction, float Force, Vector3 Location)
    {
        OnMarbleReadyToLaunch?.Invoke(Team, Data, Direction, Force, Location);
    }
    public static event Action<MarbleTeam> OnScoreChange;
    public static void OnScoreChanged(MarbleTeam Team)
    {
        OnScoreChange?.Invoke(Team);
    }
    public static event Action<int> OnRoundsWonChanged;
    public static void OnRoundsWonChange(int RoundsWon)
    {
        OnRoundsWonChanged?.Invoke(RoundsWon);
    }
}
