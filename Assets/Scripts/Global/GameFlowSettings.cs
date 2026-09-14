using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "GameFlowSettings", menuName = "ScriptableObjects/GameFlowSettings")]
public class GameFlowSettings : ScriptableObject
{
    public bool DrawnNewHandEachTurn = true; // implemented
    public bool UseEnergy = true; // implemented
    public bool OneMarblePerTurn = false; // implemented
    public bool UseCombatSystem = false; // not implemented
}
