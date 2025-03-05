using System.Collections.Generic;
using UnityEngine;

public class DeckManager : MonoBehaviour
{

    [SerializeField]
    private List<GameObject> MarblePrefabs;
    public List<GameObject> GenerateDeck(int DeckSize)
    {
        List<GameObject> marbles = new List<GameObject>();
        for (int i = 0; i < DeckSize; ++i)
        {
            GameObject marble = MarblePrefabs[0];
            marbles.Add(marble);
        }
        return marbles;
    }
}
