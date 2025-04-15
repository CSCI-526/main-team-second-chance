using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using TMPro;
using UnityEngine.PlayerLoop;

public class ScoringCircle : MonoBehaviour
{
    public float BaseRadius = 3.0f;
    private float TargetRadius = 1.5f;
    private float StartRadius = 3.0f;
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

    public void ShrinkScoringCircle(float NewRadius, bool ProgressTurn = true)
    {
        StartRadius = GetComponent<CapsuleCollider>().radius;
        TargetRadius = NewRadius;
        StartCoroutine(ShrinkCirlce(ProgressTurn));
    }

    IEnumerator ShrinkCirlce(bool ProgressTurn)
    {
        float timer = 0.0f;
        float length = 3.0f;
        while (timer < length)
        {
            float newRadius = Mathf.Lerp(StartRadius, TargetRadius, timer / length);
            GetComponent<CapsuleCollider>().radius = newRadius;
            transform.GetChild(0).transform.localScale = new Vector3(newRadius * 2.0f, 1.0f, newRadius * 2.0f);
            
            timer += Time.deltaTime;
            yield return null;
        }

        if (ProgressTurn)
        {
            GameManager.Instance.CleanupMarbles();
            TurnStateEvents.OnTurnProgressed(TurnState.EnemyTurn);
        }
    }
}
