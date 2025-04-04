using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TurnUI : MonoBehaviour
{
    [SerializeField]
    private Image PlayerTurnArrow;
    [SerializeField]
    private Image EnemyTurnArrow;
    [SerializeField]
    private TextMeshProUGUI TurnText;
    [SerializeField]
    private float HideTime = 2.0f;
    private Color color;
    private Coroutine timerCoroutine;
    private void OnEnable()
    {
        TurnStateEvents.OnTurnProgress += UpdateTurnPanel;
    }
    private void OnDisable()
    {
        TurnStateEvents.OnTurnProgress -= UpdateTurnPanel;
    }

    private void Start()
    {
        color = EnemyTurnArrow.color;
        TurnText.color = color;
        TurnText.text = "ENEMY TURN";
        PlayerTurnArrow.enabled = false;
        EnemyTurnArrow.enabled = true;
    }

    private void UpdateTurnPanel(TurnState turn)
    {
        if(timerCoroutine != null)
        {
            StopCoroutine(timerCoroutine);
        }

        switch (turn)
        {
            case TurnState.PlayerTurn:
                color = PlayerTurnArrow.color;
                TurnText.color = color;
                TurnText.text = "PLAYER TURN";
                PlayerTurnArrow.enabled = true;
                EnemyTurnArrow.enabled = false;
                break;
            case TurnState.WaitingOnEnemyTurn:
                color = EnemyTurnArrow.color;
                TurnText.color = color;
                TurnText.text = "WAITING ON ENEMY TURN";
                PlayerTurnArrow.enabled = false;
                EnemyTurnArrow.enabled = true;
                break;
            case TurnState.EnemyTurn:
                color = EnemyTurnArrow.color;
                TurnText.color = color;
                TurnText.text = "ENEMY TURN";
                PlayerTurnArrow.enabled = false;
                EnemyTurnArrow.enabled = true;
                break;
            case TurnState.WaitingOnPlayerTurn:
                color = PlayerTurnArrow.color;
                TurnText.color = color;
                TurnText.text = "WAITING ON PLAYER TURN";
                PlayerTurnArrow.enabled = true;
                EnemyTurnArrow.enabled = false;
                break;
            default:
                // We can just hide this if it isn't a turn 
                TurnText.text = "";
                PlayerTurnArrow.enabled = false;
                EnemyTurnArrow.enabled = false;
                break;
        }
        timerCoroutine = StartCoroutine(TimeUntilHide(HideTime));
    }

    private IEnumerator TimeUntilHide(float Duration)
    {
        yield return new WaitForSecondsRealtime(Duration);

        TurnText.text = "";
        PlayerTurnArrow.enabled = false;
        EnemyTurnArrow.enabled = false;
    }
}
