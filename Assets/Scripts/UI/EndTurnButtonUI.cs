using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndTurnButtonUI : MonoBehaviour
{
    public void EndTurn()
    {
        TurnStateEvents.OnEndTurnPressed(TurnState.PlayerTurn);
    }
}
