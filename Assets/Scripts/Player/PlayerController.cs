using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public GameObject tutorialBar;
    public GameObject tutorial;
    private Vector3 StartLocationMouse = Vector3.zero;
    private Vector3 EndLocationMouse = Vector3.zero;
    private LineRenderer LineRenderer;
    private bool bCanShootMarble = false;

    public Texture2D restrictedCursorTexture;
    public Texture2D allowedCursorTexture;
    public Texture2D launchingCursorTexture;
    private bool bShowedTutorial = false;
    private float TimeSinceMouseHeldDown = 0.0f;
    [SerializeField]
    private float MINIMUM_TIME_HELD_DOWN = 0.5f;
    [SerializeField]
    private float MINIMUM_FORCE_USED = 4.0f;
    [SerializeField]
    private float MAXIMUM_FORCE_USED = 10.0f;
    [SerializeField]
    private float MAX_DRAG_DISTANCE = 5.0f;
    void Start()
    {
        LineRenderer = GetComponent<LineRenderer>();
        LineRenderer.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        // DEBUG TOOLS
#if UNITY_EDITOR
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            SceneManagerScript.Instance.loadSceneByIndex(2);
        }
        else if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            GameManager.Instance.ForceUpdateEvents(TurnState.GameOver);
        }
#endif
        bool isPlayerTurnAndHasSelectedMarble =
            GameManager.Instance.GetTurnState() == TurnState.PlayerTurn &&
            GameManager.Instance.PlayerHasSelectedMarble();
        if (isPlayerTurnAndHasSelectedMarble)
        {
            if (!bShowedTutorial)
            {
                if (tutorialBar != null && tutorial != null)
                {
                    tutorialBar.SetActive(true);
                    tutorial.SetActive(true);
                }
            }

            UpdateCursor();
        }
        else
        {
            if (tutorialBar != null && tutorial != null)
            {
                if (tutorialBar.activeSelf)
                {
                    tutorialBar.SetActive(false);
                }
                if (tutorial.activeSelf)
                {
                    tutorial.SetActive(false);
                }
            }
            Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
        }

        // Currently dragging mouse
        if (Input.GetMouseButton(0))
        {
            if (bCanShootMarble)
            {
                Vector3 MouseSpace = ConvertMouseIntoWorldSpace();
                MouseSpace.y = 1;
                if (Vector3.Distance(StartLocationMouse, MouseSpace) < MAX_DRAG_DISTANCE)
                {
                    EndLocationMouse = MouseSpace;
                    EndLocationMouse.y = 1;
                }
                else
                {
                    // direction vector
                    Vector3 StartToMouse = MouseSpace - StartLocationMouse;
                    StartToMouse.Normalize();

                    // modify end location mouse to also match 
                    EndLocationMouse = StartLocationMouse + StartToMouse * MAX_DRAG_DISTANCE;
                    EndLocationMouse.y = 1;
                }
                LineRenderer.SetPosition(1, EndLocationMouse);
                GameManager.Instance.GetPlayerManager().isLaunchingMarble = true;
            }
            else
            {
                GameManager.Instance.GetPlayerManager().isLaunchingMarble = false;
            }
            TimeSinceMouseHeldDown += Time.deltaTime;
        }
        // Start mouse drag
        if (Input.GetMouseButtonDown(0))
        {
            StartLocationMouse = ConvertMouseIntoWorldSpace();
            bCanShootMarble = CanShootMarble(StartLocationMouse);
            // Must set this after CanShootMarble() to avoid problems in IsNotInButtonsZone().
            // Must be higher than 0 so that the line renders above the PlayPlane.
            StartLocationMouse.y = 1;
            if (bCanShootMarble)
            {
                LineRenderer.enabled = true;
                LineRenderer.SetPosition(0, StartLocationMouse);
                LineRenderer.SetPosition(1, StartLocationMouse);

                if (tutorialBar != null && tutorial != null)
                {
                    tutorialBar.SetActive(false);
                    tutorial.SetActive(false);
                }
            }
        }
        // Release mouse drag
        if (Input.GetMouseButtonUp(0))
        {
            if (bCanShootMarble)
            {
                GameManager.Instance.GetPlayerManager().isLaunchingMarble = false;
                LineRenderer.enabled = false;
                EndLocationMouse = ConvertMouseIntoWorldSpace();
                EndLocationMouse.y = 1;
                Vector3 Direction = StartLocationMouse - EndLocationMouse;
                float DirectionMagnitude = Vector3.Magnitude(Direction);
                Debug.Log("MagDir " + DirectionMagnitude);
                // Ignore launches that are too weak. OR we have not held down the mouse for long enough
                if (DirectionMagnitude < MINIMUM_FORCE_USED || TimeSinceMouseHeldDown < MINIMUM_TIME_HELD_DOWN)
                {
                    TimeSinceMouseHeldDown = 0.0f;
                    return;
                }
                MarbleData MarbleData = GameManager.Instance.GetPlayerManager().GetPlayerDeck().UseMarble(MarbleTeam.Player);
                if (!MarbleData)
                {
                    Debug.LogError("PlayerController.Update(): MarbleData requested is Null");
                    return;
                }

                if (!bShowedTutorial)
                {
                    bShowedTutorial = true;
                }

                TimeSinceMouseHeldDown = 0.0f;
                Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
                // Apply nonlinear scaling
                DirectionMagnitude = Mathf.Pow(DirectionMagnitude, 1.5f);
                // Clamp the direction magnitude
                DirectionMagnitude = Mathf.Clamp(DirectionMagnitude, MINIMUM_FORCE_USED, MAXIMUM_FORCE_USED);
                Debug.Log("Modified Dir Mag: " + DirectionMagnitude);

                MarbleEvents.MarbleReadyToLaunch(MarbleTeam.Player, MarbleData, Direction, DirectionMagnitude, StartLocationMouse, false);
            }
            bCanShootMarble = true;
        }
    }

    private void UpdateCursor()
    {
        Vector3 mousePos = ConvertMouseIntoWorldSpace();
        if (!IsNotInScoringZone(mousePos))
        {
            Cursor.SetCursor(restrictedCursorTexture, new Vector2(12, 12), CursorMode.Auto);
        }
        else if (LineRenderer.enabled)
        {
            Cursor.SetCursor(launchingCursorTexture, new Vector2(12, 12), CursorMode.Auto);
        }
        else if (GameManager.Instance.GetPlayerManager().GetPlayerDeck().bIsHoveringDeck || !IsNotInButtonsZone(mousePos))
        {
            Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
        }
        else
        {
            Cursor.SetCursor(allowedCursorTexture, new Vector2(12, 12), CursorMode.Auto);
        }
    }

    Vector3 ConvertMouseIntoWorldSpace()
    {
        Vector3 worldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        return worldPos;
    }

    private bool IsNotInScoringZone(Vector3 testWorldPoint)
    {
        Vector2 WorldPoint2D = new(testWorldPoint.x, testWorldPoint.z);
        GameObject scoreZone = GameManager.Instance.GetScoringCircle();
        CapsuleCollider capsuleCollider = scoreZone.GetComponent<CapsuleCollider>();

        Vector2 zoneCenter = new(scoreZone.transform.position.x, scoreZone.transform.position.z);

        float sqrMag = Vector2.SqrMagnitude(WorldPoint2D - zoneCenter);
        float sqrRad = capsuleCollider.radius * capsuleCollider.radius;
        if (sqrMag <= sqrRad)
        {
            return false;
        }
        return true;
    }

    private bool IsNotInButtonsZone(Vector3 testWorldPoint)
    {
        Vector3 testScreenPoint = Camera.main.WorldToScreenPoint(testWorldPoint);
        Vector2 ScreenPoint2D = new(testScreenPoint.x, testScreenPoint.y);
        RectTransform buttonsRect = GameManager.Instance.GetMainUIButtons().GetComponent<RectTransform>();
        return !RectTransformUtility.RectangleContainsScreenPoint(buttonsRect, ScreenPoint2D);
    }

    private bool IsNotInRestrictedZones(Vector3 testWorldPoint)
    {
        return IsNotInScoringZone(testWorldPoint) && IsNotInButtonsZone(testWorldPoint);
    }

    private bool CanShootMarble(Vector3 testWorldPoint)
    {
        // Valid Scoring Zone
        bool bRestrictedZoneTest = IsNotInRestrictedZones(testWorldPoint);
        if (!bRestrictedZoneTest)
        {
            Debug.LogError("PlayerController.CanShootMarble(Vector3 testWorldPoint): You are in a restricted zone. You should try shooting outside of the restricted zone");
        }
        bool bValidDeckSize = GameManager.Instance.GetPlayerManager().GetPlayerDeck().GetDeckSize() > 0 || GameManager.Instance.GetPlayerManager().GetPlayerDeck().GetHandSize() > 0;
        if (!bValidDeckSize)
        {
            Debug.LogError("PlayerController.CanShootMarble(Vector3 testWorldPoint): Your deck is empty. You cannot shoot anymore");
        }
        bool bHasSelectedAMarble = GameManager.Instance.PlayerHasSelectedMarble();
        if (!bHasSelectedAMarble)
        {
            Debug.LogError("PlayerController.CanShootMarble(Vector3 testWorldPoint): You have not yet selected a marble. Please pick one to shoot");
        }
        bool bMarblesMoving = GameManager.Instance.GetAreMarblesMoving();
        if (bMarblesMoving)
        {
            Debug.LogError("PlayerController.CanShootMarble(Vector3 testWorldPoint): Marbles are still moving. Wait until marbles have stopped until you shoot again");
        }

        bool bIsCorrectState = GameManager.Instance.GetTurnState() == TurnState.PlayerTurn;
        if (!bIsCorrectState)
        {
            Debug.LogError("PlayerController.CanShootMarble(Vector3 testWorldPoint): It is not the player's turn. Please wait");
        }
        return bRestrictedZoneTest && bValidDeckSize && bHasSelectedAMarble && !bMarblesMoving && bIsCorrectState;
    }
}
