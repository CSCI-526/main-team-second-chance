using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private Vector3 StartLocationMouse = Vector3.zero;
    private Vector3 EndLocationMouse = Vector3.zero;
    private Vector3 CurrentLocationMouse = Vector3.zero;
    private LineRenderer lineRenderer;
    // Start is called before the first frame update
    void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            StartLocationMouse = ConvertMouseIntoWorldSpace();
            lineRenderer.enabled = true;
            lineRenderer.SetPosition(0, StartLocationMouse);
            lineRenderer.SetPosition(1, StartLocationMouse);
        }
        if (Input.GetMouseButton(0))
        {
            CurrentLocationMouse = ConvertMouseIntoWorldSpace();
            lineRenderer.SetPosition(1, CurrentLocationMouse);
        }
        if (Input.GetMouseButtonUp(0))
        {
            lineRenderer.enabled = false;
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
