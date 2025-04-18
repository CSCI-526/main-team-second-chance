using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewSplitAbility", menuName = "ScriptableObjects/Abilities/Steal")]

public class StealAbility : Ability
{
    [SerializeField] private Material playerMaterial;
    [SerializeField] private Material enemyMaterial;

    private bool bHasStolen = false;
    
    public override void CollisionCast(Marble marble, Marble other)
    {
        if (bHasStolen)
        {
            return;
        }

        bHasStolen = true;

        if (other.Team != marble.Team)
        {
            if (other.bIsInsideScoringCircle)
            {
                GameManager.Instance.UpdateEntityScore(other.Team, false);
                GameManager.Instance.UpdateEntityScore(marble.Team, true);
            }
            other.Team = marble.Team;
        }



        MeshRenderer MarbleRenderer = other.GetComponent<MeshRenderer>();
        if (!MarbleRenderer)
        {
            Debug.LogError("MarbleLauncher.LaunchMarble(): Prefab does not contain a mesh renderer is not attached to marble prefab");
            return;
        }
        MarbleRenderer.material = marble.Team == MarbleTeam.Player ? playerMaterial : enemyMaterial;
    }
}
