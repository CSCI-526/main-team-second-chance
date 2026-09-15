using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DebugSettingsUI : MonoBehaviour
{
    [SerializeField] private Toggle energyToggle;
    [SerializeField] private Toggle marbleToggle;
    [SerializeField] private Toggle combatToggle;
    [SerializeField] private Toggle drawHandToggle;
    public void Awake()
    {
        energyToggle.isOn = PlayerPrefs.GetInt("UseEnergy") == 1;
        marbleToggle.isOn = PlayerPrefs.GetInt("OneMarble") == 1;
        drawHandToggle.isOn = PlayerPrefs.GetInt("DrawNewHand") == 1;
        combatToggle.isOn = PlayerPrefs.GetInt("UseCombat") == 1;
    }

    public void SetEnergyUse(bool useEnergy)
    {
        PlayerPrefs.SetInt("UseEnergy",useEnergy ? 1 : 0);
    }

    public void SetOneMarble(bool oneMarble)
    {
        PlayerPrefs.SetInt("OneMarble",oneMarble ? 1 : 0);
    }

    public void SetDrawNewHand(bool newHand)
    {
        PlayerPrefs.SetInt("DrawNewHand",newHand ? 1 : 0);
    }

    public void SetUseCombatSystem(bool useCombat)
    {
        PlayerPrefs.SetInt("UseCombat",useCombat ? 1 : 0);
    }
}
