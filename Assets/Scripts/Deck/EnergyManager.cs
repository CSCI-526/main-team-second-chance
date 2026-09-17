using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnergyManager : MonoBehaviour
{
    private int _currentEnergy = 3;
    [SerializeField]
    private int startingEnergy = 3;
    
    private void OnEnable()
    {
        EnergyEvents.OnEnergyCheck += EnergyCheck;
        EnergyEvents.OnEnergySpend += EnergySpend;
        TurnStateEvents.OnTurnProgress += OnTurnProgress;
    }

    private void OnDisable()
    {
        EnergyEvents.OnEnergyCheck -= EnergyCheck;
        EnergyEvents.OnEnergySpend -= EnergySpend;
        TurnStateEvents.OnTurnProgress -= OnTurnProgress;
    }

    private void OnTurnProgress(TurnState turn)
    {
        if (turn == TurnState.PlayerTurn)
        {
            _currentEnergy = startingEnergy;
            EnergyEvents.OnEnergyUpdated(_currentEnergy);
        }
    }

    private bool EnergySpend(int energyToSpend)
    {
        if (energyToSpend <= _currentEnergy)
        {
            _currentEnergy -= energyToSpend;
            Debug.Log($"Spent {energyToSpend}, {_currentEnergy} energy remaining");
            EnergyEvents.OnEnergyUpdated(_currentEnergy);
            return true;
        }

        return false;
    }

    private bool EnergyCheck(int energyToSpend)
    {
        return energyToSpend <= _currentEnergy;
    }
}
