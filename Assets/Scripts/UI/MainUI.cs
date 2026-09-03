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
    [SerializeField] private TextMeshProUGUI PlayerDeckCount;
    [SerializeField] private TextMeshProUGUI PlayerScore;
    [SerializeField] private TextMeshProUGUI EnemyScore;
    [SerializeField] private TextMeshProUGUI MatchVictoryText;
    [SerializeField] private Transform HandStartingPoint;
    [SerializeField] private GameObject CardPrefab;
    [SerializeField] private float ScoreBounceHeight = 30.0f;
    [SerializeField] private float ScoreBounceDuration = 0.3f;
    private List<GameObject> Cards = new List<GameObject>();
    
    [SerializeField] private AudioInfo winRoundSound;
    [SerializeField] private AudioInfo loseRoundSound;

    private RectOffset handDefaultPadding;

    private void OnEnable()
    {
        MarbleEvents.OnScoreChange += UpdateScore;
        TurnStateEvents.OnMatchEnd += UpdateRoundsWon;

        TurnStateEvents.OnTurnProgress += OnTurnStateProgress;

        DeckEvents.OnDeckGenerated += UpdateDeckCount;
        DeckEvents.OnMarbleUsed += UpdateDeckCount;
        DeckEvents.OnHandUpdated += UpdateHand;
    }

    private void OnDisable()
    {
        MarbleEvents.OnScoreChange -= UpdateScore;
        TurnStateEvents.OnMatchEnd -= UpdateRoundsWon;
        
        TurnStateEvents.OnTurnProgress -= OnTurnStateProgress;

        DeckEvents.OnDeckGenerated -= UpdateDeckCount;
        DeckEvents.OnMarbleUsed -= UpdateDeckCount;
        DeckEvents.OnHandUpdated -= UpdateHand;
    }

    private void UpdateRoundsWon(TurnStateEvents.MatchResult matchResult)
    {
        if (matchResult == TurnStateEvents.MatchResult.PlayerWin)
        {
            MatchVictoryText.color = GameManager.Instance.playerColor;
            MatchVictoryText.text = "VICTORY!";
            AudioManager.TriggerSound(winRoundSound,Vector3.zero);
        }
        else
        {
            MatchVictoryText.color = GameManager.Instance.enemyColor;
            MatchVictoryText.text = "DEFEAT!";
            AudioManager.TriggerSound(loseRoundSound,Vector3.zero);
        }
        
        MatchVictoryText.gameObject.SetActive(true);
        StartCoroutine(BounceScoreGO(MatchVictoryText.rectTransform));
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
            EnemyScore.text =
                $"{NodeManager.Instance.GetLevelData().GetEnemyName()}\n<color=#FF0000>🔴</color> {GameManager.Instance.GetEnemyScore()}";
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
            Debug.LogError(
                "MainUI.UpdateHand(): The marbledata list being sent in is null. This probably shouldn't happen");
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
            Debug.LogError(
                "MainUI.UpdateHand(): NumCardsToRep is larger than the actual number of spawn points. This shouldn't happen \n" +
                Cards.Count + " <" + dataList.Count);
            return;
        }

        for (int i = 0; i < dataList.Count; i++)
        {
            // Activate a corresponding UI Prefab
            Card card = Cards[i].GetComponent<Card>();
            MarbleData marbleData = dataList[i];
            if (!card || !marbleData)
            {
                Debug.LogWarning(
                    "MainUI.UpdateHand(): Card.cs is not attached to the card prefab. Or input data has is incorrect This shouldn't happen");
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
        if (turnState == TurnState.CardSelect || turnState == TurnState.GameOver)
        {
            MatchVictoryText.gameObject.SetActive(false);
        }
    }
}
