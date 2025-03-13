using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(Deck))]
[RequireComponent(typeof(PlayerController))]
public class PlayerManager : MonoBehaviour
{
    public Deck GetPlayerDeck() { return PlayerDeck; }
    public MarbleTeam GetTeam() { return Team; }
    [SerializeField]
    private int DeckSize = 3;
    [SerializeField]
    private MarbleTeam Team = MarbleTeam.Player;
    private Deck PlayerDeck;
    // Start is called before the first frame update
    void Start()
    {
        PlayerDeck = GetComponent<Deck>();
        InitializePlayerDeck();
    }

    public void InitializePlayerDeck() {
        PlayerDeck.InitializeDeck(Team, DeckSize);
    }
}
