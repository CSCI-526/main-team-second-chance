using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UI;

public class Deck : MonoBehaviour
{
    public int GetDeckSize() { return MarbleDeck.Count; }
    public int GetHandSize() { return Hand.Count; }
    public int GetTotalRemainingMarbles()
    {
        return Mathf.Clamp((GetDeckSize() - NumMarblesUsed - GetHandSize()), 0, GetDeckSize());
    }
    public int GetNumMarblesUsed()
    {
        return NumMarblesUsed;
    }
    public List<int> GetHand() { return Hand; }
    public int GetMaxHandSize() { return MAX_HAND_SIZE; }
    public int GetSelectedMarbleIndex() { return IndexOfHand; }
    public void ResetSelectedMarbleIndex() { IndexOfHand = -1; }
    public Image SelectedMarbleRef {get; set;}
    public bool bIsHoveringDeck {get; set;}
    public void AddMarbleToDeck(MarbleTeam Team, MarbleData marble)
    {
        MarbleDeck.Add(marble);
        NumMarblesUsed = 0;
        ShuffleDeck();
        GenerateInitialHand(Team);
    }
    public MarbleData UseMarble(MarbleTeam Team)
    {
        MarbleData marbleData = null;

        if (Team == MarbleTeam.Enemy)
        {
            EnemyChooseHandIndex();
        }
        // This means that we didn't select a marble from the hand
        if (IndexOfHand < 0 || IndexOfHand >= Hand.Count)
        {
            Debug.LogWarning("Deck.UseMarble(): The IndexOfHand is not valid. Something Wrong has happened");
            return null;
        }
        int DeckIndex = Hand[IndexOfHand];
        if (Team == MarbleTeam.Enemy)
        {
            Debug.Log("Enemy Index of Hand: " + IndexOfHand + " \nEnemy DeckIndex: " + DeckIndex);
        }

        if (DeckIndex < 0 || DeckIndex >= MarbleDeck.Count)
        {
            Debug.LogWarning("Deck.UseMarble(): The DeckIndex is not valid. Something Wrong has happened");
            return null;
        }

        marbleData = MarbleDeck[DeckIndex];
        NumMarblesUsed++;
        if (NumMarblesUsed > MarbleDeck.Count)
        {
            Debug.LogWarning("We have used up all our marbles.");
            return null;
        }

        UpdateHand(Team);

        DeckEvents.MarbleUsed(Team, GetTotalRemainingMarbles());


        return marbleData;
    }

    public void InitializeDeck(MarbleTeam Team, int DeckSize)
    {
        // Clear the hand, make sure that we do not have anything in the hand at the moment
        Hand.Clear();
        NextIndexToDrawToHand = 0;
        NumMarblesUsed = 0;
        MarbleDeck = GameManager.Instance.GetDeckManager().GenerateDeck(Team, DeckSize);
        ShuffleDeck();
        GenerateInitialHand(Team);
        DeckEvents.DeckGenerated(Team, GetTotalRemainingMarbles());

        if (Team == MarbleTeam.Player) {
            SelectedMarbleRef = null;
            bIsHoveringDeck = false;
        }
    }
    public void UpdateHand(MarbleTeam Team)
    {
        // Remove the selected card
        Hand.RemoveAt(IndexOfHand);

        List<MarbleData> data = new List<MarbleData>();
        if (NextIndexToDrawToHand >= MarbleDeck.Count)
        {
            Debug.LogWarning("Deck.UpdateHand():" + Team + " We are out of marbles that we can add to hand.");
            // Set the selected marble index to -1 again
            IndexOfHand = -1;
            // HandUpdated signal
            for (int i = 0; i < Hand.Count; ++i)
            {
                data.Add(MarbleDeck[Hand[i]]);
            }
            DeckEvents.HandUpdated(Team, data);
            return;
        }
        // Draw a new card from the Marble Deck
        Hand.Add(NextIndexToDrawToHand++);
        // Reset the selected marble index
        IndexOfHand = -1;
        for (int i = 0; i < Hand.Count; ++i)
        {
            data.Add(MarbleDeck[Hand[i]]);
        }
        // HandUpdated signal
        DeckEvents.HandUpdated(Team, data);
    }

    public List<MarbleData> MarbleDeck = new List<MarbleData>();
    // Contains Indices of cards in the hand
    public List<int> Hand = new List<int>();
    // Index that keeps track of the next card to draw to the hand
    private int NextIndexToDrawToHand = 0;
    // The selected marble from your hand
    private int IndexOfHand = -1;
    // Number of marbles used so far
    private int NumMarblesUsed = 0;
    [SerializeField]
    private int MAX_HAND_SIZE = 5;
    [SerializeField]
    private int INIT_HAND_SIZE = 3;
    public void GenerateInitialHand(MarbleTeam Team)
    {
        if (MarbleDeck.Count == 0)
        {
            Debug.LogWarning("Deck.GenerateInitialHand() The Marble Deck is currently empty.");
            NextIndexToDrawToHand = -1;
            return;
        }

        if (INIT_HAND_SIZE > MarbleDeck.Count)
        {
            for (int i = 0; i < MarbleDeck.Count; ++i)
            {
                Hand.Add(i);
            }
        }
        else
        {
            for (int i = 0; i < INIT_HAND_SIZE; ++i)
            {
                Hand.Add(i);
            }
        }
        
        NextIndexToDrawToHand = INIT_HAND_SIZE;

        List<MarbleData> data = new List<MarbleData>();
        for (int i = 0; i < Hand.Count; ++i)
        {
            data.Add(MarbleDeck[Hand[i]]);
        }
        
        DeckEvents.HandUpdated(Team, data);
        DeckEvents.MarbleUsed(Team, GetTotalRemainingMarbles());
    }
    private void OnEnable()
    {
        DeckEvents.OnMarbleSelectedFromHand += GrabSelectedID;
    }
    private void OnDisable()
    {
        DeckEvents.OnMarbleSelectedFromHand -= GrabSelectedID;
    }
    private void GrabSelectedID(MarbleTeam Team, int ID)
    {
        if (this == GameManager.Instance.GetPlayerManager().GetPlayerDeck())
        {
            IndexOfHand = ID;
        }
    }
    private void EnemyChooseHandIndex()
    {
        IndexOfHand = Random.Range(0, Hand.Count);
    }
    private void ShuffleDeck()
    {
        for (int i = MarbleDeck.Count - 1; i > 0; i--)
        {
            int randomIndex = Random.Range(0, i + 1);
            (MarbleDeck[i], MarbleDeck[randomIndex]) = (MarbleDeck[randomIndex], MarbleDeck[i]); // Swap
        }
    }
}
