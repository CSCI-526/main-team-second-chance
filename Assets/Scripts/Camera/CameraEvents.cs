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

    public static event Action<Vector2> OnCameraZoomOut;
    public static void CameraZoomOut(Vector2 mousePos)
    {
        OnCameraZoomOut?.Invoke(mousePos);
    }
}
