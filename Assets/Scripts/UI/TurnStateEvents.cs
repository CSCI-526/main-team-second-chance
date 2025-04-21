using System;

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
    public static event Action OnSuddenDeath;
    public static void DoSuddenDeath()
    {
        OnSuddenDeath?.Invoke();
    }
}
