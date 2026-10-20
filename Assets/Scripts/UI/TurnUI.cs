using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
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
    [SerializeField]
    private TextMeshProUGUI RoundText;
    private Color color;
    private Tween hideUI;

    private void OnEnable()
    {
        TurnStateEvents.OnTurnProgress += UpdateTurnPanel;
    }
    private void OnDisable()
    {
        TurnStateEvents.OnTurnProgress -= UpdateTurnPanel;
        hideUI.Kill();
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
        if(hideUI != null)
            hideUI.Kill();

        if (turn == TurnState.EnemyTurn)
        {
            if (!GameManager.UseCombatSystem &&
                GameManager.Instance.GetTurnCount() == GameManager.Instance.GetGameLengthInTurns())
            {
                RoundText.text =  "Final Round";
            }
            else
            {
                RoundText.text =  "Round " + GameManager.Instance.GetTurnCount();
            }

            Sequence jump = RoundText.transform.DOJump(RoundText.transform.position, 30.0f, 1, 1.0f * Time.timeScale);
            jump.AppendInterval(2.0f * Time.timeScale);
            jump.OnComplete(() => { RoundText.enabled = false; });
            RoundText.enabled = true;
        }

        switch (turn)
        {
            case TurnState.PlayerTurn:
                UpdateTurnTextForPlayer();
                TurnText.text = "YOUR TURN";
                PlayerTurnArrow.enabled = true;
                EnemyTurnArrow.enabled = false;
                break;
            
            case TurnState.PlayerEndOfTurn:
                /*
                UpdateTurnTextForEnemy();
                TurnText.text = "WAITING";
                PlayerTurnArrow.enabled = false;
                EnemyTurnArrow.enabled = true;
                */
                break;
            case TurnState.EnemyTurn:
                UpdateTurnTextForEnemy();
                TurnText.text = "ENEMY TURN";
                PlayerTurnArrow.enabled = false;
                EnemyTurnArrow.enabled = true;
                break;
            
            case TurnState.EnemyEndOfTurn:
                /*
                UpdateTurnTextForPlayer();
                TurnText.text = "WAITING";
                PlayerTurnArrow.enabled = true;
                EnemyTurnArrow.enabled = false;
                */
                break;
            default:
                // We can just hide this if it isn't a turn 
                TurnText.text = "";
                PlayerTurnArrow.enabled = false;
                EnemyTurnArrow.enabled = false;
                break;
        }

        hideUI = DOVirtual.DelayedCall(HideTime * Time.timeScale, () =>
        {
            TurnText.text = "";
            PlayerTurnArrow.enabled = false;
            EnemyTurnArrow.enabled = false;
        }, false);
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
}
