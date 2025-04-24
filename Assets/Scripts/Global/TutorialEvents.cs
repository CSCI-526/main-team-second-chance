using System;
using System.Collections;
using System.Collections.Generic;

public static class TutorialEvents
{
    public static event Action<TutorialPhases> OnTryDisplayTutorialItem;
    public static void DoTryDisplayTutorialItem(TutorialPhases tutorialPhases)
    {
        OnTryDisplayTutorialItem?.Invoke(tutorialPhases);
    }
    public static event Action<TutorialPhases> OnTutorialItemDisplayed;
    public static void DoTutorialItemDisplayed(TutorialPhases TutorialPhase)
    {
        OnTutorialItemDisplayed?.Invoke(TutorialPhase);
    }
}
