using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum TutorialPhases
{
    SELECT_MARBLE = 0,
    LAUNCH_MARBLE = 1,
    ENERGY_SPENDING = 2,
    ENDING_TURN = 3,
    DAMAGING_ENEMY = 4,
    END_GAME = 5,
}
public class TutorialManager : MonoBehaviour
{
    public static TutorialManager Instance;
    public TutorialPhases CurrentTutorialPhase
    {
        get { return currentTutorialPhase; }
    }
    public bool ShouldDisplayAnymore
    {
        get { return bShouldDisplayAnymore; }
    }
    public bool IsLaunchCalled
    {
        get { return bIsLaunchCalled; }
        set { bIsLaunchCalled = value; }
    }

    private bool SpentEnergy = false;
    
    private void OnLevelWasLoaded(int level)
    {
        // if a level is loaded and we are not in endgame and we can still display, we reset 
        if (currentTutorialPhase != TutorialPhases.END_GAME && bShouldDisplayAnymore)
        {
            currentTutorialPhase = TutorialPhases.SELECT_MARBLE;
        }
    }
    public void UpdateCurrentTutorialPhase()
    {
        int TutorialPhaseToInt = (int)currentTutorialPhase;
        TutorialPhaseToInt++;
        if (TutorialPhaseToInt > (int)TutorialPhases.END_GAME)
        {
            bShouldDisplayAnymore = false;
        }
        else
        {
            currentTutorialPhase = (TutorialPhases)TutorialPhaseToInt;
        }
    }
    private TutorialPhases currentTutorialPhase = TutorialPhases.SELECT_MARBLE;
    private bool bShouldDisplayAnymore = true;
    private bool bIsLaunchCalled = false;
    private void Awake()
    {
        if (Instance != null && Instance != this)
            Destroy(this.gameObject);
        else
        {
            DontDestroyOnLoad(gameObject);
            Instance = this;
        }
    }

    private void OnEnable()
    {
        if (TutorialManager.Instance.ShouldDisplayAnymore)
        {
            EnergyEvents.OnEnergyUpdate += EnergyEventsOnOnEnergyUpdate;
        }
    }

    private void EnergyEventsOnOnEnergyUpdate(int obj)
    {
        if (!SpentEnergy && obj < 3)
        {
            TutorialEvents.DoTutorialItemDisplayed(TutorialManager.Instance.CurrentTutorialPhase);
            TutorialEvents.DoTryDisplayTutorialItem(TutorialManager.Instance.CurrentTutorialPhase);
            SpentEnergy = true;
        }
        else if (obj == 0)
        {
            TutorialEvents.DoTutorialItemDisplayed(TutorialManager.Instance.CurrentTutorialPhase);
            TutorialEvents.DoTryDisplayTutorialItem(TutorialManager.Instance.CurrentTutorialPhase);
            EnergyEvents.OnEnergyUpdate -= EnergyEventsOnOnEnergyUpdate;
        }
        
    }

    private void OnDisable()
    {
        EnergyEvents.OnEnergyUpdate -= EnergyEventsOnOnEnergyUpdate;
    }
}
