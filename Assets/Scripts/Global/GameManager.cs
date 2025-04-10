using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;

public enum TurnState
{
    EnemyTurn,
    WaitingOnPlayerTurn,
    PlayerTurn,
    WaitingOnEnemyTurn,
    GameOver,
    CardSelect
}

public class GameManager : MonoBehaviour
{
    public static GameManager Instance = null;
    public EnemyManager GetEnemyManager() { return EnemyManager; }
    public PlayerManager GetPlayerManager() { return PlayerManager; }
    public DeckManager GetDeckManager() { return DeckManager; }
    public int GetPlayerScore() { return playerScore; }
    public int GetNumWins() { return numWins; }
    public int GetEnemyScore() { return enemyScore; }
    public TurnState GetTurnState() { return turnState; }
    public GameObject GetMainUIButtons() { return MainUIButtons; }

    public bool PlayerHasSelectedMarble() { return PlayerManager.GetPlayerDeck().GetSelectedMarbleIndex() >= 0; }

    public void OverrideTurnState(TurnState newTurnState)
    {
        turnState = newTurnState;
        Debug.Log(turnState);
        TurnStateEvents.OnTurnProgressed(turnState);

        if (turnState == TurnState.CardSelect)
        {
            GoToNextRound();
        }
        else if (turnState == TurnState.GameOver)
        {
            // don't think we're ever hitting this naturally but
            TurnStateEvents.OnGameOvered();
        }
    }
    public void IncremetTurnState()
    {
        if (turnState == TurnState.WaitingOnEnemyTurn)
        {
            turnState = TurnState.EnemyTurn;
        }
        else
        {
            turnState++;
        }

        // At the start of the enemy turn, we want to check whether or not if we use another player marble, if it will be greater. if so we should override and move to card select
        // We go to card select since we assume that we have not yet finished all matches yet
        // This may need to be refactored later.
        if (turnState == TurnState.EnemyTurn)
        {
            if (PlayerManager.GetPlayerDeck().GetNumMarblesUsed() + 1 > PlayerManager.GetPlayerDeck().GetDeckSize())
            {
                OverrideTurnState(TurnState.CardSelect);
                return;
            }
        }

        Debug.Log(turnState);
        TurnStateEvents.OnTurnProgressed(turnState);
    }

    public void UpdateEntityScore(MarbleTeam Team, bool bIsInScoreZone)
    {
        if (Team == MarbleTeam.Player)
        {
            playerScore += bIsInScoreZone ? 1 : -1;
            Mathf.Clamp(playerScore, 0, playerScore);
        }
        else
        {
            enemyScore += bIsInScoreZone ? 1 : -1;
            Mathf.Clamp(enemyScore, 0, playerScore);
        }
        MarbleEvents.OnScoreChanged(Team);
    }
    public GameObject GetScoringCircle() { return ScoringCircle; }
    public bool GetAreMarblesMoving() { return bAreMarblesMoving; }
    public List<Marble> GetMarblesList() { return MarblesList; }
    public void RegisterMarble(Marble MarbleObject)
    {
        MarblesList.Add(MarbleObject);
    }

    public void RemoveMarble(Marble MarbleObject)
    {
        MarblesList.Remove(MarbleObject);
        Destroy(MarbleObject.gameObject);
    }

    public IEnumerator WaitForMarblesToSettle()
    {
        IncremetTurnState();
        bAreMarblesMoving = true;
        yield return new WaitForSeconds(1.0f);


        while (true)
        {
            bool bMarblesSettled = true;
            foreach (Marble marble in MarblesList)
            {
                if (!marble)
                {
                    MarblesToDelete.Add(marble);
                    continue;
                }
                if (!marble.gameObject.activeInHierarchy)
                {
                    continue;
                }
                if (marble.bIsInsideGameplayCircle)
                {
                    Rigidbody physics = marble.GetComponent<Rigidbody>();
                    if (physics.velocity.sqrMagnitude > 0.05f)
                    {
                        bMarblesSettled = false;
                    }
                }
            }

            if (bMarblesSettled)
            {
                break;
            }

            yield return new WaitForSeconds(2.0f);
        }

        yield return new WaitForSeconds(1.0f);

        bAreMarblesMoving = false;
        CleanupMarbles();
        IncremetTurnState();
    }


    [SerializeField]
    private GameObject ScoringCircle;
    [SerializeField]
    private DeckManager DeckManager;
    [SerializeField]
    private PlayerManager PlayerManager;
    [SerializeField]
    private EnemyManager EnemyManager;
    [SerializeField]
    private TurnState turnState = TurnState.EnemyTurn;
    [SerializeField]
    private GameObject MainUIButtons;

    private List<Marble> MarblesList = new List<Marble>();
    private List<Marble> MarblesToDelete = new List<Marble>();
    private int playerScore = 0;
    private int enemyScore = 0;
    private bool bAreMarblesMoving = false;
    private int numWins = 0;
    private int numLosses = 0;
    private int totalGames = 0;
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }
    private void Start()
    {
        if (!ScoringCircle)
        {
            Debug.LogError("Scoring Circle Reference is null (GameManager)");
        }

        ForceUpdateEvents(TurnState.EnemyTurn);

        TurnStateEvents.OnGameOver += OnGameOver;

        Time.timeScale = 2.0f;
    }

    private void OnDestroy()
    {
        TurnStateEvents.OnGameOver -= OnGameOver;
    }

    private void CleanupMarbles()
    {
        if (MarblesList.Count != 0)
        {
            foreach (Marble marble in MarblesList)
            {
                if (!marble.bIsInsideScoringCircle || !marble.bIsInsideGameplayCircle)
                {
                    MarblesToDelete.Add(marble);
                }
            }
        }

        if (MarblesToDelete.Count != 0)
        {
            foreach (Marble marble in MarblesToDelete)
            {
                MarblesList.Remove(marble);
                Destroy(marble.gameObject);
            }
            MarblesToDelete.Clear();
        }
        MarblesToDelete.Clear();
    }
    private void ClearMarbles()
    {
        if (MarblesList.Count != 0)
        {
            foreach (Marble marble in MarblesList)
            {
                MarblesToDelete.Add(marble);
            }
        }

        if (MarblesToDelete.Count != 0)
        {
            foreach (Marble marble in MarblesToDelete)
            {
                MarblesList.Remove(marble);
                Destroy(marble.gameObject);
            }
            MarblesToDelete.Clear();
        }
        MarblesToDelete.Clear();
    }

    public void GoToNextRound()
    {
        ClearMarbles();
        if (playerScore > enemyScore)
        {
            numWins++;
        }
        else
        {
            numLosses++;
        }
        totalGames = numWins + numLosses;
        MarbleEvents.OnRoundsWonChange(totalGames, numWins);
        if (numLosses >= 2 || numWins >= 2) // If the player has lost 2 or won 2 
        {
            ForceUpdateEvents(TurnState.GameOver);
            TurnStateEvents.OnGameOvered();
            return;
        }
        playerScore = 0;
        enemyScore = 0;
        DeckEvents.SelectNewMarbleToAdd(DeckManager.GenerateNewMarbles());
        EnemyManager.InitializeEnemyDeck();
    }
    public void RestartGame()
    {
        ClearMarbles();
        playerScore = 0;
        enemyScore = 0;
        numWins = 0;
        numLosses = 0;
        PlayerManager.InitializePlayerDeck();
        EnemyManager.InitializeEnemyDeck();
        ForceUpdateEvents(TurnState.EnemyTurn);
    }

    public void ForceUpdateEvents(TurnState turnState)
    {
        OverrideTurnState(turnState);
        MarbleEvents.OnScoreChanged(MarbleTeam.Player);
        MarbleEvents.OnScoreChanged(MarbleTeam.Enemy);
    }

    private void OnGameOver()
    {
        // me when ternary 🤩
        AnalyticsManager.SendMetric("game_result", new AnalyticsManager.IntMetric(
            GetPlayerScore() - GetEnemyScore()
        ));
    }
}
