using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewFlingAbility", menuName = "ScriptableObjects/Abilities/Fling")]
public class FlingAbility : Ability
{
    [SerializeField] private int multiplier = 3;

    public override void CollisionCast(Marble marble, Marble other)
    {
        if (marble.timesCasted >= abilityMaxTriggers)
        {
            return;
        }

        if (other.bIsInsideScoringCircle)
        {
            GameManager.Instance.UpdateEntityScore(other.Team, other.GetMarbleData().Points, false);
            HealthManager manager = marble.Team == MarbleTeam.Player ? GameManager.Instance.GetEnemyManager().GetHealthManager() : GameManager.Instance
                .GetPlayerManager().GetHealthManager();
            manager.TakeDamage(other.GetMarbleData().Points * multiplier);
            other.gameObject.SetActive(false);
        }

        AudioManager.TriggerSound(AbilitySound,marble.transform.position);
        marble.timesCasted++;
    }
}
