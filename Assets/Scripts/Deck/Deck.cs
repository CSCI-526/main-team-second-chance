using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public struct DeckItem
{
    public MarbleData MarbleData;
    public bool bHasBeenUsed;
}
public class Deck : MonoBehaviour
{
    /* MARKED DONE */
    public int GetDeckSize() { return MarbleDeck.Count; }
    /* MARKED DONE */
    public int GetHandSize() { return Hand.Count; }
    /* TO DO REFACTOR */
    public int GetTotalRemainingMarbles() { return MarbleDeck.Count - NumUsedCards; }
    /* MARKED DONE */
    public List<int> GetHand() { return Hand; }
    /* MARKED DONE */
    public int GetMaxHandSize() { return MAX_HAND_SIZE; }
    /* MARKED DONE */
    public int GetSelectedMarbleIndex() { return IndexOfHand; }
    /* MARKED DONE */
    public void AddMarbleToDeck(MarbleTeam Team, DeckItem marble)
    {
        MarbleDeck.Add(marble);
        ResetDeck();
        ShuffleDeck();
        GenerateInitialHand(Team);
        GameManager.Instance.turnState = TurnState.PlayerTurn;
        GameManager.Instance.ForceUpdateEvents();
    }
    /* MARKED DONE */
    public MarbleData UseMarble(MarbleTeam Team)
    {
        MarbleData marbleData = null;
        // This means that we didn't select a marble from the hand
        if (IndexOfHand < 0 || IndexOfHand >= Hand.Count)
        {
            Debug.LogWarning("Deck.UseMarble(): The IndexOfHand is not valid. Something Wrong has happened");
            return marbleData;
        }
        int DeckIndex = Hand[IndexOfHand];

        if (DeckIndex < 0 || DeckIndex >= MarbleDeck.Count)
        {
            Debug.LogWarning("Deck.UseMarble(): The DeckIndex is not valid. Something Wrong has happened");
            return marbleData;
        }

        marbleData = MarbleDeck[DeckIndex].MarbleData;
        NumUsedCards++;
        UpdateHand(Team);

        DeckEvents.MarbleUsed(Team, GetTotalRemainingMarbles());

        if (GetTotalRemainingMarbles() == 0)
        {
            Debug.LogWarning("We have used up all our marbles.");
            return null;
        }
        return marbleData;
    }

    /* MARK DONE */
    public void InitializeDeck(MarbleTeam Team, int DeckSize)
    {
        // Clear the hand, make sure that we do not have anything in the hand at the moment
        Hand.Clear();
        NextIndexToDrawToHand = 0;
        MarbleDeck = GameManager.Instance.GetDeckManager().GenerateDeck(Team, DeckSize);
        DeckEvents.DeckGenerated(Team, MarbleDeck.Count);
        ShuffleDeck();
        GenerateInitialHand(Team);
    }
    /* TO DO REFACTOR UPDATE HAND*/
    public void UpdateHand(MarbleTeam Team)
    {
        // Remove the selected card
        Hand.RemoveAt(IndexOfHand);


        if (NextIndexToDrawToHand >= MarbleDeck.Count)
        {
            Debug.LogError("Deck.UpdateHand(): We are out of marbles.");
            // Set the selected marble index to -1 again
            IndexOfHand = -1;
            // HandUpdated signal
            DeckEvents.HandUpdated(Team, null);
            return;
        }
        // Draw a new card from the Marble Deck
        Hand.Add(NextIndexToDrawToHand++);
        // Reset the selected marble index
        IndexOfHand = -1;
        List<MarbleData> data = new List<MarbleData>();
        for (int i = 0; i < Hand.Count; ++i)
        {
            data.Add(MarbleDeck[Hand[i]].MarbleData);
        }
        // HandUpdated signal
        DeckEvents.HandUpdated(Team, data);
    }

    public List<DeckItem> MarbleDeck = new List<DeckItem>();
    // Contains Indices of cards in the hand
    public List<int> Hand = new List<int>();
    // Index that keeps track of the next card to draw to the hand
    private int NextIndexToDrawToHand = 0;
    // The selected marble from your hand
    private int IndexOfHand = -1;
    // number of cards that have been used 
    private int NumUsedCards = 0;
    [SerializeField]
    private int MAX_HAND_SIZE = 5;
    [SerializeField]
    private int INIT_HAND_SIZE = 3;
    /* MARKED DONE */
    private void GenerateInitialHand(MarbleTeam Team)
    {
        if (MarbleDeck.Count == 0)
        {
            Debug.LogWarning("Deck.GenerateInitialHand() The Marble Deck is currently empty.");
            NextIndexToDrawToHand = -1;
            return;
        }

        for (int i = 0; i < INIT_HAND_SIZE; ++i)
        {
            Hand.Add(i);
        }
        NextIndexToDrawToHand = INIT_HAND_SIZE;

        List<MarbleData> data = new List<MarbleData>();
        for (int i = 0; i < Hand.Count; ++i)
        {
            data.Add(MarbleDeck[Hand[i]].MarbleData);
        }
        DeckEvents.HandUpdated(Team, data);
        DeckEvents.MarbleUsed(Team, MarbleDeck.Count);
    }
    /* MARKED DONE */
    private void OnEnable()
    {
        DeckEvents.OnMarbleSelectedFromHand += GrabSelectedID;
    }
    /* MARKED DONE */
    private void OnDisable()
    {
        DeckEvents.OnMarbleSelectedFromHand -= GrabSelectedID;
    }
    /* MARKED DONE */
    private void GrabSelectedID(MarbleTeam Team, int ID) 
    {
        if(Team == MarbleTeam.Enemy)
        {
            return;
        }
        IndexOfHand = ID; 
    }
    private void EnemyChooseHandIndex()
    {
        IndexOfHand = Random.Range(0, Hand.Count);
    }
    /* MARKED DONE*/
    private void ShuffleDeck()
    {
        for (int i = MarbleDeck.Count - 1; i > 0; i--)
        {
            int randomIndex = Random.Range(0, i + 1);
            (MarbleDeck[i], MarbleDeck[randomIndex]) = (MarbleDeck[randomIndex], MarbleDeck[i]); // Swap
        }
    }
    /* TO DO REFACTOR */
    private void ResetDeck()
    {
        for(int i = 0; i < MarbleDeck.Count; i++)
        {
            DeckItem Item = MarbleDeck[i];
            Item.bHasBeenUsed = false;
        }
    }
}
