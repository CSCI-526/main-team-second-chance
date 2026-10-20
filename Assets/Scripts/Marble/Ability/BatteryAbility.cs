using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/Abilities/Battery")]
public class BatteryAbility : Ability
{
    public override void KnockoutCast(Marble knocker, Marble knocked, Marble self)
    {
        if (knocked == self && self.Team == MarbleTeam.Player)
        {
            AudioManager.TriggerSound(AbilitySound,self.transform.position);
            EnergyEvents.SpendEnergy(-1);
        }
    }
}
