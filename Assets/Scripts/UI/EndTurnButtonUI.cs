using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndTurnButtonUI : MonoBehaviour
{
    void Start()
    {
        if (GameManager.OneMarblePerTurn)
        {
            gameObject.SetActive(false);
        }
    }

    public void EndTurn()
    {
        TurnStateEvents.OnEndTurnPressed(TurnState.PlayerTurn);
    }
}
