using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public GameObject tutorialBar;
    public GameObject tutorial;

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
            bCanShootMarble = CanShootMarble(To2DSpace);
            if (bCanShootMarble)
            {
                LineRenderer.enabled = true;
                LineRenderer.SetPosition(0, StartLocationMouse);
                LineRenderer.SetPosition(1, StartLocationMouse);

                if(tutorialBar != null && tutorial != null)
                {
                    tutorialBar.SetActive(false);
                    tutorial.SetActive(false);
                }
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
                float DirectionMagnitude = Vector3.Magnitude(Direction);
                if (DirectionMagnitude < Mathf.Epsilon)
                {
                    return;
                }
                Debug.Log("Direction magnitude: " + DirectionMagnitude);
                GameObject MarbleObject = GameManager.Instance.GetPlayerManager().GetPlayerDeck().UseMarble(MarbleTeam.Player);
                if (!MarbleObject)
                {
                    return;
                }
                MarbleLauncher.ins.LaunchMarble(Direction.normalized, DirectionMagnitude, StartLocationMouse, MarbleObject);
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

    private bool CanShootMarble(Vector2 testPoint)
    {
        // Valid Scoring Zone
        bool bScoringZoneTest = IsNotInScoringZone(testPoint);
        if (!bScoringZoneTest)
        {
            Debug.LogError("PlayerController.CanShootMarble(Vector2 testPoint): You are in the scoring zone. You should try shooting outside of the scoring zone");
        }
        bool bValidDeckSize = GameManager.Instance.GetPlayerManager().GetPlayerDeck().GetDeckSize() > 0 || GameManager.Instance.GetPlayerManager().GetPlayerDeck().GetHandSize() > 0;
        if (!bValidDeckSize)
        {
            Debug.LogError("PlayerController.CanShootMarble(Vector2 testPoint): Your deck is empty. You cannot shoot anymore");
        }
        bool bHasSelectedAMarble = GameManager.Instance.GetPlayerManager().GetPlayerDeck().GetSelectedMarbleIndex() >= 0;
        if (!bHasSelectedAMarble)
        {
            Debug.LogError("PlayerController.CanShootMarble(Vector2 testPoint): You have not yet selected a marble. Please pick one to shoot");
        }
        bool bMarblesMoving = GameManager.Instance.GetAreMarblesMoving();
        if (bMarblesMoving)
        {
            Debug.LogError("PlayerController.CanShootMarble(Vector2 testPoint): Marbles are still moving. Wait until marbles have stopped until you shoot again");
        }

        bool bIsCorrectState = GameManager.Instance.turnState == TurnState.PlayerTurn;
        if (!bIsCorrectState)
        {
            Debug.LogError("PlayerController.CanShootMarble(Vector2 testPoint): It is not the player's turn. Please wait");
        }
        return bScoringZoneTest && bValidDeckSize && bHasSelectedAMarble && !bMarblesMoving && bIsCorrectState;
    }
}
