using System.Collections.Generic;
using UnityEngine;

public class DeckManager : MonoBehaviour
{
    // The different possible marble prefabs that we want to give
    [SerializeField]
    private List<GameObject> MarblePrefabs;
    public List<GameObject> GenerateDeck(MarbleTeam Team, int DeckSize)
    {
        List<GameObject> marbles;
        if (Team == MarbleTeam.Player)
        {
            marbles = GeneratePlayerInitialDeck(DeckSize);
        }
        else
        {
            marbles = GenerateEnemyDeck(DeckSize);
        }

        return marbles;
    }

    public List<GameObject> GeneratePlayerInitialDeck(int DeckSize)
    {
        List<GameObject> marbles = new List<GameObject>();

        int firstHalf = DeckSize / 2;
        int secondHalf = DeckSize - firstHalf;
        // first half is basic
        for (int i = 0; i < firstHalf; ++i)
        {
            GameObject marble = Instantiate(MarblePrefabs[0]);
            marble.SetActive(false);
            Marble marbleComp = marble.GetComponent<Marble>();
            marbles.Add(marble);
        }

        // second half is random
        for (int i = 0; i < secondHalf; ++i)
        {
            int randomIndex = Random.Range(1, MarblePrefabs.Count);
            GameObject marble = Instantiate(MarblePrefabs[randomIndex]);
            marble.SetActive(false);
            Marble marbleComp = marble.GetComponent<Marble>();
            marbles.Add(marble);
        }


        return marbles;
    }

    public List<GameObject> GenerateEnemyDeck(int DeckSize)
    {
        List<GameObject> marbles = new List<GameObject>();
        for (int i = 0; i < DeckSize; ++i)
        {
            GameObject marble = Instantiate(MarblePrefabs[0]); // Generates only basics initially
            marble.SetActive(false);
            Marble marbleComp = marble.GetComponent<Marble>();
            marbles.Add(marble);
        }

        return marbles;
    }

    public GameObject GetDefaultMarble()
    {
        return MarblePrefabs[0];
    }
}
