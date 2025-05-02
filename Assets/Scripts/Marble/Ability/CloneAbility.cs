using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "NewCloneAbility", menuName = "ScriptableObjects/Abilities/Clone")]
public class CloneAbility : Ability
{

    public int clones = 0;
    public override void CollisionCast(Marble marble, Marble other)
    {
        Debug.Log("Ability Casted: Clone");
        if (marble.OneTimeCasted)
        {
            return;
        }

        marble.OneTimeCasted = true;
        //AudioManager.TriggerSound(AbilitySound, marble.transform.position);

        Rigidbody rb = marble.GetComponent<Rigidbody>();
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

    public override float SettledCast(Marble marble)
    {
        clones = 0;
        return 0.0f;
    }
}
