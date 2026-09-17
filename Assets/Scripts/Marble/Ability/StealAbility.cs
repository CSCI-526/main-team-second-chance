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
            if (other.bIsInsideScoringCircle)
            {
                GameManager.Instance.UpdateEntityScore(other.Team,other.GetMarbleData().Points, false);
                GameManager.Instance.UpdateEntityScore(marble.Team,other.GetMarbleData().Points, true);
            }
            other.SetMarbleTeam(marble.Team);
        }
    }
}
