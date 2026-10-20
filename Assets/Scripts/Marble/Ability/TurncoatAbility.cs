using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/Abilities/Turncoat")]
public class TurncoatAbility : Ability
{
    [SerializeField] private float spawnTime = 1.5f;

    public override Sequence RoundEndCast(Marble marble)
    {
        marble.SetMarblePoints(marble.Team == MarbleTeam.Player ? MarbleTeam.Enemy : MarbleTeam.Player,marble.GetPoints());
        AudioManager.TriggerSound(AbilitySound,marble.transform.position);
        return WaitForSecondsSequence(spawnTime);
    }
}
