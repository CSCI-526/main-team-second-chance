using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DebugSettingsUI : MonoBehaviour
{
    [SerializeField] private GameFlowSettings gameFlowSettings;
    [SerializeField] private Toggle energyToggle;
    [SerializeField] private Toggle marbleToggle;
    [SerializeField] private Toggle combatToggle;
    [SerializeField] private Toggle drawHandToggle;
    public void Awake()
    {
        energyToggle.isOn = gameFlowSettings.UseEnergy;
        marbleToggle.isOn = gameFlowSettings.OneMarblePerTurn;
        drawHandToggle.isOn = gameFlowSettings.DrawnNewHandEachTurn;
        combatToggle.isOn = gameFlowSettings.UseCombatSystem;
    }

    public void SetEnergyUse(bool useEnergy)
    {
        gameFlowSettings.UseEnergy = useEnergy;
    }

    public void SetOneMarble(bool oneMarble)
    {
        gameFlowSettings.OneMarblePerTurn = oneMarble;
    }

    public void SetDrawNewHand(bool newHand)
    {
        gameFlowSettings.DrawnNewHandEachTurn = newHand;
    }

    public void SetUseCombatSystem(bool useCombat)
    {
        gameFlowSettings.UseCombatSystem = useCombat;
    }
}
