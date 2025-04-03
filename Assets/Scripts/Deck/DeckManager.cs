using System.Collections.Generic;
using UnityEngine;

public class DeckManager : MonoBehaviour
{
    // The different possible marble prefabs that we want to give
    [SerializeField]
    private List<MarbleData> MarblePrefabs;
    public List<MarbleData> GenerateDeck(MarbleTeam Team, int DeckSize)
    {
        List<MarbleData> marbles;
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

    public List<MarbleData> GeneratePlayerInitialDeck(MarbleTeam Team, int DeckSize)
    {
        List<MarbleData> marbles = new List<MarbleData>();

        int firstHalf = DeckSize / 2;
        int secondHalf = DeckSize - firstHalf;
        // first half is basic
        for (int i = 0; i < firstHalf; ++i)
        {
            marbles.Add(MarblePrefabs[0]);
        }

        // second half is random
        for (int i = 0; i < secondHalf; ++i)
        {
            int randomIndex = Random.Range(1, MarblePrefabs.Count);
            marbles.Add(MarblePrefabs[randomIndex]);
        }


        return marbles;
    }

    public List<MarbleData> GenerateEnemyDeck(MarbleTeam Team, int DeckSize)
    {
        // For now only generate default data
        List<MarbleData> marbles = new List<MarbleData>();
        for (int i = 0; i < DeckSize; ++i)
        {
            marbles.Add(MarblePrefabs[0]);
        }

        return marbles;
    }

    public List<MarbleData> GenerateNewMarbles()
    {
        List<MarbleData> PossibleMarbleData = new List<MarbleData>();
        for (int i = 0; i < 3; ++i)
        {
            int randomIndex = Random.Range(0, MarblePrefabs.Count);
            PossibleMarbleData.Add(MarblePrefabs[randomIndex]);
        }

        return PossibleMarbleData;
    }

    public MarbleData GetDefaultMarble()
    {
        return MarblePrefabs[0];
    }
}
