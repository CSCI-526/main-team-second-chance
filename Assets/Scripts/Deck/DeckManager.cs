using System.Collections.Generic;
using UnityEngine;

public class DeckManager : MonoBehaviour
{
    // The different possible marble prefabs that we want to give
    [SerializeField]
    private List<MarbleData> MarblePrefabs;
    public List<DeckItem> GenerateDeck(MarbleTeam Team, int DeckSize)
    {
        List<DeckItem> marbles;
        if (Team == MarbleTeam.Player)
        {
            marbles = GeneratePlayerInitialDeck(Team, DeckSize);
        }
        else
        {
            marbles = GenerateEnemyDeck(Team, DeckSize);
        }

        return marbles;
    }

    public List<DeckItem> GeneratePlayerInitialDeck(MarbleTeam Team, int DeckSize)
    {
        List<DeckItem> marbles = new List<DeckItem>();

        int firstHalf = DeckSize / 2;
        int secondHalf = DeckSize - firstHalf;
        // first half is basic
        for (int i = 0; i < firstHalf; ++i)
        {
            DeckItem newItem = new DeckItem();
            newItem.MarbleData = MarblePrefabs[0];
            newItem.bHasBeenUsed = false;
            marbles.Add(newItem);
        }

        // second half is random
        for (int i = 0; i < secondHalf; ++i)
        {
            int randomIndex = Random.Range(1, MarblePrefabs.Count);
            DeckItem newItem = new DeckItem();
            newItem.MarbleData = MarblePrefabs[randomIndex];
            newItem.bHasBeenUsed = false;
            marbles.Add(newItem);
        }


        return marbles;
    }

    public List<DeckItem> GenerateEnemyDeck(MarbleTeam Team, int DeckSize)
    {
        // For now only generate default data
        List<DeckItem> marbles = new List<DeckItem>();
        for (int i = 0; i < DeckSize; ++i)
        {
            DeckItem newItem = new DeckItem();
            newItem.MarbleData = MarblePrefabs[0];
            newItem.bHasBeenUsed = false;
            marbles.Add(newItem);
        }

        return marbles;
    }

    /* TO DO REFACTOR*/
    public List<MarbleData> GenerateNewMarbles()
    {
        List<MarbleData> marbles = new List<MarbleData>();
        for (int i = 0; i < 3; ++i)
        {
            int randomIndex = Random.Range(0, MarblePrefabs.Count);
            MarbleData marble = MarblePrefabs[randomIndex];
        }

        return marbles;
    }

    public MarbleData GetDefaultMarble()
    {
        return MarblePrefabs[0];
    }
}
