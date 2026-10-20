using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;

[CreateAssetMenu(fileName = "NewCameraShake", menuName = "ScriptableObjects/Camera/CameraShakeData")]
public class CameraShakeData : ScriptableObject
{
    [SerializeField] public CameraManager.ShakeIntensity intensity;
    [SerializeField] private float strength = 1.0f;
    [SerializeField] private float duration = 1.0f;
    [SerializeField] private int vibrato = 10;

    public Sequence StartShake(Transform t)
    {
        Sequence seq = DOTween.Sequence();
        seq.Pause();
        seq.Append(t.DOShakePosition(duration * Time.timeScale, strength, vibrato));
        return seq;
    }
}
