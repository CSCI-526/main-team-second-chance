using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewGrappleAbility", menuName = "ScriptableObjects/Abilities/Grapple")]
public class GrappleAbility : Ability
{
    public override void CollisionCast(Marble marble, Marble other)
    {
        if (marble.timesCasted >= abilityMaxTriggers)
        {
            return;
        }

        marble.timesCasted++;
        AudioManager.TriggerSound(AbilitySound,marble.transform.position);
        
        FixedJoint joint = marble.gameObject.AddComponent<FixedJoint>();
        joint.connectedBody = other.GetMarbleRigidbody();
        joint.enableCollision = false; 
    }
}
