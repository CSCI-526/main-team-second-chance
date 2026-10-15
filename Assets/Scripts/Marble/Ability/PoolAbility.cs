using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(menuName = "ScriptableObjects/Abilities/Pool")]
public class PoolAbility : Ability
{
    private int _marblesKnockedOut = 0;
    [SerializeField] private int knockoutsForDamage = 5;
    [SerializeField] private int knockoutBigDamage = 88;
    
    private void OnEnable()
    {
        TurnStateEvents.OnMatchEnd += result => _marblesKnockedOut = 0;
    }

    public override void KnockoutCast(Marble knocker, Marble knocked, Marble self)
    {
        if (self == knocked)
        {
            _marblesKnockedOut += 1;
            AudioManager.TriggerSound(AbilitySound,self.transform.position);
            if (_marblesKnockedOut >= knockoutsForDamage)
            {
                HealthManager manager = self.Team == MarbleTeam.Player ? GameManager.Instance.GetEnemyManager().GetHealthManager() : GameManager.Instance
                    .GetPlayerManager().GetHealthManager();
                manager.TakeDamage(knockoutBigDamage);
            }
        }
    }
}
