using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    public enum ShakeIntensity
    {
        Low,
        Medium,
        High
    }

    [SerializeField] private Camera mainCamera;
    [SerializeField] private CameraShakeMap shakeDatas;
    private Vector3 _originalPos;
    private float _originalCamSize;

    private Sequence _shakes;

    private void OnEnable()
    {
        _originalPos = transform.position;
        _originalCamSize = mainCamera.orthographicSize;
        CameraEvents.OnCameraShake += OnCameraShake;
        CameraEvents.OnCameraZoomOut += OnCameraZoomOut;
    }

    private void OnDisable()
    {
        CameraEvents.OnCameraShake -= OnCameraShake;
        CameraEvents.OnCameraZoomOut -= OnCameraZoomOut;
    }

    private void OnCameraZoomOut(Vector2 mousePos)
    {
        //mainCamera.orthographicSize;
    }

    private void OnCameraShake(ShakeIntensity intensity)
    {
        Sequence s = shakeDatas.GetShakeData(intensity).StartShake(transform);
        //s.Append(transform.DOMove(_originalPos, 0.5f * Time.timeScale));
        if (_shakes != null && _shakes.active && _shakes.IsPlaying())
        {
            _shakes.OnComplete(() =>
            {
                _shakes = s.Play();
            });
        }
        else
        {
            _shakes = s.Play();
        }
    }
}
