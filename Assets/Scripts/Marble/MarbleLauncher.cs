using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MarbleLauncher : MonoBehaviour
{
    [SerializeField, Range(0.0f, 2.0f)]
    private float LaunchForceScale = 0.2f;
    [SerializeField] private AudioInfo launchSound;

    private void OnEnable()
    {
        MarbleEvents.OnMarbleReadyToLaunch += LaunchMarble;
    }
    private void OnDisable()
    {
        MarbleEvents.OnMarbleReadyToLaunch -= LaunchMarble;
    }
    public void LaunchMarble(MarbleTeam Team, MarbleData Type, Vector3 Direction, float Force, Vector3 Location, bool bOverrideWaiting, bool triggerCast)
    {
        if (GameManager.Instance.GetAreMarblesMoving() && !bOverrideWaiting)
        {
            Debug.LogWarning("MarbleLauncher.LaunchMarble(): Marbles are still moving");
            return;
        }
        Location.y = 0.25f;
        Direction.y = 0.0f;

        
        Marble MarbleIns = Marble.CreateMarble(Type,Team);
        MarbleIns.transform.SetPositionAndRotation(Location, Quaternion.identity);
        if (!MarbleIns)
        {
            Debug.LogError("MarbleLauncher.LaunchMarble(): Marble.cs is not attached to marble prefab");
            return;
        }

        Rigidbody MarbleRigidBody = MarbleIns.GetMarbleRigidbody();
        // Normalize Direction then apply launch
        Direction.Normalize();
        Direction *= LaunchForceScale * Force;
        MarbleRigidBody.AddForce(Direction, ForceMode.Impulse);
        MarbleEvents.OnMarbleSpawn(MarbleIns);
        
        if (!bOverrideWaiting)
        {
            AudioManager.TriggerSound(launchSound,Location);
            MarbleEvents.OnMarbleLaunch();
        }

        if (triggerCast)
        {
            MarbleIns.CastAbility();
        }
        else
        {
            MarbleIns.timesCasted = MarbleIns.GetMarbleData().AbilityObject.abilityMaxTriggers;
        }

        if (Team == MarbleTeam.Player)
        {
            AnalyticsManager.SendMetric("launch_position", new AnalyticsManager.Vector2Metric(
                new Vector2(Location.x, Location.z)));
        }
    }
}
