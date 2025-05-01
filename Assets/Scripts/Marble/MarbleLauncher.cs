using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MarbleLauncher : MonoBehaviour
{
    [SerializeField, Range(0.0f, 2.0f)]
    private float LaunchForceScale = 0.2f;
    [SerializeField] private Material playerMaterial;
    [SerializeField] private Material playerOutlineMaterial;
    [SerializeField] private Material enemyMaterial;
    [SerializeField] private Material enemyOutlineMaterial;
    private Material[] materialCopies;
    [SerializeField] private AudioInfo launchSound;

    private void Awake()
    {
        materialCopies = new Material[2];
    }

    private void OnEnable()
    {
        MarbleEvents.OnMarbleReadyToLaunch += LaunchMarble;
    }
    private void OnDisable()
    {
        MarbleEvents.OnMarbleReadyToLaunch -= LaunchMarble;
    }
    public void LaunchMarble(MarbleTeam Team, MarbleData Type, Vector3 Direction, float Force, Vector3 Location, bool bOverrideWaiting)
    {
        if (GameManager.Instance.GetAreMarblesMoving() && !bOverrideWaiting)
        {
            Debug.LogWarning("MarbleLauncher.LaunchMarble(): Marbles are still moving");
            return;
        }
        Location.y = 0.25f;
        Direction.y = 0.0f;

        GameObject MarbleObject = Instantiate(Type.MarblePrefab);
        MarbleObject.transform.SetPositionAndRotation(Location, Quaternion.identity);

        Marble MarbleIns = MarbleObject.GetComponent<Marble>();
        if (!MarbleIns)
        {
            Debug.LogError("MarbleLauncher.LaunchMarble(): Marble.cs is not attached to marble prefab");
            return;
        }
        MarbleIns.Team = Team;
        // Register the marble 
        GameManager.Instance.RegisterMarble(MarbleIns);
        MeshRenderer MarbleRenderer = MarbleObject.GetComponent<MeshRenderer>();
        if (!MarbleRenderer)
        {
            Debug.LogError("MarbleLauncher.LaunchMarble(): Prefab does not contain a mesh renderer is not attached to marble prefab");
            return;
        }
        //MarbleRenderer.material = Team == MarbleTeam.Player ? playerMaterial : enemyMaterial;
        //MarbleRenderer.materials[1] = Team == MarbleTeam.Player ? playerOutlineMaterial : enemyOutlineMaterial;

        materialCopies[0] = Team == MarbleTeam.Player ? playerMaterial : enemyMaterial;
        materialCopies[1] = Team == MarbleTeam.Player ? playerOutlineMaterial : enemyOutlineMaterial;

        MarbleRenderer.materials = materialCopies;

        Rigidbody MarbleRigidBody = MarbleObject.GetComponent<Rigidbody>();
        // Normalize Direction then apply launch
        Direction.Normalize();
        Direction *= LaunchForceScale * Force;
        MarbleRigidBody.AddForce(Direction, ForceMode.Impulse);
        
        if (!bOverrideWaiting)
        {
            AudioManager.TriggerSound(launchSound,Location);
            StartCoroutine(GameManager.Instance.WaitForMarblesToSettle());
        }

        if (Team == MarbleTeam.Player)
        {
            AnalyticsManager.SendMetric("launch_position", new AnalyticsManager.Vector2Metric(
                new Vector2(Location.x, Location.z)));
        }
    }
}
