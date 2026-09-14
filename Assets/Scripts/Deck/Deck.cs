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
    public List<MarbleData> GetHand() { return Hand; }
    public int GetMaxHandSize() { return MAX_HAND_SIZE; }
    public int GetSelectedMarbleIndex() { return IndexOfHand; }
    public void ResetSelectedMarbleIndex() { IndexOfHand = -1; }
    public void Reset(MarbleTeam Team, int index)
    {
        if (SelectedMarbleRef != null)
        {
            SelectedMarbleRef.material = null;
        }
        SelectedMarbleRef = null;
        ResetSelectedMarbleIndex();
    }
    public Image SelectedMarbleRef { get; set; }
    public bool bIsHoveringDeck { get; set; }
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
        if (Team == MarbleTeam.Enemy)
        {
            Debug.Log("Enemy Index of Hand: " + IndexOfHand);
        }

        marbleData = Hand[IndexOfHand];
        NumMarblesUsed++;
        if (marbleData == null)
        {
            Debug.LogWarning("Marble data of card is null.");
            return null;
        }
        // Remove the selected card
        DiscardPile.Add(marbleData);
        Hand.RemoveAt(IndexOfHand);
        UpdateHand(Team);

        DeckEvents.MarbleUsed(Team, GetTotalRemainingMarbles());


        return marbleData;
    }

    public void InitializeDeck(MarbleTeam Team, int DeckSize)
    {
        _marbleTeam = Team;
        // Clear the hand, make sure that we do not have anything in the hand at the moment
        Hand.Clear();
        NextIndexToDrawToHand = 0;
        NumMarblesUsed = 0;
        MarbleDeck = GameManager.Instance.GetDeckManager().GenerateDeck(Team, DeckSize);
        ShuffleDeck();
        GenerateInitialHand(Team);
        DeckEvents.DeckGenerated(Team, GetTotalRemainingMarbles());

        if (Team == MarbleTeam.Player)
        {
            SelectedMarbleRef = null;
            bIsHoveringDeck = false;
        }
    }

    public void DrawCard()
    {
        if (MarbleDeck.Count <= 0)
        {
            MarbleDeck = DiscardPile;
            DiscardPile = new List<MarbleData>();
            ShuffleDeck();
        }
        
        if (0 < MarbleDeck.Count && Hand.Count < MAX_HAND_SIZE)
        {
            // Draw a new card from the Marble Deck
            MarbleData marble = MarbleDeck[0];
            Hand.Add(marble);
            MarbleDeck.RemoveAt(0);
            DeckEvents.OnMarbleDrawn(_marbleTeam,MarbleDeck.Count);
        }
        UpdateHand(_marbleTeam);
    }
    
    private void UpdateHand(MarbleTeam Team)
    {
        List<MarbleData> data = new List<MarbleData>();
        // Reset the selected marble index
        IndexOfHand = -1;
        for (int i = 0; i < Hand.Count; ++i)
        {
            data.Add(Hand[i]);
        }
        // HandUpdated signal
        DeckEvents.HandUpdated(Team, data);
    }

    public List<MarbleData> MarbleDeck = new List<MarbleData>();
    public List<MarbleData> DiscardPile = new List<MarbleData>();
    // Contains Indices of cards in the hand
    public List<MarbleData> Hand = new List<MarbleData>();
    // Index that keeps track of the next card to draw to the hand
    private int NextIndexToDrawToHand = 0;
    // The selected marble from your hand
    private int IndexOfHand = -1;
    // Number of marbles used so far
    private int NumMarblesUsed = 0;
    [SerializeField]
    private int MAX_HAND_SIZE = 7;
    [SerializeField]
    private int INIT_HAND_SIZE = 3;

    private MarbleTeam _marbleTeam;
    public void GenerateInitialHand(MarbleTeam Team)
    {
        if (MarbleDeck.Count == 0)
        {
            Debug.LogWarning("Deck.GenerateInitialHand() The Marble Deck is currently empty.");
            NextIndexToDrawToHand = -1;
            return;
        }
        // Reset the hand if something already exists
        if (Hand.Count != 0)
        {
            Hand.Clear();
        }

        int HandSizeModified = INIT_HAND_SIZE;
        // If the deck's total size is actually smaller than the smallest hand size
        if (INIT_HAND_SIZE > MarbleDeck.Count)
        {
            HandSizeModified = MarbleDeck.Count;
        }

        if (!GameManager.DrawnNewHandEachTurn)
        {
            for (int i = 0; i < HandSizeModified; ++i)
            {
                DrawCard();
            }
        }
        /*
        NextIndexToDrawToHand = HandSizeModified;
        NumMarblesUsed = 0;

        List<MarbleData> data = new List<MarbleData>();
        for (int i = 0; i < Hand.Count; ++i)
        {
            data.Add(MarbleDeck[Hand[i]]);
        }

        DeckEvents.HandUpdated(Team, data);
        DeckEvents.MarbleUsed(Team, GetTotalRemainingMarbles());
        */
    }
    private void OnEnable()
    {
        DeckEvents.OnMarbleSelectedFromHand += GrabSelectedID;
        DeckEvents.OnMarbleUsed += Reset;
        TurnStateEvents.OnTurnProgress += OnTurnProgress;
    }

    private void OnDisable()
    {
        DeckEvents.OnMarbleSelectedFromHand -= GrabSelectedID;
        DeckEvents.OnMarbleUsed -= Reset;
        TurnStateEvents.OnTurnProgress -= OnTurnProgress;
    }
    
    private void OnTurnProgress(TurnState turn)
    {
        if (_marbleTeam == MarbleTeam.Player)
        {
            if (turn == TurnState.PlayerTurn)
            {
                DrawCard();
                if (GameManager.DrawnNewHandEachTurn)
                {
                    DrawCard();
                    DrawCard();
                }
            }
            else if (turn == TurnState.WaitingOnEnemyTurn && GameManager.DrawnNewHandEachTurn)
            {
                DiscardPile.AddRange(Hand);
                Hand.Clear();
                UpdateHand(_marbleTeam);
            }
        }
        else if(_marbleTeam == MarbleTeam.Enemy && turn == TurnState.EnemyTurn)
        {
            DrawCard();
            DrawCard();
        }
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
