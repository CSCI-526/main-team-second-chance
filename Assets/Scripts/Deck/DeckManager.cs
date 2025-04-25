using System.Collections.Generic;
using UnityEngine;

public enum EnemyDeckType
{
    DEFAULT,
    RANDOM,
    EXPLODER,
    BLACKHOLER,
    GROWER
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
                GenerateMonoTypeDeck(DeckSize, marbles, 0);
                break;
            case EnemyDeckType.RANDOM:
                GenerateRandomEnemyDeck(DeckSize, marbles);
                break;
            case EnemyDeckType.EXPLODER:
                GenerateMonoTypeDeck(DeckSize, marbles, 4);
                break;
            case EnemyDeckType.BLACKHOLER:
                GenerateMonoTypeDeck(DeckSize, marbles, 2);
                break;
            case EnemyDeckType.GROWER:
                GenerateMonoTypeDeck(DeckSize, marbles, 8);
                break;
            default:
                GenerateMonoTypeDeck(DeckSize, marbles, 0);
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
    private void GenerateMonoTypeDeck(int DeckSize, List<MarbleData> marbles, int TypeIndex)
    {
        /*int halfDeckSize = DeckSize / 2;
        // we want at least half the deck to be whatever
        int numExploders = Random.Range(halfDeckSize, DeckSize);*/

        for (int i = 0; i < DeckSize; ++i)
        {
            // this prob shouldn't be hard coded and should be set as an enum but like :shrug: who's gonna change the prefab order amirite
            marbles.Add(MarbleSpace.MarblePrefabs[TypeIndex]);
        }

        /*int remainder = DeckSize - numExploders;
        if (remainder == 0)
        {
            return;
        }
        // add defaults if we didn't fill entire deck with whatever type we picked
        for (int i = 0; i < remainder; ++i)
        {
            marbles.Add(MarbleSpace.MarblePrefabs[0]);
        }*/
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
