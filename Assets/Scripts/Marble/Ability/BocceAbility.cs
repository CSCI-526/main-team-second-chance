using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/Abilities/Bocce")]
public class BocceAbility : Ability
{
    [SerializeField] private float damageTime = 1.5f;
    [SerializeField] private float scoreMultiplier = 1.0f;
    
    
    public override Sequence RoundEndCast(Marble marble)
    {
        List<Marble> marblesInScoring = new List<Marble>(GameManager.Instance.GetMarblesList());
        var position = marble.transform.position;
        marblesInScoring.Sort((Marble a, Marble b) => (a.transform.position - position).sqrMagnitude.CompareTo((b.transform.position - position)
            .sqrMagnitude));

        int totalDamage = 0;
        foreach (var marbleToCompare in marblesInScoring)
        {
            if(marble == marbleToCompare)
                continue;
            if (marble.Team == marbleToCompare.Team)
            {
                totalDamage++;
            }
            else
            {
                break;
            }
        }
        
        HealthManager manager = marble.Team == MarbleTeam.Player ? GameManager.Instance.GetEnemyManager().GetHealthManager() : GameManager.Instance
            .GetPlayerManager().GetHealthManager();
        manager.TakeDamage((int)(totalDamage * scoreMultiplier));
        AudioManager.TriggerSound(AbilitySound,marble.transform.position);

        return WaitForSecondsSequence(damageTime);
    }
}
