using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class DeckEvents
{
    public static event Action<MarbleTeam, int> OnDeckGenerated;
    public static void DeckGenerated(MarbleTeam Team, int Count)
    {
        OnDeckGenerated?.Invoke(Team, Count);
    }

    public static event Action<MarbleTeam, int> OnMarbleUsed;
    public static void MarbleUsed(MarbleTeam Team, int Count)
    {
        OnMarbleUsed?.Invoke(Team, Count);
    }

    public static event Action OnHandUpdated;
    public static void HandUpdated()
    {
        OnHandUpdated?.Invoke();
    }

    public static event Action<int> OnCardSelected;
    public static void CardSelected(int CardID)
    {
        OnCardSelected?.Invoke(CardID);
    }
}
