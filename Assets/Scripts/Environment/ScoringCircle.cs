using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using TMPro;
using UnityEngine.PlayerLoop;
using UnityEngine.Serialization;

public enum ZoneType
{
    Scoring,
    Launch,
    Blocked
}

public class ScoringCircle : MonoBehaviour
{
    public ZoneType Type = ZoneType.Scoring;
    public int Priority = 0;
    [SerializeField] private float shrinkAmount = 0.5f;
    private float StartRadius = 3.0f;

    private void Start()
    {
        StartRadius = GetComponent<CapsuleCollider>().radius;
    }

    private void OnTriggerEnter(Collider other)
    {
        Marble marble = other.GetComponent<Marble>();
        if (marble != null)
        {
            ScoringZoneManager.ZoneStatusChange(marble, this, true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        Marble marble = other.GetComponent<Marble>();
        if (marble != null)
        {
            ScoringZoneManager.ZoneStatusChange(marble, this, false);
        }
    }

    public void SetScoringRadius(float t)
    {
        float newRadius = Mathf.Lerp(StartRadius * shrinkAmount, StartRadius, t);
        GetComponent<CapsuleCollider>().radius = newRadius;
        transform.GetChild(0).transform.localScale = new Vector3(newRadius * 2.0f, 1.0f, newRadius * 2.0f);
    }
}
