using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.GraphicsBuffer;

public class MainUI : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI PlayerDeckCount;
    [SerializeField]
    private TextMeshProUGUI PlayerScore;
    [SerializeField]
    private TextMeshProUGUI PlayerRoundsWon;
    [SerializeField]
    private TextMeshProUGUI EnemyScore;
    [SerializeField]
    private TextMeshProUGUI MatchVictoryText;
    [SerializeField]
    private Transform HandStartingPoint;
    [SerializeField]
    private GameObject CardPrefab;
    [SerializeField]
    private Image[] BestOfIndicators;
    [SerializeField]
    private VerticalLayoutGroup bestOfIndicatorLayoutGroup;
    [SerializeField]
    private float ScoreBounceHeight = 30.0f;
    [SerializeField]
    private float ScoreBounceDuration = 0.3f;
    private List<GameObject> Cards = new List<GameObject>();
    private int previouslyWonRounds = 0;
    private int playedRounds = 0;
    private bool shouldShowRoundStart = true;

    private RectOffset handDefaultPadding;
    private RectOffset bestOfIndicatorDefaultPadding;
    private int prevCanSelectMarble = -1;
    private bool bisAnimatingBestOfIndicator = false;

    private const int BEST_OF_INDICATOR_PADDING_TOP_OFFSET = 160;

    private void OnEnable()
    {
        MarbleEvents.OnScoreChange += UpdateScore;
        MarbleEvents.OnRoundsWonChanged += UpdateRoundsWon;

        TurnStateEvents.OnGameOver += ResetColors;
        TurnStateEvents.OnTurnProgress += OnTurnStateProgress;

        DeckEvents.OnDeckGenerated += UpdateDeckCount;
        DeckEvents.OnMarbleUsed += UpdateDeckCount;
        DeckEvents.OnHandUpdated += UpdateHand;

        bestOfIndicatorDefaultPadding = bestOfIndicatorLayoutGroup.padding;
        bisAnimatingBestOfIndicator = true;
    }
    private void OnDisable()
    {
        MarbleEvents.OnScoreChange -= UpdateScore;
        MarbleEvents.OnRoundsWonChanged -= UpdateRoundsWon;

        TurnStateEvents.OnGameOver -= ResetColors;
        TurnStateEvents.OnTurnProgress -= OnTurnStateProgress;

        DeckEvents.OnDeckGenerated -= UpdateDeckCount;
        DeckEvents.OnMarbleUsed -= UpdateDeckCount;
        DeckEvents.OnHandUpdated -= UpdateHand;
    }

    private void Update()
    {
        int canSelectMarble =
            GameManager.Instance.GetTurnState() == TurnState.PlayerTurn &&
            (!GameManager.Instance.PlayerHasSelectedMarble() ||
             GameManager.Instance.GetPlayerManager().GetPlayerDeck().bIsHoveringDeck)
            ? 1 : 0;
        if (canSelectMarble != prevCanSelectMarble)
        {
            bisAnimatingBestOfIndicator = true;
            prevCanSelectMarble = canSelectMarble;
        }

        AnimateBestOfIndicator();
    }

    private void AnimateBestOfIndicator()
    {
        if (bisAnimatingBestOfIndicator)
        {
            int newTopPadding;
            if (prevCanSelectMarble == 0)
            {
                newTopPadding = (int)Mathf.MoveTowards(
                    bestOfIndicatorLayoutGroup.padding.top,
                    bestOfIndicatorDefaultPadding.top - BEST_OF_INDICATOR_PADDING_TOP_OFFSET,
                    BEST_OF_INDICATOR_PADDING_TOP_OFFSET * 2f * Time.deltaTime);

                if (newTopPadding == bestOfIndicatorDefaultPadding.top - BEST_OF_INDICATOR_PADDING_TOP_OFFSET)
                {
                    bisAnimatingBestOfIndicator = false;
                }
            }
            else
            {
                newTopPadding = (int)Mathf.MoveTowards(
                    bestOfIndicatorLayoutGroup.padding.top,
                    bestOfIndicatorDefaultPadding.top,
                    BEST_OF_INDICATOR_PADDING_TOP_OFFSET * 2f * Time.deltaTime);

                if (newTopPadding == bestOfIndicatorDefaultPadding.top)
                {
                    bisAnimatingBestOfIndicator = false;
                }
            }

            bestOfIndicatorLayoutGroup.padding = new RectOffset(
                bestOfIndicatorDefaultPadding.left,
                bestOfIndicatorDefaultPadding.right,
                newTopPadding,
                bestOfIndicatorDefaultPadding.bottom
            );
        }
    }

    private void ResetColors()
    {
        foreach (Image i in BestOfIndicators)
        {
            i.color = Color.black;
        }

        PlayerRoundsWon.text = "";
        previouslyWonRounds = 0;
        playedRounds = 0;
    }
    private void UpdateRoundsWon(int RoundNum, int RoundsWon)
    {
        //PlayerRoundsWon.text = $"{RoundsWon} / 3 Matches Won";

        // Round Num will always sum to 3 
        // --> 0 index round num to indicate which index we are modifying
        // --> save previous rounds won value. if it is less than incoming, means we won a new round, otherwise we lost
        if (previouslyWonRounds < RoundsWon)
        {
            BestOfIndicators[RoundNum - 1].color = GameManager.Instance.playerColor;
            MatchVictoryText.color = GameManager.Instance.playerColor;
            MatchVictoryText.text = "GAME WIN!";
        }
        else
        {
            BestOfIndicators[RoundNum - 1].color = GameManager.Instance.enemyColor;
            MatchVictoryText.color = GameManager.Instance.enemyColor;
            MatchVictoryText.text = "GAME DEFEAT!";
        }
        previouslyWonRounds = RoundsWon;
        playedRounds = RoundNum;
        
        shouldShowRoundStart = true;
        MatchVictoryText.gameObject.SetActive(true);
        StartCoroutine(ProgressGameWinsText());
    }
    private void UpdateScore(MarbleTeam Team)
    {
        if (Team == MarbleTeam.Player)
        {
            PlayerScore.text = $"You\n<color=#49A9DB>🔴</color> {GameManager.Instance.GetPlayerScore()}";
            StartCoroutine(BounceScoreGO(PlayerScore.rectTransform));
        }
        else
        {
            EnemyScore.text = $"{NodeManager.Instance.GetLevelData().GetEnemyName()}\n<color=#FF0000>🔴</color> {GameManager.Instance.GetEnemyScore()}";
            StartCoroutine(BounceScoreGO(EnemyScore.rectTransform));
        }
    }
    private IEnumerator BounceScoreGO(RectTransform scoreGO)
    {
        Vector3 startingPosition = scoreGO.anchoredPosition;
        Vector3 upPos = startingPosition + Vector3.up * ScoreBounceHeight;

        float time = 0f;

        // Move up
        while (time < ScoreBounceDuration)
        {
            float t = time / ScoreBounceDuration;
            scoreGO.anchoredPosition = Vector3.Lerp(startingPosition, upPos, t);
            time += Time.deltaTime;
            yield return null;
        }

        scoreGO.anchoredPosition = upPos;

        time = 0f;

        // Move back down
        while (time < ScoreBounceDuration)
        {
            float t = time / ScoreBounceDuration;
            scoreGO.anchoredPosition = Vector3.Lerp(upPos, startingPosition, t);
            time += Time.deltaTime;
            yield return null;
        }

        scoreGO.anchoredPosition = startingPosition;
    }
    private void UpdateDeckCount(MarbleTeam Team, int Count)
    {
        if (Team != MarbleTeam.Player)
        {
            return;
        }
        if (Count == 0)
        {
            PlayerDeckCount.color = Color.red;
        }
        else
        {
            PlayerDeckCount.color = Color.white;
        }
        PlayerDeckCount.text = $"{Count}";

    }
    private void UpdateHand(MarbleTeam Team, List<MarbleData> dataList)
    {
        if (Team != MarbleTeam.Player)
        {
            return;
        }
        if (dataList == null)
        {
            Debug.LogError("MainUI.UpdateHand(): The marbledata list being sent in is null. This probably shouldn't happen");
            return;
        }

        if (Cards.Count == 0)
        {
            for (int i = 0; i < GameManager.Instance.GetPlayerManager().GetPlayerDeck().GetMaxHandSize(); i++)
            {
                GameObject prefab = Instantiate(CardPrefab, HandStartingPoint);
                prefab.transform.position = Vector3.zero;
                prefab.SetActive(false);
                Cards.Add(prefab);

                HandManager.Instance.AddCard(prefab.GetComponent<Card>());
            }
        }
        // if the number of cards to rep is greater than the hand size
        if (Cards.Count < dataList.Count)
        {
            Debug.LogError("MainUI.UpdateHand(): NumCardsToRep is larger than the actual number of spawn points. This shouldn't happen \n" + Cards.Count + " <" + dataList.Count);
            return;
        }
        for (int i = 0; i < dataList.Count; i++)
        {
            // Activate a corresponding UI Prefab
            Card card = Cards[i].GetComponent<Card>();
            MarbleData marbleData = dataList[i];
            if (!card || !marbleData)
            {
                Debug.LogWarning("MainUI.UpdateHand(): Card.cs is not attached to the card prefab. Or input data has is incorrect This shouldn't happen");
                return;
            }
            card.UpdateInformation(marbleData, false);
            card.SetHandIndex(i);
            Cards[i].SetActive(true);
        }
        // cleanup the rest of the available cards if there are more than the hand size ie if 
        if (dataList.Count < Cards.Count)
        {
            for (int i = dataList.Count; i < Cards.Count; i++)
            {
                if (Cards[i].activeInHierarchy)
                {
                    Cards[i].SetActive(false);
                }
            }
        }
    }

    private void OnTurnStateProgress(TurnState turnState)
    {
        if (turnState == TurnState.EnemyTurn && shouldShowRoundStart)
        {
            shouldShowRoundStart = false;
            StartCoroutine(ShowRoundStartText());
        }
        else if (turnState == TurnState.CardSelect)
        {
            MatchVictoryText.gameObject.SetActive(false);
        }
    }

    private IEnumerator ProgressGameWinsText()
    {
        StartCoroutine(BounceScoreGO(MatchVictoryText.rectTransform));
        yield return new WaitForSeconds(4.0f);
        StartCoroutine(BounceScoreGO(MatchVictoryText.rectTransform));
        if (previouslyWonRounds == 2)
        {
            MatchVictoryText.text = "RIVAL VANQUISHED!";
        }
        else
        {
            MatchVictoryText.text = $"{previouslyWonRounds} - {playedRounds - previouslyWonRounds}";
        }
        yield return new WaitForSeconds(4.0f);
        StartCoroutine(BounceScoreGO(MatchVictoryText.rectTransform));
    }

    private IEnumerator ShowRoundStartText()
    {
        MatchVictoryText.text = "GAME " + $"{playedRounds + 1}" + " START";
        MatchVictoryText.color = Color.white;
        MatchVictoryText.gameObject.SetActive(true);
        StartCoroutine(BounceScoreGO(MatchVictoryText.rectTransform));
        yield return new WaitForSeconds(4.0f);
        MatchVictoryText.gameObject.SetActive(false);
    }
}
