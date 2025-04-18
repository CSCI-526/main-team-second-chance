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

    public static event Action<MarbleTeam, List<MarbleData>> OnHandUpdated;
    public static void HandUpdated(MarbleTeam Team, List<MarbleData> dataList)
    {
        OnHandUpdated?.Invoke(Team, dataList);
    }

    public static event Action<MarbleTeam, int> OnMarbleSelectedFromHand;
    public static void MarbleSelectedFromHand(MarbleTeam Team, int CardID)
    {
        OnMarbleSelectedFromHand?.Invoke(Team, CardID);
    }
    public static event Action<List<MarbleData>> OnSelectNewMarbleToAdd;
    public static void SelectNewMarbleToAdd(List<MarbleData> MarblesToAdd)
    {
        OnSelectNewMarbleToAdd?.Invoke(MarblesToAdd);
    }

    public static event Action<MarbleData> OnAddNewMarbleToDeck;
    public static void AddNewMarbleToDeck(MarbleData MarbleObject)
    {
        OnAddNewMarbleToDeck?.Invoke(MarbleObject);
    }

    public static event Action OnPlayerDeckInitialize;
    public static void OnPlayerDeckInitialized()
    {
        OnPlayerDeckInitialize?.Invoke();
    }
}
