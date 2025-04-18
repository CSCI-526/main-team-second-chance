using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(Deck))]
[RequireComponent(typeof(PlayerController))]
public class PlayerManager : MonoBehaviour
{
    public Deck GetPlayerDeck() { return PlayerDeck; }
    public bool isLaunchingMarble { get; set; }

    public MarbleTeam GetTeam() { return Team; }
    [SerializeField]
    private int DeckSize = 3;
    [SerializeField]
    private MarbleTeam Team = MarbleTeam.Player;
    [SerializeField]
    private Deck PlayerDeck;
    // Start is called before the first frame update
    void Start()
    {
        PlayerDeck = GetComponent<Deck>();
        InitializePlayerDeck();
    }
    private void OnEnable()
    {
        DeckEvents.OnAddNewMarbleToDeck += AddMarbleToDeck;
    }
    private void OnDisable()
    {
        DeckEvents.OnAddNewMarbleToDeck -= AddMarbleToDeck;
    }
    public void InitializePlayerDeck()
    {
        if (NodeManager.Instance.GetPlayerDeck().Count != 0)
        {
            PlayerDeck.MarbleDeck = NodeManager.Instance.GetPlayerDeck();
            PlayerDeck.GenerateInitialHand(Team);
        }
        else
        {
            PlayerDeck.InitializeDeck(Team, DeckSize);
            NodeManager.Instance.UpdatePlayerDeck(PlayerDeck.MarbleDeck);
        }
        DeckEvents.OnPlayerDeckInitialized();
    }
    private void AddMarbleToDeck(MarbleData gameObject)
    {
        if (!gameObject)
        {
            return;
        }
        PlayerDeck.AddMarbleToDeck(Team, gameObject);
        NodeManager.Instance.UpdatePlayerDeck(PlayerDeck.MarbleDeck);

        // We need to override the turn back to enemy turn since we just added a new marble 
        // Eventually we probably want to just turn this to like the levelselect state
        GameManager.Instance.OverrideTurnState(TurnState.EnemyTurn);

        AnalyticsManager.SendMetric("new_marble_choice", new AnalyticsManager.StringMetric(
                gameObject.MarbleName
        ));

    }
}
