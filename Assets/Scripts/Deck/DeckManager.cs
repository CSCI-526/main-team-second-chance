using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public enum EnemyDeckType
{
    DEFAULT,
    RANDOM,
    EXPLODER,
    BLACKHOLER,
    GROWER,
    BABYER,
    CHONKER,
    //GHOSTER,
    SHRINKER,
    SQUARBLEER,
    VAMPIREER,
    SPLITTERER
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
        NodeManager Singleton = NodeManager.Instance;
        if (!Singleton)
        {
            Debug.LogError("The Node Manager Singleton is null");
            return null;
        }

        LevelDataSO currentLevel = Singleton.GetLevelData();
        EnemyDeckType DeckType = currentLevel.GetEnemyDeckType();

        List<MarbleData> marbles = new List<MarbleData>();
        switch (DeckType)
        {
            case EnemyDeckType.DEFAULT:
                GenerateMonoTypeDeck(DeckSize, marbles, "Basic");
                break;
            case EnemyDeckType.RANDOM:
                GenerateRandomEnemyDeck(DeckSize, marbles);
                break;
            case EnemyDeckType.EXPLODER:
                GenerateMonoTypeDeck(DeckSize, marbles, "Bomble");
                break;
            case EnemyDeckType.BLACKHOLER:
                GenerateMonoTypeDeck(DeckSize, marbles, "Black Hole");
                break;
            case EnemyDeckType.GROWER:
                GenerateMonoTypeDeck(DeckSize, marbles, "Grower");
                break;
            case EnemyDeckType.BABYER:
                GenerateMonoTypeDeck(DeckSize, marbles, "Baby");
                break;
            case EnemyDeckType.CHONKER:
                GenerateMonoTypeDeck(DeckSize, marbles, "Chonker");
                break;
            case EnemyDeckType.SHRINKER:
                GenerateMonoTypeDeck(DeckSize, marbles, "Shrinker");
                break;
            case EnemyDeckType.SQUARBLEER:
                GenerateMonoTypeDeck(DeckSize, marbles, "Squarble");
                break;
            case EnemyDeckType.VAMPIREER:
                GenerateMonoTypeDeck(DeckSize, marbles, "Vampire");
                break;
            case EnemyDeckType.SPLITTERER:
                GenerateMonoTypeDeck(DeckSize, marbles, "Splitter");
                break;
            default:
                GenerateMonoTypeDeck(DeckSize, marbles, "Basic");
                break;
        }
        return marbles;
    }
    private void GenerateRandomEnemyDeck(int DeckSize, List<MarbleData> marbles)
    {
        int Count = MarbleSpace.MarblePrefabs.Count;
        for (int i = 0; i < DeckSize; ++i)
        {
            marbles.Add(MarbleSpace.MarblePrefabs[Random.Range(0, Count)]);
        }
    }
    private void GenerateMonoTypeDeck(int DeckSize, List<MarbleData> marbles, string marbleName)
    {
        int halfDeckSize = DeckSize / 2;
        int quarterDeckSize = DeckSize / 4;
        // we want at most half the deck to be whatever with a quarter of the deck as the minimum
        int numSpecials = Random.Range(quarterDeckSize, halfDeckSize);

        var marbleData = MarbleSpace.MarblePrefabs.Find((MarbleData data) => data.MarbleName == marbleName);

        for (int i = 0; i < DeckSize; ++i)
        {
            // this prob shouldn't be hard coded and should be set as an enum but like :shrug: who's gonna change the prefab order amirite
            // I changed it now - samhi
            marbles.Add(marbleData);
        }

        int remainder = DeckSize - numSpecials;
        if (remainder == 0)
        {
            return;
        }
        // add defaults if we didn't fill entire deck with whatever type we picked
        for (int i = 0; i < remainder; ++i)
        {
            marbles.Add(MarbleSpace.MarblePrefabs[0]);
        }
    }
    public List<MarbleData> GenerateNewMarbles()
    {
        List<MarbleData> PossibleMarbleData = new List<MarbleData>();
        for (int i = 0; i < 3; ++i)
        {
            bool bHasGeneratedUniqueMarble = false;
            while(!bHasGeneratedUniqueMarble)
            {
                int randomIndex = Random.Range(0, MarbleSpace.MarblePrefabs.Count);
                if(!PossibleMarbleData.Contains(MarbleSpace.MarblePrefabs[randomIndex]))
                {
                    bHasGeneratedUniqueMarble = true;
                    PossibleMarbleData.Add(MarbleSpace.MarblePrefabs[randomIndex]);
                }
            }
        }

        return PossibleMarbleData;
    }

    public MarbleData GetDefaultMarble()
    {
        return MarbleSpace.MarblePrefabs[0];
    }
}
