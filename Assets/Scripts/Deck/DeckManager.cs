using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.FilePathAttribute;

public class DeckManager : MonoBehaviour
{

    [SerializeField]
    private List<GameObject> MarblePrefabs;
    public List<GameObject> GenerateDeck(int DeckSize)
    {
        List<GameObject> marbles = new List<GameObject>();
        for (int i = 0; i < DeckSize; ++i)
        {
            GameObject marble = Instantiate(MarblePrefabs[0], Vector3.zero, Quaternion.identity);
            marble.SetActive(false);
            marbles.Add(marble);
        }
        return marbles;
    }
}
