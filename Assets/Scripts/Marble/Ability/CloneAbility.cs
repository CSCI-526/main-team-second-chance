using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
[CreateAssetMenu(fileName = "NewCloneAbility", menuName = "ScriptableObjects/Abilities/Clone")]
public class CloneAbility : Ability
{

    public int clones = 0;
    public override void CollisionCast(Marble marble, Marble other)
    {
        Debug.Log("Ability Casted: Clone");
        if (marble.timesCasted >= abilityMaxTriggers)
            return;

        marble.timesCasted++;
        //AudioManager.TriggerSound(AbilitySound, marble.transform.position);

        Rigidbody rb = marble.GetMarbleRigidbody();
        Quaternion rotation = Quaternion.Euler(0.0f, 90.0f, 0.0f);
        Vector3 Offset = rotation * rb.velocity;
        Vector3 Position = marble.transform.position + 0.1f * Offset.normalized;

        if (clones < 50)
        {
            MarbleEvents.MarbleReadyToLaunch(other.Team, other.GetMarbleData(), Offset.normalized, Offset.magnitude, Position, true);
            AudioManager.TriggerSound(AbilitySound, marble.transform.position);
            ++clones;
        }
    }

    public override Sequence SettledCast(Marble marble)
    {
        clones = 0;
        return null;
    }
}
