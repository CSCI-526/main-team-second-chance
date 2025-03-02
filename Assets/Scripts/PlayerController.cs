using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private Vector3 StartLocationMouse = Vector3.zero;
    private Vector3 EndLocationMouse = Vector3.zero;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            StartLocationMouse = ConvertMouseIntoWorldSpace();

        }
        if (Input.GetMouseButtonUp(0))
        {
            EndLocationMouse = ConvertMouseIntoWorldSpace();
            MarbleLauncher.ins.LaunchMarble((StartLocationMouse-EndLocationMouse).normalized,1.0f,StartLocationMouse,MarbleTeam.Player);
        }
    }

    Vector3 ConvertMouseIntoWorldSpace()
    {
        float y = 1.0f;
        Vector3 WorldPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        float x = WorldPoint.x;
        float z = WorldPoint.z;
        return new Vector3(x, y, z);
    }
}
