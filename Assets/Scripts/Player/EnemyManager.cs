using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(Deck))]
[RequireComponent(typeof(EnemyController))]

public class EnemyManager : MonoBehaviour
{
    public Deck GetEnemyDeck() { return EnemyDeck; }
    public MarbleTeam GetTeam() { return Team; }
    [SerializeField]
    private int DeckSize = 12;
    [SerializeField]
    private MarbleTeam Team = MarbleTeam.Enemy;
    private Deck EnemyDeck;
    // Start is called before the first frame update
    void Start()
    {
        EnemyDeck = GetComponent<Deck>();
        InitializeEnemyDeck();
    }

    public void InitializeEnemyDeck()
    {
        EnemyDeck.InitializeDeck(Team, DeckSize);
    }
}
