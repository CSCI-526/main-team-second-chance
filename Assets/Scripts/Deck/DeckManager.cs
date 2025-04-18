using System.Collections.Generic;
using UnityEngine;

public enum EnemyDeckType
{
    DEFAULT,
    RANDOM
}

public class DeckManager : MonoBehaviour
{
    // The different possible marble prefabs that we want to give
    [SerializeField] private MarbleList MarbleSpace;
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
            marbles.Add(MarbleSpace.MarblePrefabs[0]);
        }

        // second half is random
        for (int i = 0; i < secondHalf; ++i)
        {
            int randomIndex = Random.Range(1, MarbleSpace.MarblePrefabs.Count);
            marbles.Add(MarbleSpace.MarblePrefabs[randomIndex]);
        }


        return marbles;
    }

    public List<MarbleData> GenerateEnemyDeck(MarbleTeam Team, int DeckSize)
    {
        // For now only generate default data
        List<MarbleData> marbles = new List<MarbleData>();
        for (int i = 0; i < DeckSize; ++i)
        {
            marbles.Add(MarbleSpace.MarblePrefabs[0]);
        }

        return marbles;
    }

    public List<MarbleData> GenerateNewMarbles()
    {
        List<MarbleData> PossibleMarbleData = new List<MarbleData>();
        for (int i = 0; i < 3; ++i)
        {
            int randomIndex = Random.Range(0, MarbleSpace.MarblePrefabs.Count);
            PossibleMarbleData.Add(MarbleSpace.MarblePrefabs[randomIndex]);
        }

        return PossibleMarbleData;
    }

    public MarbleData GetDefaultMarble()
    {
        return MarbleSpace.MarblePrefabs[0];
    }
}
