using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DebugSettingsUI : MonoBehaviour
{
    [SerializeField] private Toggle energyToggle;
    [SerializeField] private Toggle marbleToggle;
    [SerializeField] private Toggle combatToggle;
    [SerializeField] private Toggle drawHandToggle;
    [SerializeField] private TMP_Text versionText;
    [SerializeField] private GameObject textCanvas;
    public void Awake()
    {
        if (PlayerPrefs.GetInt("EverBeenOpened") == 0)
        {
            PlayerPrefs.SetInt("UseEnergy",1);
            PlayerPrefs.SetInt("OneMarble",0);
            PlayerPrefs.SetInt("DrawNewHand",1);
            PlayerPrefs.SetInt("UseCombat",1);
            PlayerPrefs.SetInt("EverBeenOpened",1);
        }
        
        
        energyToggle.isOn = PlayerPrefs.GetInt("UseEnergy") == 1;
        marbleToggle.isOn = PlayerPrefs.GetInt("OneMarble") == 1;
        drawHandToggle.isOn = PlayerPrefs.GetInt("DrawNewHand") == 1;
        combatToggle.isOn = PlayerPrefs.GetInt("UseCombat") == 1;

        versionText.text = "version " + AnalyticsManager.GAME_VERSION;
        DontDestroyOnLoad(textCanvas);
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
