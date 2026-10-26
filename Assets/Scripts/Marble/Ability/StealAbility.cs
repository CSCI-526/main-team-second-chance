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
        
        if (other.Team != marble.Team)
        {
            marble.timesCasted++;
            AudioManager.TriggerSound(AbilitySound,marble.transform.position);
            other.SetMarblePoints(marble.Team,other.GetPoints());
        }
    }
}
