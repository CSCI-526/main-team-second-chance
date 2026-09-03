using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using Random = UnityEngine.Random;

public enum TurnState
{
    EnemyTurn,
    WaitingOnPlayerTurn,
    PlayerTurn,
    WaitingOnEnemyTurn,
    GameOver,
    CardSelect,
    MatchEnd
}

public class GameManager : MonoBehaviour
{
    public static GameManager Instance = null;

    public EnemyManager GetEnemyManager()
    {
        if (!EnemyManager)
        {
            GameObject EnemyManagerGO = GameObject.Find("EnemyManager");
            if (EnemyManagerGO)
            {
                EnemyManager = EnemyManagerGO.GetComponent<EnemyManager>();
            }
        }

        return EnemyManager;
    }

    public PlayerManager GetPlayerManager()
    {
        if (!PlayerManager)
        {
            GameObject PlayerManagerGO = GameObject.Find("PlayerManager");
            if (PlayerManagerGO)
            {
                PlayerManager = PlayerManagerGO.GetComponent<PlayerManager>();
            }
        }

        return PlayerManager;
    }

    public DeckManager GetDeckManager()
    {
        if (!DeckManager)
        {
            GameObject DeckManagerGO = GameObject.Find("DeckManager");
            if (DeckManagerGO)
            {
                DeckManager = DeckManagerGO.GetComponent<DeckManager>();
            }
        }

        return DeckManager;
    }

    public ScoringZoneManager GetScoringZoneManager()
    {
        if (!scoringZoneManager)
        {
            GameObject scoreGO = GameObject.Find("ScoringZoneManager");
            if (scoreGO)
            {
                scoringZoneManager = scoreGO.GetComponent<ScoringZoneManager>();
            }
        }

        return scoringZoneManager;
    }

    public int GetPlayerScore()
    {
        return playerScore;
    }

    public int GetEnemyScore()
    {
        return enemyScore;
    }

    public TurnState GetTurnState()
    {
        return turnState;
    }

    public GameObject GetMainUIButtons()
    {
        return MainUIButtons;
    }

    public bool PlayerHasSelectedMarble()
    {
        return PlayerManager.GetPlayerDeck().GetSelectedMarbleIndex() >= 0;
    }

    private void OverrideTurnState(TurnState newTurnState)
    {
        turnState = newTurnState;
        Debug.Log(turnState);
        TurnStateEvents.OnTurnProgressed(turnState);

        if (turnState == TurnState.CardSelect)
        {
            GoToCardSelect();
        }
        else if (turnState == TurnState.GameOver)
        {
            TurnStateEvents.OnGameOvered();
        }
        else if (turnState == TurnState.MatchEnd)
        {
            StartCoroutine(MatchEnded());
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

        if (turnState == TurnState.PlayerTurn)
        {
            if (TutorialManager.Instance.ShouldDisplayAnymore)
            {
                TutorialEvents.DoTryDisplayTutorialItem(TutorialManager.Instance.CurrentTutorialPhase);
            }

            numPlayerTurns++;
        }
        else if (turnState == TurnState.WaitingOnPlayerTurn)
        {
            if (TutorialManager.Instance.ShouldDisplayAnymore)
            {
                if (TutorialManager.Instance.CurrentTutorialPhase >= TutorialPhases.LAUNCH_MARBLE)
                {
                    TutorialEvents.DoTutorialItemDisplayed(TutorialManager.Instance.CurrentTutorialPhase);
                }
            }
        }

        // At the start of the enemy turn, we want to check whether or not if we use another player marble, if it will be greater. if so we should override and move to card select
        // We go to card select since we assume that we have not yet finished all matches yet
        if (turnState == TurnState.EnemyTurn)
        {
            if (PlayerManager.GetPlayerDeck().GetNumMarblesUsed() + 1 > PlayerManager.GetPlayerDeck().GetDeckSize() ||
                bInSuddenDeath)
            {
                if (enemyScore == playerScore)
                {
                    if (!bInSuddenDeath)
                    {
                        bInSuddenDeath = true;
                        PlayerManager.InitializePlayerDeck();
                        TurnStateEvents.DoSuddenDeath();
                        StartCoroutine(SuddenDeathRoutine());
                        AudioManager.TriggerSound(SuddenDeath, transform.position);
                        // we'll notify the next turn from the scoring cirlce because I hate code quality :))
                        return;
                    }
                    // protect from double deck out
                    if ((PlayerManager.GetPlayerDeck().GetNumMarblesUsed()) %
                        PlayerManager.GetPlayerDeck().GetDeckSize() == 0)
                    {
                        PlayerManager.InitializePlayerDeck();
                    }
                }
                else
                {
                    // Match IS OVER HERE
                    bool oldSuddenDeath = bInSuddenDeath;
                    bInSuddenDeath = false;
                    if (oldSuddenDeath)
                    {
                        StartCoroutine(SuddenDeathRoutine());
                    }

                    OverrideTurnState(TurnState.MatchEnd);
                    return;
                }
            }
        }

        Debug.Log(turnState);
        TurnStateEvents.OnTurnProgressed(turnState);
    }

    IEnumerator SuddenDeathRoutine()
    {
        float timer = 0.0f;
        float length = 3.0f;
        while (timer < length)
        {
            float t = timer / length;
            if (bInSuddenDeath) t = 1 - t;
            scoringZoneManager.SetScoringCircleScales(t);
            
            timer += Time.deltaTime;
            yield return null;
        }
        
        if (bInSuddenDeath)
        {
            CleanupMarbles();
            TurnStateEvents.OnTurnProgressed(TurnState.EnemyTurn);
        }
        else
        {
            scoringZoneManager.SetScoringCircleScales(1.0f);
        }
    }
    

    public void UpdateEntityScore(MarbleTeam Team, bool bIsInScoreZone)
    {
        if (Team == MarbleTeam.Player)
        {
            playerScore += bIsInScoreZone ? 1 : -1;
        }
        else
        {
            enemyScore += bIsInScoreZone ? 1 : -1;
        }

        if (bIsInScoreZone)
        {
            AudioManager.TriggerSound(GainPoints, transform.position);
        }
        else
        {
            AudioManager.TriggerSound(LosePoints, transform.position);
        }

        MarbleEvents.OnScoreChanged(Team);
    }

    public bool GetAreMarblesMoving()
    {
        return bAreMarblesMoving;
    }

    public List<Marble> GetMarblesList()
    {
        return MarblesList;
    }

    private void RegisterMarble(Marble MarbleObject)
    {
        MarblesList.Add(MarbleObject);
    }

    private void RemoveMarble(Marble MarbleObject)
    {
        MarblesList.Remove(MarbleObject);
        Destroy(MarbleObject.gameObject);
    }

    private void BeginWaitForMarblesToSettle()
    {
        StartCoroutine(Instance.WaitForMarblesToSettle());
    }

    private IEnumerator WaitForMarblesToSettle()
    {
        IncremetTurnState();
        bAreMarblesMoving = true;
        yield return new WaitForSeconds(1.0f);

        int timeWaited = 0;
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
                    Rigidbody physics = marble.GetMarbleRigidbody();
                    if (physics.velocity.sqrMagnitude > 0.035f)
                    {
                        bMarblesSettled = false;
                    }
                }
            }

            if (bMarblesSettled)
            {
                break;
            }

            if (timeWaited > 30)
            {
                Debug.LogError("Detected a likely softlock and moved on to the next turn state");
                break;
            }

            yield return new WaitForSeconds(2.0f);
            timeWaited += 2;
        }

        yield return new WaitForSeconds(1.5f);

        CleanupMarbles();

        foreach (var marble in MarblesList)
        {
            yield return new WaitForSeconds(marble.CastSettleAbility());
        }

        bAreMarblesMoving = false;
        IncremetTurnState();
    }

    [SerializeField]
    private ScoringZoneManager scoringZoneManager;
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
    private bool bInSuddenDeath = false;
    private int numPlayerTurns = 0;

    [SerializeField] private AudioInfo GainPoints;
    [SerializeField] private AudioInfo LosePoints;
    [SerializeField] private AudioInfo SuddenDeath;
    
    [SerializeField]
    public Color playerColor;
    [SerializeField]
    public Color enemyColor;
    public int NumPlayerTurns
    {
        get { return numPlayerTurns; }
    }
    private void Awake()
    {
        if (Instance != null && Instance != this)
            Destroy(this.gameObject);
        else
        {
            Instance = this;
        }
    }
    private void Start()
    {
        Time.timeScale = 2.0f;
        
        if (!scoringZoneManager)
        {
            Debug.LogError("Scoring Zone Reference is null (GameManager)");
        }

        GameObject MapManager = GameObject.Find("MapManager");
        if (MapManager)
        {
            NodeManager NodeManager = MapManager.GetComponent<NodeManager>();
            if (NodeManager)
            {
                LevelDataSO LevelData = NodeManager.GetLevelData();
        
                if (PlayerManager)
                {
                    PlayerManager.InitializePlayerDeck();
                }

                if (EnemyManager)
                {
                    scoringZoneManager.SetArena(LevelData.GetArena());
                    EnemyManager.InitializeLevelData(LevelData.GetAggressionLevel(), LevelData.GetEnemyDifficulty());
                    ForceUpdateEvents(TurnState.EnemyTurn);
                }
            }
        }
    }

    private void OnEnable()
    {
        DeckEvents.OnAddNewMarbleToDeck += OnMarbleAddedToDeck;
        MarbleEvents.OnMarbleLaunched += BeginWaitForMarblesToSettle;
        MarbleEvents.OnMarbleSpawned += RegisterMarble;
    }

    private void OnDisable()
    {
        DeckEvents.OnAddNewMarbleToDeck -= OnMarbleAddedToDeck;
        MarbleEvents.OnMarbleLaunched -= BeginWaitForMarblesToSettle;
        MarbleEvents.OnMarbleSpawned -= RegisterMarble;
    }

    // removes all marbles not within scoring zone
    public void CleanupMarbles()
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
    
    // removes all marbles
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
        scoringZoneManager.ClearMarbleStates();
    }

    private IEnumerator MatchEnded()
    {
        TurnStateEvents.MatchResult result = playerScore > enemyScore ? TurnStateEvents.MatchResult.PlayerWin : TurnStateEvents.MatchResult
            .EnemyWin;

        AnalyticsManager.SendMetric("round_result", new AnalyticsManager.IntMetric(
            playerScore - enemyScore
        ));

        TurnStateEvents.OnMatchEnded(result);
        
        yield return new WaitForSeconds(8.0f);
        ClearMarbles();

        OverrideTurnState(result == TurnStateEvents.MatchResult.PlayerWin ? TurnState.CardSelect : TurnState.GameOver);
    }

    // pulls up marble select UI
    private void GoToCardSelect()
    {
        DeckEvents.SelectNewMarbleToAdd(DeckManager.GenerateNewMarbles());
    }
    
    public void RestartGame()
    {
        ClearMarbles();
        playerScore = 0;
        enemyScore = 0;
        PlayerManager.InitializePlayerDeck();
        EnemyManager.InitializeEnemyDeck();
        ForceUpdateEvents(TurnState.EnemyTurn);
    }

    // debug force turn state
    public void ForceUpdateEvents(TurnState turnState)
    {
        OverrideTurnState(turnState);
        MarbleEvents.OnScoreChanged(MarbleTeam.Player);
        MarbleEvents.OnScoreChanged(MarbleTeam.Enemy);
    }

    // triggers on marble added and moves to level select screen
    private void OnMarbleAddedToDeck(MarbleData data)
    {
        SceneManagerScript.Instance.loadSceneByIndex(2);
    }
}
