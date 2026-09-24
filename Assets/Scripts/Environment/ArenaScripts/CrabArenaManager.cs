using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class CrabArenaManager : ArenaManager
{
    [SerializeField] private GameObject rightHand;
    [SerializeField] private GameObject leftHand;
    [SerializeField] private float radius = 1.5f;
    [SerializeField] private float power = 10.0f;
    protected override Coroutine OnRoundStart()
    {
        return StartCoroutine(RoundStartRoutine());
    }

    private IEnumerator RoundStartRoutine()
    {
        yield return Slam(leftHand.transform,FindTarget());
        yield return Slam(rightHand.transform,FindTarget());
        yield return new WaitForSeconds(1.0f * Time.timeScale);
    }

    private IEnumerator Slam(Transform fist, Vector3 target)
    {
        Vector3 returnPos = fist.position;
        Sequence s = DOTween.Sequence();
        s.Append(fist.DOShakePosition(0.5f * Time.timeScale));
        s.Append(fist.DOMove(target + new Vector3(0.0f, 2.0f, 0.0f), 1.5f * Time.timeScale));
        s.Append(fist.DOMove(target, 0.25f * Time.timeScale));
        yield return s.WaitForCompletion();
        
        Collider[] colliders = Physics.OverlapSphere(target, radius, LayerMask.GetMask("MarblePhysics"));

        foreach (Collider hit in colliders)
        {
            Rigidbody rb = hit.GetComponent<Rigidbody>();

            if (rb != null && hit.CompareTag("Marble"))
                rb.AddExplosionForce(power, target, radius, 0.0f, ForceMode.Impulse);
        }
        
        fist.DOShakePosition(0.125f * Time.timeScale);
        fist.DOMove(returnPos, 2.0f * Time.timeScale);
    }

    private Vector3 FindTarget()
    {
        Vector3 target = Vector3.zero;
        foreach (var marble in GameManager.Instance.GetMarblesList())
        {
            if (marble != null || !marble.bIsInsideScoringCircle || !marble.isActiveAndEnabled || marble.Team == MarbleTeam.Enemy)
            {
                continue;
            }

            return marble.transform.position;
        }
        
        
        ScoringZoneManager zoneManager = GameManager.Instance.GetScoringZoneManager();
        CapsuleCollider circle = (CapsuleCollider)zoneManager.GetDefaultScoringZone();
        
        float zoneRadius = circle.radius;
        var rand = Random.insideUnitCircle;
        target = circle.transform.position + new Vector3(zoneRadius * rand.x,0.0f,zoneRadius * rand.y) * 0.95f;
        
        return target;
    }
}
