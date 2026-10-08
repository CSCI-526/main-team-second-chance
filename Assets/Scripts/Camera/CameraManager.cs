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

    [SerializeField] private float ZOOM_OUT_EDGE_AREA = 0.8f;
    private Sequence _shakes;
    private Sequence _zoomSequence;

    private void OnEnable()
    {
        _originalPos = transform.position;
        _originalCamSize = mainCamera.orthographicSize;
        CameraEvents.OnCameraShake += OnCameraShake;
        CameraEvents.OnCameraZoomIn += OnCameraZoomIn;
        CameraEvents.OnUpdateCameraZoom += UpdateCameraZoom;
        MarbleEvents.OnMarbleLaunched += OnMarbleLaunched;
    }

    private void OnDisable()
    {
        CameraEvents.OnCameraShake -= OnCameraShake;
        CameraEvents.OnCameraZoomIn -= OnCameraZoomIn;
        CameraEvents.OnUpdateCameraZoom -= UpdateCameraZoom;
        MarbleEvents.OnMarbleLaunched -= OnMarbleLaunched;
    }

    private void UpdateCameraZoom(Vector3 position)
    {
        Vector2 pos =mainCamera.WorldToViewportPoint(position);

        pos = (pos - new Vector2(0.5f,0.5f)) * 2.0f;
        if (Mathf.Abs(pos.x) > ZOOM_OUT_EDGE_AREA || Mathf.Abs(pos.y) > ZOOM_OUT_EDGE_AREA)
        {
            mainCamera.orthographicSize += 0.5f * Time.deltaTime;
        }
    }

    private void OnCameraZoomIn()
    {
       SetCameraZoom(1.0f);
    }

    private void SetCameraZoom(float scale)
    {
        float t = 0.0f;
        float startScale = mainCamera.orthographicSize;
        _zoomSequence = DOTween.Sequence();
        _zoomSequence.Append(
            DOTween.To(() => t, x =>
            {
                t = x;
                mainCamera.orthographicSize = Mathf.Lerp(startScale, _originalCamSize * scale, t);
            }, 1.0f, 1.5f * Time.timeScale));
    }
    
    private void OnMarbleLaunched(Marble marble)
    {
        DOVirtual.DelayedCall(1.5f * Time.timeScale, () => { SetCameraZoom(1.0f); }, false);
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
