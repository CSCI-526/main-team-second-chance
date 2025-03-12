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

    private Color color;

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
        color = PlayerTurnArrow.color;
        TurnText.color = color;
        TurnText.text = "PLAYER TURN";
        PlayerTurnArrow.enabled = true;
        EnemyTurnArrow.enabled = false;
    }

    private void UpdateTurnPanel(TurnState turn)
    {
        switch ((int)turn)
        {
            case 0:
                color = PlayerTurnArrow.color;
                TurnText.color = color;
                TurnText.text = "PLAYER TURN";
                PlayerTurnArrow.enabled = true;
                EnemyTurnArrow.enabled = false;
                break;
            case 1:
                color = EnemyTurnArrow.color;
                TurnText.color = color;
                TurnText.text = "WAITING ON ENEMY TURN";
                PlayerTurnArrow.enabled = false;
                EnemyTurnArrow.enabled = true;
                break;
            case 2:
                color = EnemyTurnArrow.color;
                TurnText.color = color;
                TurnText.text = "ENEMY TURN";
                PlayerTurnArrow.enabled = false;
                EnemyTurnArrow.enabled = true;
                break;
            case 3:
                color = PlayerTurnArrow.color;
                TurnText.color = color;
                TurnText.text = "WAITING ON PLAYER TURN";
                PlayerTurnArrow.enabled = true;
                EnemyTurnArrow.enabled = false;
                break;
            default:
                TurnText.text = "DEFAULT TURN STATE";
                PlayerTurnArrow.enabled = true;
                EnemyTurnArrow.enabled = true;
                break;
        }
    }

}
