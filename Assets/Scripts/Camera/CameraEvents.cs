using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class CameraEvents
{
    public static event Action<CameraManager.ShakeIntensity> OnCameraShake;
    public static void CameraShake(CameraManager.ShakeIntensity intensity)
    {
        OnCameraShake?.Invoke(intensity);
    }

    public static event Action OnCameraZoomIn;
    public static void CameraZoomIn()
    {
        OnCameraZoomIn?.Invoke();
    }
    
    public static event Action<Vector3> OnUpdateCameraZoom;
    public static void UpdateCameraZoom(Vector3 pos)
    {
        OnUpdateCameraZoom?.Invoke(pos);
    }
}
