using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MainUI : MonoBehaviour
{

    [SerializeField]
    private TextMeshProUGUI PlayerDeckCount;
    [SerializeField]
    private TextMeshProUGUI PlayerScore;

    [SerializeField]
    private TextMeshProUGUI EnemyScore;
    private void OnEnable()
    {
        MarbleEvents.OnScoreChange += UpdateScore;
        DeckEvents.OnDeckGenerated += UpdateDeckCount;
        DeckEvents.OnMarbleUsed += UpdateDeckCount;
    }
    private void OnDisable()
    {
        MarbleEvents.OnScoreChange -= UpdateScore;
        DeckEvents.OnDeckGenerated -= UpdateDeckCount;
        DeckEvents.OnMarbleUsed -= UpdateDeckCount;
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
        if (Count == 0)
        {
            PlayerDeckCount.color = Color.red;
        }
        else
        {
            PlayerDeckCount.color = Color.white;
        }
        if (Team == MarbleTeam.Player)
        {

            PlayerDeckCount.text = $"{Count}";
        }
        else
        {
            PlayerDeckCount.text = $"{Count}";
        }
    }
}
