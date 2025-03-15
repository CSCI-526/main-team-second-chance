using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MarbleLauncher : MonoBehaviour
{
    [SerializeField] 
    private GameObject MarblePrefab;
    [SerializeField] private float LaunchForceScale = 0.2f;
    [SerializeField] private Material playerMaterial;
    [SerializeField] private Material enemyMaterial;
    private void OnEnable()
    {
        MarbleEvents.OnMarbleReadyToLaunch += LaunchMarble;
    }
    private void OnDisable()
    {
        MarbleEvents.OnMarbleReadyToLaunch -= LaunchMarble;
    }
    public void LaunchMarble(MarbleTeam Team, MarbleData Data, Vector3 Direction, float Force, Vector3 Location)
    {
        if (GameManager.Instance.GetAreMarblesMoving())
        {
            Debug.LogWarning("MarbleLauncher.LaunchMarble(): Marbles are still moving");
            return;
        }
        Location.y = 0.25f;
        Direction.y = 0.0f;

        GameObject MarbleObject = Instantiate(MarblePrefab);
        MarbleObject.transform.SetPositionAndRotation(Location, Quaternion.identity);
        
        Marble MarbleIns = MarbleObject.GetComponent<Marble>();
        if(!MarbleIns)
        {
            Debug.LogError("MarbleLauncher.LaunchMarble(): Marble.cs is not attached to marble prefab");
            return;
        }
        MarbleIns.UpdateMarbleData(Data, Team);

        // Register the marble 
        GameManager.Instance.RegisterMarble(MarbleIns);
        MeshRenderer MarbleRenderer = MarbleObject.GetComponent<MeshRenderer>();
        if (!MarbleRenderer)
        {
            Debug.LogError("MarbleLauncher.LaunchMarble(): Prefab does not contain a mesh renderer is not attached to marble prefab");
            return;
        }
        MarbleRenderer.material = Team == MarbleTeam.Player ? playerMaterial : enemyMaterial;

        Rigidbody MarbleRigidBody = MarbleObject.GetComponent<Rigidbody>();
        Direction *= LaunchForceScale * Force;
        MarbleRigidBody.AddForce(Direction, ForceMode.Impulse);
        StartCoroutine(GameManager.Instance.WaitForMarblesToSettle());
    }
}
