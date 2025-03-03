using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using TMPro;
using UnityEngine.PlayerLoop;

public class ScoringCircle : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        Marble marble = other.GetComponent<Marble>();
        marble.bIsInsideScoringCircle = true;
        GameManager.Instance.UpdateEntityScore(marble.Team, true);
    }

    private void OnTriggerExit(Collider other)
    {
        Marble marble = other.GetComponent<Marble>();
        marble.bIsInsideScoringCircle = false;
        GameManager.Instance.UpdateEntityScore(marble.Team, false);
    }

    private void UpdateText()
    {
    }
}
