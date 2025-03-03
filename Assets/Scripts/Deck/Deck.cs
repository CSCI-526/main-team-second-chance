using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Deck : MonoBehaviour
{
    public int GetDeckSize() { return MarbleDeck.Count; } 
    public GameObject UseMarble(MarbleTeam Team)
    {
        if(MarbleDeck.Count != 0)
        {
            GameObject marble = MarbleDeck[0];
            MarbleDeck.Remove(marble);
            DeckEvents.MarbleUsed(Team, MarbleDeck.Count);
            return marble;
        }
        return null;
    }
    public void InitializeDeck(MarbleTeam Team, int DeckSize)
    {
        MarbleDeck = GameManager.Instance.GetDeckManager().GenerateDeck(DeckSize);
        DeckEvents.DeckGenerated(Team, MarbleDeck.Count);
    }
    private List<GameObject> MarbleDeck;

}
