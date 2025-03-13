using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public static class TurnStateEvents
{
    public static event Action<TurnState> OnTurnProgress;
    public static void OnTurnProgressed(TurnState turn)
    {
        OnTurnProgress?.Invoke(turn);
    }

    public static event Action OnGameOver;
    public static void OnGameOvered()
    {
        OnGameOver?.Invoke();
    }
}
