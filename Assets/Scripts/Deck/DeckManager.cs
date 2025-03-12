using System.Collections.Generic;
using UnityEngine;

public class DeckManager : MonoBehaviour
{
    // The different possible marble prefabs that we want to give
    [SerializeField]
    private List<GameObject> MarblePrefabs;
    public List<GameObject> GenerateDeck(int DeckSize)
    {
        List<GameObject> marbles = new List<GameObject>();
        for (int i = 0; i < DeckSize; ++i)
        {
            GameObject marble = MarblePrefabs[0];

            Marble marbleComp = marble.GetComponent<Marble>();
            if (marbleComp)
            {
                marbleComp.SetUniqueID(i);
            }
            marbles.Add(marble);
        }
        return marbles;
    }
}
