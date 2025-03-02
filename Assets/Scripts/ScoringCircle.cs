using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using TMPro;
using UnityEngine.PlayerLoop;

public class ScoringCircle : MonoBehaviour
{
    private int playerScore = 0;
    private int enemyScore = 0;
    [SerializeField] private TextMeshProUGUI playerText;
    [SerializeField] private TextMeshProUGUI enemyText;

    private void Update()
    {
        UpdateText();
    }
    private void OnTriggerEnter(Collider other)
    {
        Marble marble = other.GetComponent<Marble>();
        if (marble.Team == MarbleTeam.Player)
        {
            playerScore++;
        }
        else
        {
            enemyScore++;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        Marble marble = other.GetComponent<Marble>();
        if (marble.Team == MarbleTeam.Player)
        {
            playerScore--;
        }
        else
        {
            enemyScore--;
        }
    }

    private void UpdateText()
    {
        playerText.text = $"Player Score: {playerScore}";
        enemyText.text = $"Enemy Score: {enemyScore}";
    }
}
