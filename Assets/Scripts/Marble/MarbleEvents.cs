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
