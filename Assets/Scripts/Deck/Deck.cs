using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Deck : MonoBehaviour
{
    public int GetDeckSize() { return MarbleDeck.Count; }
    public int GetHandSize() { return Hand.Count; }
    public int GetTotalRemainingMarbles() { return MarbleDeck.Count + Hand.Count; }
    public List<GameObject> GetHand() { return Hand; }
    public int GetMaxHandSize() { return MAX_HAND_SIZE; }
    public int GetSelectedMarbleIndex() { return SelectedMarble; }
    public GameObject UseMarble(MarbleTeam Team)
    {
        GameObject marble = null;

        if (Team == MarbleTeam.Player)
        {
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

        if (MarbleDeck.Count == 0)
        {
            Debug.LogWarning("Deck.UseMarble(): EnemyMarble Deck's size is now 0.");

            return null;
        }
        marble = MarbleDeck[0];
        MarbleDeck.RemoveAt(0);

        return MarbleDeck[0];
    }
    public void InitializeDeck(MarbleTeam Team, int DeckSize)
    {
        Hand.Clear();
        MarbleDeck = GameManager.Instance.GetDeckManager().GenerateDeck(Team, DeckSize);
        DeckEvents.DeckGenerated(Team, MarbleDeck.Count);
        ShuffleDeck();
        if (Team == MarbleTeam.Player)
        {
            GenerateInitialHand();
            DeckEvents.HandUpdated();
        }
    }

    public void UpdateHand()
    {
        // Remove the selected card
        Hand.RemoveAt(SelectedMarble);

        if (MarbleDeck.Count <= 0)
        {
            Debug.LogError("Deck.UpdateHand(): MarbleDeck's size is now 0. We can no longer update the hand.");
            // Set the selected marble index to -1 again
            SelectedMarble = -1;
            // HandUpdated signal
            DeckEvents.HandUpdated();

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
        DeckEvents.OnMarbleSelectedFromHand += GrabSelectedID;
    }
    private void OnDisable()
    {
        DeckEvents.OnMarbleSelectedFromHand -= GrabSelectedID;
    }
    private void GrabSelectedID(int ID) { SelectedMarble = ID; }
    private void ShuffleDeck()
    {
        for (int i = MarbleDeck.Count - 1; i > 0; i--)
        {
            int randomIndex = Random.Range(0, i + 1);
            (MarbleDeck[i], MarbleDeck[randomIndex]) = (MarbleDeck[randomIndex], MarbleDeck[i]); // Swap
        }
    }
}
