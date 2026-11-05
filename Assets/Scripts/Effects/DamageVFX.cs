using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class DamageVFX : MonoBehaviour
{
    [SerializeField] private TrailRenderer trail;
    public Sequence FlyToTarget(Vector3 target, float time, Color color)
    {
        trail.startColor = color;
        trail.endColor = color;
        float t = 0.0f;
        Vector3 start = transform.position;
        Vector3 dir = target - start;
        Vector3 startControl = start  + (dir / 4.0f) + Random.onUnitSphere;
        Vector3 endControl = target + -(dir / 4.0f) + Random.onUnitSphere;

        
        
        Sequence sequence = DOTween.Sequence();
        sequence.Append(
            DOTween.To(() => t, x =>
            {
                t = x;
                gameObject.transform.position = DOCurve.CubicBezier.GetPointOnSegment(start, startControl, target, endControl, t);

            }, 1.0f, time * Time.timeScale));
        sequence.OnComplete(() => { Destroy(gameObject); });

        return sequence;
    }
}
