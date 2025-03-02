using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using TMPro;
using UnityEngine.PlayerLoop;

public class GameplayCircle : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        Marble marble = other.GetComponent<Marble>();
        marble.bIsInsideGameplayCircle = true;
    }

    private void OnTriggerExit(Collider other)
    {
        Marble marble = other.GetComponent<Marble>();
        marble.bIsInsideGameplayCircle = false;
    }
}
