using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "NewSplitAbility", menuName = "ScriptableObjects/Abilities/Split")]
public class SplitAbility : Ability
{
    public override void Cast(Marble marble)
    {
        Debug.Log("Ability Casted: Split");
        if (marble == null) return;

        Rigidbody rb = marble.GetComponent<Rigidbody>();
        Quaternion rotation = Quaternion.Euler(0.0f, 90.0f, 0.0f);
        Vector3 Offset = rotation * rb.velocity;
        Vector3 Position = marble.transform.position + 0.1f * Offset.normalized;


        MarbleEvents.MarbleReadyToLaunch(marble.Team, GameManager.Instance.GetDeckManager().GetDefaultMarble(), Offset.normalized, Offset.magnitude, Position, true);
    }
}
