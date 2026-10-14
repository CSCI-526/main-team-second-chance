using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/Abilities/Constellation")]
public class ConstellationAbility : Ability
{
    [SerializeField] private float damageTime = 1.5f;
    [SerializeField] private float placementAllowance = 0.5f;
    [SerializeField] private int constellationDamage = 15;
    
    
    public override Sequence RoundEndCast(Marble marble)
    {
        List<Marble> marblesInScoring = new List<Marble>(GameManager.Instance.GetMarblesList());
        var position = marble.transform.position;

        foreach (var marbleToCompare in marblesInScoring)
        {
            if (marble == marbleToCompare || marbleToCompare.Team != marble.Team)
                continue;
            Vector3 otherPos = marbleToCompare.transform.position;
            Vector3 starToMarble = otherPos - position;

            // try constellation marble as the center
            if (MarbleCheckHelper(position - starToMarble,marble) || MarbleCheckHelper(otherPos + starToMarble,marble))
            {
                return DealDamageHelper(marble);
            }
        }

        return null;
    }

    private Sequence DealDamageHelper(Marble marble)
    {
        HealthManager manager = marble.Team == MarbleTeam.Player ? GameManager.Instance.GetEnemyManager().GetHealthManager() : GameManager.Instance
            .GetPlayerManager().GetHealthManager();
        manager.TakeDamage(constellationDamage);

        return WaitForSecondsSequence(damageTime);
    }

    private bool MarbleCheckHelper(Vector3 target, Marble marble)
    {
        Collider[] colliders = Physics.OverlapSphere(target, placementAllowance, LayerMask.GetMask("MarblePhysics"));

        foreach (Collider hit in colliders)
        {
            Marble m = hit.GetComponent<Marble>();
            if (m != null)
            {
                if (m.Team == marble.Team)
                {
                    return true;
                }
            }
        }

        return false;
    }
}
