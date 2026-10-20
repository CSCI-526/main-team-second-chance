using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

[CreateAssetMenu(fileName = "DebugDamageAbility", menuName = "ScriptableObjects/Abilities/DebugDamage")]
public class DEBUGDamageMarble : Ability
{
    [SerializeField] private MarbleTeam teamToDamage;
    [SerializeField] private int damage = 999;
    [SerializeField] private bool damageOnCollision = false;
    [SerializeField] private TurnState turnToTriggerOn;
    
    public override void CollisionCast(Marble marble, Marble other)
    {
        if (damageOnCollision)
        {
            DamageTeam();
        }

        base.CollisionCast(marble, other);
    }

    public override Sequence RoundEndCast(Marble marble)
    {
        if (GameManager.Instance.GetTurnState() == turnToTriggerOn)
        {
            DamageTeam();
        }

        return base.RoundEndCast(marble);
    }

    public override Sequence SettledCast(Marble marble)
    {
        if (GameManager.Instance.GetTurnState() == turnToTriggerOn)
        {
            DamageTeam();
        }

        return base.SettledCast(marble);
    }

    private void DamageTeam()
    {
        HealthManager manager = teamToDamage == MarbleTeam.Enemy ? GameManager.Instance.GetEnemyManager().GetHealthManager() : GameManager.Instance
            .GetPlayerManager().GetHealthManager();
        manager.TakeDamage(damage);
    }
}
