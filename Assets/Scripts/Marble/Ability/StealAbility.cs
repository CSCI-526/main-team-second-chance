using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewSplitAbility", menuName = "ScriptableObjects/Abilities/Steal")]

public class StealAbility : Ability
{
    public override void CollisionCast(Marble marble, Marble other)
    {
        if (marble.timesCasted >= abilityMaxTriggers)
        {
            return;
        }

        marble.timesCasted++;
        AudioManager.TriggerSound(AbilitySound,marble.transform.position);

        if (other.Team != marble.Team)
        {
            other.SetMarblePoints(marble.Team,other.GetPoints());
        }
    }
}
