using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Deck : MonoBehaviour
{
    public int GetDeckSize() { return MarbleDeck.Count; }
    public int GetHandSize() { return Hand.Count; }
    public List<GameObject> GetHand() { return Hand; }
    public int GetMaxHandSize() { return MAX_HAND_SIZE; }
    public int GetSelectedMarbleIndex() { return SelectedMarble; }
    public GameObject UseMarble(MarbleTeam Team)
    {
        GameObject marble = null;
        if (SelectedMarble < 0)
        {
            return marble;
        }

        if (SelectedMarble < Hand.Count && SelectedMarble >= 0)
        {
            marble = Hand[SelectedMarble];
            UpdateHand();
        }
        DeckEvents.MarbleUsed(Team, MarbleDeck.Count);
        return marble;
    }
    public void InitializeDeck(MarbleTeam Team, int DeckSize)
    {
        MarbleDeck = GameManager.Instance.GetDeckManager().GenerateDeck(DeckSize);
        DeckEvents.DeckGenerated(Team, MarbleDeck.Count);
        GenerateInitialHand();
        DeckEvents.HandUpdated();
    }

    public void UpdateHand()
    {
        // Add the card to the graveyard
        Graveyard.Add(Hand[SelectedMarble]);
        // Remove the selected card
        Hand.RemoveAt(SelectedMarble);

        if (MarbleDeck.Count <= 0)
        {
            Debug.LogError("Deck.UpdateHand(): MarbleDeck's size is now 0. We can no longer update the hand.");
            // Set the selected marble index to -1 again
            SelectedMarble = -1;
            return;
        }
        // Draw a new card from the Marble Deck
        Hand.Add(MarbleDeck[0]);
        // Remove it from the Deck
        MarbleDeck.RemoveAt(0);

        // Reset the selected marble index
        SelectedMarble = -1;
        // HandUpdated signal
        DeckEvents.HandUpdated();
    }


    private List<GameObject> MarbleDeck = new List<GameObject>();
    private List<GameObject> Hand = new List<GameObject>();
    private List<GameObject> Graveyard = new List<GameObject>();
    // The selected marble from your hand
    private int SelectedMarble = -1;
    [SerializeField]
    private int MAX_HAND_SIZE = 5;
    [SerializeField]
    private int INIT_HAND_SIZE = 3;
    private void GenerateInitialHand()
    {
        if (MarbleDeck.Count == 0)
        {
            Debug.LogError("Deck.GenerateInitialHand() The Marble Deck is currently empty.");
            return;
        }

        for (int i = 0; i < INIT_HAND_SIZE; ++i)
        {
            Hand.Add(MarbleDeck[0]);
            if (MarbleDeck.Count == 0)
            {
                Debug.LogError("Deck.GenerateInitialHand() The Marble Deck is now empty. We cannot draw any more");
                return;
            }
            MarbleDeck.RemoveAt(0);
        }
        DeckEvents.MarbleUsed(GameManager.Instance.GetPlayerManager().GetTeam(), MarbleDeck.Count);
    }
    private void OnEnable()
    {
        DeckEvents.OnCardSelected += GrabSelectedID;
    }
    private void OnDisable()
    {
        DeckEvents.OnCardSelected -= GrabSelectedID;
    }
    private void GrabSelectedID(int ID) { SelectedMarble = ID; }
}
