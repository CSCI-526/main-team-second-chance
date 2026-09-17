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
    
    public int GetTurnCount()
    {
        return numPlayerTurns;
    }

    public int GetGameLengthInTurns()
    {
        return gameLength;
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

    public ColorInfo GetColorInfo()
    {
        return colorInfo;
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

    private void IncrementTurnState()
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
        if (turnState == TurnState.EnemyTurn)
        {
            numPlayerTurns++;
            if (HasGameEnded() ||
                bInSuddenDeath)
            {
                if (enemyScore == playerScore)
                {
                    if (!bInSuddenDeath)
                    {
                        bInSuddenDeath = true;
                        PlayerManager.InitializePlayerDeck();
                        SuddenDeathRoutine().OnComplete(() => {
                            CleanupMarbles();
                            TurnStateEvents.OnTurnProgressed(turnState); });
                        // we'll notify the next turn from the scoring cirlce because I hate code quality :))
                        // now we notify from the completion delegate of shrinking the scoring zones
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
                    OverrideTurnState(TurnState.MatchEnd);
                    return;
                }
            }
        }

        Debug.Log(turnState);
        TurnStateEvents.OnTurnProgressed(turnState);
    }
    
    private void OnTurnProgress(TurnState turn)
    {
        // tutorial stuff (move elsewhere later)
        if (turnState == TurnState.PlayerTurn)
        {
            if (TutorialManager.Instance.ShouldDisplayAnymore)
            {
                TutorialEvents.DoTryDisplayTutorialItem(TutorialManager.Instance.CurrentTutorialPhase);
            }
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

        if (turnState == TurnState.WaitingOnEnemyTurn || turnState == TurnState.WaitingOnPlayerTurn)
        {
            SettleAfterRoundEnd();
        }
    }
    
    private void OnEndTurnPress(TurnState turnOwner)
    {
        if (turnState == turnOwner && !_advanceToNextTurn)
        {
            if (bAreMarblesMoving)
            {
                _advanceToNextTurn = true;
            }
            else
            {
                IncrementTurnState();
            }
        }
    }

    private bool HasGameEnded()
    {
        if (UseCombatSystem)
        {
            return _enemyHealth <= 0 || _playerHealth <= 0;
        }
        return numPlayerTurns > gameLength;
        //return PlayerManager.GetPlayerDeck().GetNumMarblesUsed() + 1 > PlayerManager.GetPlayerDeck().GetDeckSize();
    }

    Sequence SuddenDeathRoutine()
    {
        TurnStateEvents.DoSuddenDeath();
        AudioManager.TriggerSound(SuddenDeath, transform.position);

        float t = 1.0f;
        Sequence shrinkSequence = DOTween.Sequence();
        shrinkSequence.Append(
        DOTween.To(() => t, x =>
        {
            t = x;
            scoringZoneManager.SetScoringCircleScales(t);
        }, 0.0f, 2.0f * Time.timeScale * turnSpeed));
        shrinkSequence.AppendInterval(2.0f * Time.timeScale * turnSpeed);
        return shrinkSequence;
    }
    
    public void UpdateEntityScore(MarbleTeam Team, int points, bool bIsInScoreZone)
    {
        if (Team == MarbleTeam.Player)
        {
            playerScore += bIsInScoreZone ? points : -points;
        }
        else
        {
            enemyScore += bIsInScoreZone ? points : -points;
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

    private void SettleAfterMarbleLaunch()
    {
        StartCoroutine(WaitForSettleAfterLaunch());
    }

    private IEnumerator WaitForSettleAfterLaunch()
    {
        bAreMarblesMoving = true;
        yield return new WaitForSeconds(0.5f * Time.timeScale * turnSpeed);
        
        yield return StartCoroutine(WaitForMarblesToSettle());
        
        foreach (var marble in MarblesList)
        {
            Sequence settleSequence = marble.CastSettleAbility();
            if (settleSequence != null)
            {
                MarbleEvents.OnMarbleAbilityCasted(marble);
                yield return settleSequence.WaitForCompletion();
                yield return StartCoroutine(WaitForMarblesToSettle());
            }
        }
        
        bAreMarblesMoving = false;
        if (OneMarblePerTurn)
        {
            IncrementTurnState();
        }
        TurnStateEvents.OnMarblesSettled(turnState);
        if (_advanceToNextTurn)
        {
            _advanceToNextTurn = false;
            IncrementTurnState();
        }
    }

    private void SettleAfterRoundEnd()
    {
        StartCoroutine(WaitForSettleAfterRoundEnd());
    }

    private IEnumerator WaitForSettleAfterRoundEnd()
    {
        bAreMarblesMoving = true;
        yield return new WaitForSeconds(0.5f * Time.timeScale * turnSpeed);
        
        yield return StartCoroutine(WaitForMarblesToSettle());
        
        foreach (var marble in MarblesList)
        {
            Sequence roundEndSequence = marble.CastRoundEndAbility();
            if (roundEndSequence != null)
            {
                MarbleEvents.OnMarbleAbilityCasted(marble);
                yield return roundEndSequence.WaitForCompletion();
                yield return StartCoroutine(WaitForMarblesToSettle());
            }
        }
        
        bAreMarblesMoving = false;
        if (TurnState.WaitingOnEnemyTurn == turnState)
        {
            _playerHealth -= enemyScore;
            _enemyHealth -= playerScore;
            TurnStateEvents.OnHealthUpdated(_playerHealth, _playerMaxHealth,MarbleTeam.Player);
            TurnStateEvents.OnHealthUpdated(_enemyHealth, _enemyMaxHealth,MarbleTeam.Enemy);
            NodeManager.Instance.SetPlayerHealth(_playerHealth);
        }
        TurnStateEvents.OnMarblesSettled(turnState);
        IncrementTurnState();
    }

    private IEnumerator WaitForMarblesToSettle()
    {
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

            if (timeWaited > 10)
            {
                Debug.LogError("Detected a likely softlock and moved on to the next turn state");
                break;
            }

            yield return new WaitForSeconds(1.0f * Time.timeScale);
            timeWaited += 1;
        }

        if (timeWaited > 0)
        {
            yield return new WaitForSeconds(0.75f * Time.timeScale * turnSpeed);
        }

        CleanupMarbles();
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

    [SerializeField] private ColorInfo colorInfo;
    [SerializeField] private int gameLength = 3;
    [SerializeField] private float gameSpeed = 2.0f;
    [SerializeField] private float turnSpeed = 1.5f;

    private List<Marble> MarblesList = new List<Marble>();
    private List<Marble> MarblesToDelete = new List<Marble>();
    private int playerScore = 0;
    private int enemyScore = 0;
    private int _playerHealth = 30;
    private int _playerMaxHealth = 30;
    private int _enemyHealth = 20;
    private int _enemyMaxHealth = 20;
    private bool bAreMarblesMoving = false;
    private bool bInSuddenDeath = false;
    private int numPlayerTurns = 0;
    private bool _advanceToNextTurn = false;

    [SerializeField] private AudioInfo GainPoints;
    [SerializeField] private AudioInfo LosePoints;
    [SerializeField] private AudioInfo SuddenDeath;

    public static bool DrawnNewHandEachTurn;
    public static bool UseEnergy;
    public static bool OneMarblePerTurn;
    public static bool UseCombatSystem;

    private void Awake()
    {
        if (Instance != null && Instance != this)
            Destroy(this.gameObject);
        else
        {
            Instance = this;
            
            UseEnergy = PlayerPrefs.GetInt("UseEnergy") == 1;
            OneMarblePerTurn = PlayerPrefs.GetInt("OneMarble") == 1;
            DrawnNewHandEachTurn = PlayerPrefs.GetInt("DrawNewHand") == 1;
            UseCombatSystem = PlayerPrefs.GetInt("UseCombat") == 1;
        }
    }
    private void Start()
    {
        Time.timeScale = gameSpeed;
        
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
                    _playerHealth = PlayerManager.GetStoredPlayerHealth();
                    if (_playerHealth < 0)
                    {
                        _playerHealth = _playerMaxHealth;
                        NodeManager.Instance.SetPlayerHealth(_playerHealth);
                    }
                    
                    TurnStateEvents.OnHealthUpdated(_playerHealth, _playerMaxHealth, MarbleTeam.Player);
                }

                if (EnemyManager)
                {
                    _enemyMaxHealth = _enemyHealth = LevelData.GetEnemyHealth();
                    TurnStateEvents.OnHealthUpdated(_enemyHealth,_enemyMaxHealth, MarbleTeam.Enemy);
                    scoringZoneManager.SetArena(LevelData.GetArena());
                    EnemyManager.InitializeLevelData(LevelData.GetAggressionLevel(), LevelData.GetEnemyDifficulty());
                    ForceUpdateEvents(TurnState.WaitingOnEnemyTurn);
                }
            }
        }
    }

    private void OnEnable()
    {
        DeckEvents.OnAddNewMarbleToDeck += OnMarbleAddedToDeck;
        MarbleEvents.OnMarbleLaunched += SettleAfterMarbleLaunch;
        MarbleEvents.OnMarbleSpawned += RegisterMarble;
        TurnStateEvents.OnEndTurnPress += OnEndTurnPress;
        TurnStateEvents.OnTurnProgress += OnTurnProgress;
    }

    private void OnDisable()
    {
        DeckEvents.OnAddNewMarbleToDeck -= OnMarbleAddedToDeck;
        MarbleEvents.OnMarbleLaunched -= SettleAfterMarbleLaunch;
        MarbleEvents.OnMarbleSpawned -= RegisterMarble;
        TurnStateEvents.OnEndTurnPress -= OnEndTurnPress;
        TurnStateEvents.OnTurnProgress -= OnTurnProgress;
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
        if (UseCombatSystem)
        {
            result = _playerHealth > 0
                ? TurnStateEvents.MatchResult.PlayerWin
                : TurnStateEvents.MatchResult.EnemyWin;
        }

        AnalyticsManager.SendMetric("round_result", new AnalyticsManager.IntMetric(
            playerScore - enemyScore
        ));

        TurnStateEvents.OnMatchEnded(result);
        
        yield return new WaitForSeconds(4.0f * Time.timeScale * turnSpeed);
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
