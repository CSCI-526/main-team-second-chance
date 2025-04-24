using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TutorialCanvasUI : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI[] TutorialTexts;
    [SerializeField]
    private GameObject TutorialDragBar;
    [SerializeField]
    private GameObject TutorialDragHandle;
    private void Start()
    {
        if (!TutorialDragBar)
        {
            TutorialDragBar = GameObject.Find("TutorialBar");
        }
        if (!TutorialDragHandle)
        {
            TutorialDragHandle = GameObject.Find("Tutorial");
        }
    }
    private void OnEnable()
    {
        TutorialEvents.OnTutorialItemDisplayed += DisplayedTutorialItem;
        TutorialEvents.OnTryDisplayTutorialItem += TryDisplayTutorialItem;
    }
    private void OnDisable()
    {
        TutorialEvents.OnTutorialItemDisplayed -= DisplayedTutorialItem;
        TutorialEvents.OnTryDisplayTutorialItem -= TryDisplayTutorialItem;
    }

    private void DisplayedTutorialItem(TutorialPhases Phase)
    {
        // check if phase valid 
        if ((int)Phase > (int)TutorialPhases.END_GAME)
        {
            // if it's a bad phase, we just hide all tutorials (JIC)
            foreach (var tutorial in TutorialTexts)
            {
                tutorial.alpha = 0.0f;
            }
            return;
        }

        if (Phase == TutorialPhases.LAUNCH_MARBLE)
        {
            if (TutorialDragHandle && TutorialDragBar)
            {
                TutorialDragBar.SetActive(false);
                TutorialDragHandle.SetActive(false);
            }
        }
        int PhaseAsInt = (int)Phase;
        TutorialTexts[PhaseAsInt].alpha = 0.0f;

        // Now we update the phase of the tutorial manager to go to next
        if (TutorialManager.Instance)
        {
            TutorialManager.Instance.UpdateCurrentTutorialPhase();
        }
    }
    private void TryDisplayTutorialItem(TutorialPhases Phase)
    {
        // check if phase valid 
        if ((int)Phase > (int)TutorialPhases.END_GAME)
        {
            // if it's a bad phase, we just hide all tutorials (JIC)
            foreach (var tutorial in TutorialTexts)
            {
                tutorial.alpha = 0.0f;
            }
            return;
        }

        if (Phase == TutorialPhases.LAUNCH_MARBLE)
        {
            TutorialManager.Instance.IsLaunchCalled = true;
            if (TutorialDragHandle && TutorialDragBar)
            {
                TutorialDragBar.SetActive(true);
                TutorialDragHandle.SetActive(true);
            }
        }
        int PhaseAsInt = (int)Phase;
        TutorialTexts[PhaseAsInt].alpha = 1.0f;
    }
}
