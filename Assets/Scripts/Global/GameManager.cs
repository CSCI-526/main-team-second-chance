using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI;

public enum TurnState
{
    PlayerTurn,
    WaitingOnEnemyTurn,
    EnemyTurn,
    WaitingOnPlayerTurn,
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

    public TurnState turnState = TurnState.PlayerTurn;
    public TurnState GetTurnState() { return turnState; }

    public void IncremetTurnState()
    {
        if (TurnState.WaitingOnPlayerTurn == turnState)
        {
            if (PlayerManager.GetPlayerDeck().GetTotalRemainingMarbles() > 0)
            {
                turnState = TurnState.PlayerTurn;
            }
            else
            {
                // Changing to card select for now
                turnState = TurnState.CardSelect;
            }
        }
        else
        {
            turnState++;
            // Skip over game over, conditions are not met here.
            if (turnState == TurnState.GameOver)
            {
                turnState = TurnState.PlayerTurn;
            }
        }

        Debug.Log(turnState);
        if (turnState == TurnState.CardSelect)
        {
            GoToNextRound();
        }
        else if (turnState == TurnState.GameOver)
        {
            // don't think we're ever hitting this naturally but
            TurnStateEvents.OnGameOvered();
        }
        else
        {
            TurnStateEvents.OnTurnProgressed(turnState);
        }



        if (turnState == TurnState.EnemyTurn)
        {
            EnemyController.ins.ShootMarble();
        }
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
        ForceUpdateEvents();

        TurnStateEvents.OnGameOver += OnGameOver;
    }

    private void OnDestroy()
    {
        TurnStateEvents.OnGameOver -= OnGameOver;
    }

    private void CleanupMarbles()
    {
        List<int> cleanupIndices = new List<int>();

        if (MarblesList.Count != 0)
        {
            for (int i = 0; i < MarblesList.Count; ++i)
            {
                if (!MarblesList[i])
                {
                    cleanupIndices.Add(i);
                }
                else
                {
                    if (!MarblesList[i].bIsInsideScoringCircle)
                    {
                        MarblesList[i].gameObject.SetActive(false);
                    }
                }
            }
        }

        if (cleanupIndices.Count != 0)
        {
            foreach (int marble in cleanupIndices)
            {
                MarblesList.RemoveAt(marble);
            }
        }

    }
    public void HardClearMarbles()
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
    public void ClearMarbles()
    {
        // only Delete the marbles that are enemy
        if (MarblesList.Count != 0)
        {
            foreach (Marble marble in MarblesList)
            {
                if (marble.Team != MarbleTeam.Player)
                {
                    MarblesToDelete.Add(marble);
                }
                else
                {
                    marble.gameObject.SetActive(false);
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
        MarbleEvents.OnRoundsWonChange(numWins);
        if (totalGames == 3) // prob not a magic num but w/e
        {
            turnState = TurnState.GameOver;
            ForceUpdateEvents();
            TurnStateEvents.OnGameOvered();
            return;
        }
        playerScore = 0;
        enemyScore = 0;
        DeckEvents.SelectNewMarbleToAdd(DeckManager.GenerateNewMarbles());
        EnemyManager.InitializeEnemyDeck();
        ForceUpdateEvents();

    }
    public void RestartGame()
    {
        HardClearMarbles();
        playerScore = 0;
        enemyScore = 0;
        PlayerManager.InitializePlayerDeck();
        EnemyManager.InitializeEnemyDeck();
        turnState = TurnState.PlayerTurn;
        ForceUpdateEvents();
    }

    public void ForceUpdateEvents()
    {
        TurnStateEvents.OnTurnProgressed(turnState);
        MarbleEvents.OnScoreChanged(MarbleTeam.Player);
        MarbleEvents.OnScoreChanged(MarbleTeam.Enemy);
    }

    private void OnGameOver()
    {
        // me when ternary 🤩
        string gameResult = numWins == numLosses ? "draw" : numWins > numLosses ? "win" : "lose";
        AnalyticsManager.SendMetric("game_result", new AnalyticsManager.StringMetric(gameResult));
    }
}
