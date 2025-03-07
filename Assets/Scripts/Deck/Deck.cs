using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Deck : MonoBehaviour
{
    public int GetDeckSize() { return MarbleDeck.Count; }
    public GameObject UseMarble(MarbleTeam Team)
    {
        GameObject marble = null;
        if (MarbleIter < MarbleDeck.Count)
        {
            marble = MarbleDeck[MarbleIter];
            MarbleIter++;
        }
        DeckEvents.MarbleUsed(Team, MarbleDeck.Count - MarbleIter);
        return marble;
    }
    public void InitializeDeck(MarbleTeam Team, int DeckSize)
    {
        MarbleDeck = GameManager.Instance.GetDeckManager().GenerateDeck(DeckSize);
        DeckEvents.DeckGenerated(Team, MarbleDeck.Count);
    }
    private List<GameObject> MarbleDeck;
    private int MarbleIter = 0;
}
