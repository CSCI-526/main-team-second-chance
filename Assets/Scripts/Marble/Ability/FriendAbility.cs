using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

[CreateAssetMenu(fileName = "NewFriendAbility", menuName = "ScriptableObjects/Abilities/Friend")]
public class FriendAbility : Ability
{
    public override void CollisionCast(Marble marble, Marble other)
    {
        if (marble.Team == other.Team)
        {
            HealthManager manager = marble.Team == MarbleTeam.Player ? GameManager.Instance.GetEnemyManager().GetHealthManager() : GameManager.Instance
                .GetPlayerManager().GetHealthManager();
            manager.TakeDamage(other.GetPoints());
            AudioManager.TriggerSound(AbilitySound,marble.transform.position);
        }
    }
}
