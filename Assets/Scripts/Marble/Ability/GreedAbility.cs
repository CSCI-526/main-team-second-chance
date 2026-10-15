using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/Abilities/Greed")]
public class GreedAbility : Ability
{
    public override void CollisionCast(Marble marble, Marble other)
    {
        if (marble.Team == MarbleTeam.Player)
        {
            GameManager.Instance.GetPlayerManager().GetPlayerDeck().DrawCard();
        }
        else
        {
            GameManager.Instance.GetEnemyManager().GetEnemyDeck().DrawCard();
        }

        AudioManager.TriggerSound(AbilitySound,marble.transform.position);
    }
}
