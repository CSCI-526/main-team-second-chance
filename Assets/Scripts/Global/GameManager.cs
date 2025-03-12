using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI;

public enum TurnState
{
    PlayerTurn,
    WaitingOnEnemyTurn,
    EnemyTurn,
    WaitingOnPlayerTurn
}

public class GameManager : MonoBehaviour
{
    public static GameManager Instance = null;
    public PlayerManager GetPlayerManager() { return PlayerManager; }
    public DeckManager GetDeckManager() { return DeckManager; }
    public int GetPlayerScore() { return playerScore; }
    public int GetEnemyScore() { return enemyScore; }

    public TurnState turnState = TurnState.PlayerTurn;
    public TurnState GetTurnState() { return turnState; }

    public void IncremetTurnState()
    {
        if (TurnState.WaitingOnPlayerTurn == turnState)
        {
            turnState = TurnState.PlayerTurn;
        }
        else
        {
            turnState++;
        }

        TurnStateEvents.OnTurnProgressed(turnState);

        Debug.Log(turnState);
        
        if (turnState == TurnState.EnemyTurn)
        {
            EnemyController.ins.ShootMarble();
        }
    }
    
    public void UpdateEntityScore(MarbleTeam Team, bool bIsInScoreZone) 
    {
        if(Team == MarbleTeam.Player)
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
                if (marble.bIsInsideGameplayCircle)
                {
                    Rigidbody physics = marble.GetComponent<Rigidbody>();
                    if (physics.velocity.sqrMagnitude > 0.1f)
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
    private List<Marble> MarblesList = new List<Marble>();
    private List<Marble> MarblesToDelete = new List<Marble>();
    private int playerScore = 0;
    private int enemyScore = 0;
    private bool bAreMarblesMoving = false;

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
        MarbleEvents.OnScoreChanged(MarbleTeam.Player);
        MarbleEvents.OnScoreChanged(MarbleTeam.Enemy);
    }

    private void CleanupMarbles()
    {
        if (MarblesList.Count != 0)
        {
            foreach (Marble marble in MarblesList)
            {
                if (!marble.bIsInsideScoringCircle)
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
    }
}
