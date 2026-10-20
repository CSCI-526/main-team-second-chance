using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewCameraShakeMap", menuName = "ScriptableObjects/Camera/CameraShakeMap")]
public class CameraShakeMap : ScriptableObject
{
    private bool initialized = false;
    [SerializeField] private List<CameraShakeData> _shakeDatas;
    private Dictionary<CameraManager.ShakeIntensity, CameraShakeData> _shakeMap = new ();

    private void InitializeMap()
    {
        _shakeMap.Clear();
        foreach (var data in _shakeDatas)
        {
            _shakeMap.Add(data.intensity,data);
        }
    }
    
    public CameraShakeData GetShakeData(CameraManager.ShakeIntensity intensity)
    {
        if (!initialized)
        {
            InitializeMap();
        }

        return _shakeMap[intensity];
    }
}
