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
    private int TutorialTextIndex = (int)TutorialPhases.END_GAME + 1;
    private float AnimTimer;
    private const float ANIM_DURATION = 2f;
    private const float POS_Y_OFFSET = -10f;
    private float GlowPowerStart = 0f;
    private float GlowPowerEnd = 0.2f;
    private float PosYStart;
    private float PosYEnd;

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

    void Update()
    {
        if (!TutorialManager.Instance.ShouldDisplayAnymore || 
            TutorialTextIndex > (int)TutorialPhases.END_GAME)
        {
            return;
        }

        
        TextMeshProUGUI TutorialItem = TutorialTexts[TutorialTextIndex];
        float newPosY = Mathf.Lerp(PosYStart, PosYEnd, AnimTimer / ANIM_DURATION);
        Vector3 newPosition = TutorialItem.transform.position;
        newPosition.y = newPosY;

        TutorialItem.transform.position = newPosition;
        TutorialItem.fontSharedMaterial.SetFloat(
            ShaderUtilities.ID_GlowPower, 
            Mathf.Lerp(GlowPowerStart, GlowPowerEnd, AnimTimer / ANIM_DURATION)
            );
        
        AnimTimer += Time.deltaTime;
        if (AnimTimer >= ANIM_DURATION) 
        {
            AnimTimer -= ANIM_DURATION;
            
            float temp = GlowPowerStart;
            GlowPowerStart = GlowPowerEnd;
            GlowPowerEnd = temp;
            
            temp = PosYStart;
            PosYStart = PosYEnd;
            PosYEnd = temp;
        }
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
        TutorialTexts[TutorialTextIndex].alpha = 0.0f;

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
        TutorialTextIndex = (int)Phase;
        TextMeshProUGUI TutorialItem = TutorialTexts[TutorialTextIndex];
        TutorialItem.alpha = 1.0f;
        PosYStart = TutorialItem.transform.position.y;
        PosYEnd = PosYStart + POS_Y_OFFSET;
        TutorialItem.fontSharedMaterial.SetFloat(ShaderUtilities.ID_GlowOuter, 1f);
        TutorialItem.fontSharedMaterial.SetFloat(ShaderUtilities.ID_GlowOffset, 1f);
        TutorialItem.fontSharedMaterial.SetColor(ShaderUtilities.ID_GlowColor, TutorialItem.color);
        AnimTimer = 0.0f;
    }
}
