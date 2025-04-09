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
        UpdateTurnTextForEnemy();
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
                UpdateTurnTextForPlayer();
                TurnText.text = "YOUR TURN";
                PlayerTurnArrow.enabled = true;
                EnemyTurnArrow.enabled = false;
                break;
            case TurnState.WaitingOnEnemyTurn:
                UpdateTurnTextForEnemy();
                TurnText.text = "WAITING ON ENEMY TURN";
                PlayerTurnArrow.enabled = false;
                EnemyTurnArrow.enabled = true;
                break;
            case TurnState.EnemyTurn:
                UpdateTurnTextForEnemy();
                TurnText.text = "ENEMY TURN";
                PlayerTurnArrow.enabled = false;
                EnemyTurnArrow.enabled = true;
                break;
            case TurnState.WaitingOnPlayerTurn:
                UpdateTurnTextForPlayer();
                TurnText.text = "WAITING ON YOUR TURN";
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

    private void UpdateTurnTextForPlayer() {
        TurnText.alignment = TextAlignmentOptions.Left;

        Vector3 newTurnTextPosition = TurnText.rectTransform.anchoredPosition;
        newTurnTextPosition.x = -220;
        TurnText.rectTransform.anchoredPosition = newTurnTextPosition;

        color = PlayerTurnArrow.color;
        TurnText.color = color;
    }

    private void UpdateTurnTextForEnemy() {
        TurnText.alignment = TextAlignmentOptions.Right;

        Vector3 newTurnTextPosition = TurnText.rectTransform.anchoredPosition;
        newTurnTextPosition.x = 220;
        TurnText.rectTransform.anchoredPosition = newTurnTextPosition;

        color = EnemyTurnArrow.color;
        TurnText.color = color;
    }

    private IEnumerator TimeUntilHide(float Duration)
    {
        yield return new WaitForSecondsRealtime(Duration);

        TurnText.text = "";
        PlayerTurnArrow.enabled = false;
        EnemyTurnArrow.enabled = false;
    }
}
