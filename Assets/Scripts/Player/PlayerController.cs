using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private Vector3 StartLocationMouse = Vector3.zero;
    private Vector3 EndLocationMouse = Vector3.zero;
    private LineRenderer LineRenderer;
    private bool bCanShootMarble = false;
    // Start is called before the first frame update
    void Start()
    {
        LineRenderer = GetComponent<LineRenderer>();
        LineRenderer.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            StartLocationMouse = ConvertMouseIntoWorldSpace();
            Vector2 To2DSpace = new Vector2(StartLocationMouse.x, StartLocationMouse.z);
            bCanShootMarble = IsNotInScoringZone(To2DSpace) && GameManager.Instance.GetPlayerManager().GetPlayerDeck().GetDeckSize() > 0;
            if(bCanShootMarble)
            {
                LineRenderer.enabled = true;
                LineRenderer.SetPosition(0, StartLocationMouse);
                LineRenderer.SetPosition(1, StartLocationMouse);
            }
            else
            {
                Debug.LogError("You are in the scoring Zone or your deck is empty");
            }
        }
        if (Input.GetMouseButton(0))
        {
            if (bCanShootMarble)
            {
                EndLocationMouse = ConvertMouseIntoWorldSpace();
                LineRenderer.SetPosition(1, EndLocationMouse);
            }
        }
        if (Input.GetMouseButtonUp(0))
        {
            if (bCanShootMarble)
            {
                LineRenderer.enabled = false;
                EndLocationMouse = ConvertMouseIntoWorldSpace();
                Vector3 Direction = StartLocationMouse - EndLocationMouse;
                float DirectionMagnitude= Vector3.Magnitude(Direction);
                Debug.Log("Direction magnitude: " + DirectionMagnitude);
                PlayerManager PlayerManager= GameManager.Instance.GetPlayerManager();
                GameObject MarbleObject = GameManager.Instance.GetPlayerManager().GetPlayerDeck().UseMarble(MarbleTeam.Player);
                MarbleLauncher.ins.LaunchMarble(Direction.normalized, 1.0f, StartLocationMouse, MarbleTeam.Player, MarbleObject);
            }
            bCanShootMarble = true;
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

    private bool IsNotInScoringZone(Vector2 testPoint)
    {
        GameObject scoreZone = GameManager.Instance.GetScoringCircle();
        CapsuleCollider capsuleCollider = scoreZone.GetComponent<CapsuleCollider>();

        Vector2 zoneCenter = new Vector2(scoreZone.transform.position.x, scoreZone.transform.position.z);

        float sqrMag = Vector2.SqrMagnitude(testPoint - zoneCenter);
        float sqrRad = (capsuleCollider.radius * capsuleCollider.radius);
        if (sqrMag <= sqrRad)
        {
            return false;
        }
        return true;
    }
}
