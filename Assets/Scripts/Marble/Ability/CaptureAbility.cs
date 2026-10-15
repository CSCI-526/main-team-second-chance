using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/Abilities/Capture")]
public class CaptureAbility : Ability
{
    public override void CollisionCast(Marble marble, Marble other)
    {
        if (marble.timesCasted >= abilityMaxTriggers)
        {
            return;
        }

        if (other.bIsInsideScoringCircle)
        {
            Deck deck = marble.Team == MarbleTeam.Player
                ? GameManager.Instance.GetPlayerManager().GetPlayerDeck()
                : GameManager.Instance.GetEnemyManager().GetEnemyDeck();
            
            deck.DiscardPile.Add(other.GetMarbleData());
            other.DestroyMarble();
        }

        AudioManager.TriggerSound(AbilitySound,marble.transform.position);
        marble.timesCasted++;
    }
}
