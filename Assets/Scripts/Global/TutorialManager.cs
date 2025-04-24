using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum TutorialPhases
{
    SELECT_MARBLE = 0,
    LAUNCH_MARBLE = 1,
    SCORE_POINTS = 2,
    DRAW_CARD = 3,
    END_GAME = 4,
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
}
