using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

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
    private Transform HandStartingPoint;
    [SerializeField]
    private HorizontalLayoutGroup HandLayoutGroup;
    [SerializeField]
    private CanvasGroup HandLayoutCanvasGroup;
    [SerializeField]
    private GameObject CardPrefab;
    [SerializeField]
    private Image[] BestOfIndicators;
    [SerializeField]
    private VerticalLayoutGroup bestOfIndicatorLayoutGroup;
    private List<GameObject> Cards = new List<GameObject>();
    private int previouslyWonRounds = 0;

    private RectOffset handDefaultPadding;
    private RectOffset bestOfIndicatorDefaultPadding;
    private int prevCanSelectMarble = -1;
    private bool bisAnimatingHand = false;
    private bool bisAnimatingBestOfIndicator = false;

    private const int HAND_PADDING_TOP_OFFSET = 400;
    private const int BEST_OF_INDICATOR_PADDING_TOP_OFFSET = 160;

    private void OnEnable()
    {
        MarbleEvents.OnScoreChange += UpdateScore;
        MarbleEvents.OnRoundsWonChanged += UpdateRoundsWon;

        TurnStateEvents.OnGameOver += ResetColors;

        DeckEvents.OnDeckGenerated += UpdateDeckCount;
        DeckEvents.OnMarbleUsed += UpdateDeckCount;
        DeckEvents.OnHandUpdated += UpdateHand;

        handDefaultPadding = HandLayoutGroup.padding;
        bestOfIndicatorDefaultPadding = bestOfIndicatorLayoutGroup.padding;
        bisAnimatingHand = true;
        bisAnimatingBestOfIndicator = true;
    }
    private void OnDisable()
    {
        MarbleEvents.OnScoreChange -= UpdateScore;
        MarbleEvents.OnRoundsWonChanged -= UpdateRoundsWon;

        TurnStateEvents.OnGameOver -= ResetColors;

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
            bisAnimatingHand = true;
            bisAnimatingBestOfIndicator = true;
            prevCanSelectMarble = canSelectMarble;
        }

        AnimateHand();
        AnimateBestOfIndicator();
    }

    private void AnimateBestOfIndicator() {
        if (bisAnimatingBestOfIndicator) {
            int newTopPadding;
            if (prevCanSelectMarble == 0) {
                newTopPadding = (int)Mathf.MoveTowards(
                    bestOfIndicatorLayoutGroup.padding.top, 
                    bestOfIndicatorDefaultPadding.top - BEST_OF_INDICATOR_PADDING_TOP_OFFSET, 
                    BEST_OF_INDICATOR_PADDING_TOP_OFFSET * 2f * Time.deltaTime);

                if (newTopPadding == bestOfIndicatorDefaultPadding.top - BEST_OF_INDICATOR_PADDING_TOP_OFFSET) {
                    bisAnimatingBestOfIndicator = false;
                }
            }
            else {
                newTopPadding = (int)Mathf.MoveTowards(
                    bestOfIndicatorLayoutGroup.padding.top, 
                    bestOfIndicatorDefaultPadding.top, 
                    BEST_OF_INDICATOR_PADDING_TOP_OFFSET * 2f * Time.deltaTime);

                if (newTopPadding == bestOfIndicatorDefaultPadding.top) {
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

    private void AnimateHand() {
        if (bisAnimatingHand) {
            int newTopPadding;
            if (prevCanSelectMarble == 0) {
                newTopPadding = (int)Mathf.MoveTowards(
                    HandLayoutGroup.padding.top, 
                    handDefaultPadding.top + HAND_PADDING_TOP_OFFSET, 
                    HAND_PADDING_TOP_OFFSET * 2f * Time.deltaTime);

                if (newTopPadding == handDefaultPadding.top + HAND_PADDING_TOP_OFFSET) {
                    bisAnimatingHand = false;
                }
            }
            else {
                newTopPadding = (int)Mathf.MoveTowards(
                    HandLayoutGroup.padding.top, 
                    handDefaultPadding.top, 
                    HAND_PADDING_TOP_OFFSET * 2f * Time.deltaTime);

                if (newTopPadding == handDefaultPadding.top) {
                    bisAnimatingHand = false;
                }
            }

            HandLayoutGroup.padding = new RectOffset(
                handDefaultPadding.left,
                handDefaultPadding.right,
                newTopPadding,
                handDefaultPadding.bottom
            );
        }
    }

    private void ResetColors()
    {
        foreach (Image i in BestOfIndicators)
        {
            i.color = Color.white;
        }

        PlayerRoundsWon.text = "";
        previouslyWonRounds = 0;
    }
    private void UpdateRoundsWon(int RoundNum, int RoundsWon)
    {
        PlayerRoundsWon.text = $"{RoundsWon} / 3 Games Won";

        // Round Num will always sum to 3 
        // --> 0 index round num to indicate which index we are modifying
        // --> save previous rounds won value. if it is less than incoming, means we won a new round, otherwise we lost
        if (previouslyWonRounds < RoundsWon)
        {
            BestOfIndicators[RoundNum - 1].color = Color.green;
        }
        else
        {
            BestOfIndicators[RoundNum - 1].color = Color.red;
        }
        previouslyWonRounds = RoundsWon;
    }
    private void UpdateScore(MarbleTeam Team)
    {
        if (Team == MarbleTeam.Player)
        {
            PlayerScore.text = $"Player Score: {GameManager.Instance.GetPlayerScore()}";
        }
        else
        {
            EnemyScore.text = $"Enemy Score: {GameManager.Instance.GetEnemyScore()}";
        }
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
            card.UpdateInformation(marbleData.MarbleName, marbleData.MarbleDescription);
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
}
