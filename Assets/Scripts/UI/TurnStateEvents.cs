using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public static class TurnStateEvents
{
    // some stuff 
    public static event Action<TurnState> OnTurnProgress;
    public static void OnTurnProgressed(TurnState turn)
    {
        OnTurnProgress?.Invoke(turn);
    }
}
