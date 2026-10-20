using System.Collections;
using UnityEngine;
[RequireComponent(typeof(Deck))]
[RequireComponent(typeof(EnemyController))]

public class EnemyManager : MonoBehaviour
{
    public Deck GetEnemyDeck() { return EnemyDeck; }

    public HealthManager GetHealthManager() { return EnemyHealthManager;}
    public MarbleTeam GetTeam() { return Team; }
    [SerializeField]
    private int DeckSize = 12;
    [SerializeField] 
    private int marblesPerRound = 2;
    [SerializeField]
    private MarbleTeam Team = MarbleTeam.Enemy;
    private Deck EnemyDeck;
    [SerializeField] private HealthManager EnemyHealthManager;
    private EnemyController EnemyController;
    private int _marblesPlayed = 0;

    void Start()
    {
        if (!EnemyDeck || !EnemyController)
        {
            EnemyDeck = GetComponent<Deck>();
            EnemyController = GetComponent<EnemyController>();
            InitializeEnemyDeck();
        }
    }
    private void OnEnable()
    {
        TurnStateEvents.OnTurnProgress += OnTurnStart;
        TurnStateEvents.OnMarblesSettle += OnMarblesSettle;
    }

    private void OnDisable()
    {
        TurnStateEvents.OnTurnProgress -= OnTurnStart;
        TurnStateEvents.OnMarblesSettle -= OnMarblesSettle;
    }
    public void InitializeEnemyDeck()
    {
        EnemyDeck.InitializeDeck(Team, DeckSize);
        DeckEvents.OnDeckInitialized(Team,EnemyDeck.GetDeckSize());
    }
    public void InitializeLevelData(AggressionLevel newLevel, float newSkill)
    {
        Debug.Log("New AggressionLevel " + newLevel + " new Skill: " + newSkill);
        if (!EnemyDeck || !EnemyController)
        {
            EnemyDeck = GetComponent<Deck>();
            EnemyController = GetComponent<EnemyController>();
            InitializeEnemyDeck();
        }

        EnemyController.SetAggression(newLevel, newSkill);
    }
    private void OnTurnStart(TurnState turnState)
    {
        if (turnState != TurnState.EnemyTurn)
        {
            return;
        }

        _marblesPlayed = 0;
        PlayMarble();
    }
    
    private void OnMarblesSettle(TurnState turn)
    {
        if (turn != TurnState.EnemyTurn)
        {
            return;
        }

        if (_marblesPlayed < marblesPerRound)
        {
            PlayMarble();
        }
        else
        {
            TurnStateEvents.OnEndTurnPressed(TurnState.EnemyTurn);
        }
    }

    private void PlayMarble()
    {
        _marblesPlayed++;
        if (!EnemyDeck)
        {
            EnemyDeck = GetComponent<Deck>();
            InitializeEnemyDeck();
        }

        MarbleData MarbleObject = EnemyDeck.UseMarble(Team);
        if (!MarbleObject)
        {
            return;
        }
        if (!EnemyController)
        {
            EnemyController = GetComponent<EnemyController>();
        }
        EnemyController.ShootMarble(MarbleObject);
    }

    public int GetMarblesPerRound()
    {
        return marblesPerRound;
    }
}
