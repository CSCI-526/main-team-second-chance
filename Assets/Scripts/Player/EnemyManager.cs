using System.Collections;
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
    private EnemyController EnemyController;

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
        TurnStateEvents.OnTurnProgress += EnemyShootMarble;
    }
    private void OnDisable()
    {
        TurnStateEvents.OnTurnProgress -= EnemyShootMarble;
    }
    public void InitializeEnemyDeck()
    {
        EnemyDeck.InitializeDeck(Team, DeckSize);
    }
    private void EnemyShootMarble(TurnState turnState)
    {
        if (turnState != TurnState.EnemyTurn)
        {
            return;
        }

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
    // LEGACY
    IEnumerator MarbleRepeater()
    {
        while (true)
        {
            yield return new WaitForSeconds(0.5f);
            if (!EnemyDeck)
            {
                InitializeEnemyDeck();
            }

            MarbleData MarbleObject = EnemyDeck.UseMarble(Team);
            if (!MarbleObject)
            {
                yield return null;
            }

            EnemyController.ShootMarble(MarbleObject);
            yield return new WaitForSeconds(2.0f);
        }
    }
}
