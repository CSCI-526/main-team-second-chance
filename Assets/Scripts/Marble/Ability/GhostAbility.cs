using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewGhostAbility", menuName = "ScriptableObjects/Abilities/Ghost")]

public class GhostAbility : Ability
{
    private Material[] materialCopies;
    
    public override void Cast(Marble marble)
    {
        marble.GetPhysicsCollider().excludeLayers = LayerMask.GetMask("MarblePhysics");
        MeshRenderer MarbleRenderer = marble.GetComponent<MeshRenderer>();
        materialCopies = MarbleRenderer.materials;
        Material[] outlineOnly = { materialCopies[1] };
        MarbleRenderer.materials = outlineOnly;
    }
    
    public override float SettledCast(Marble marble)
    {
        marble.GetPhysicsCollider().excludeLayers = 0;
        MeshRenderer MarbleRenderer = marble.GetComponent<MeshRenderer>();
        MarbleRenderer.materials = materialCopies;
        return 1.0f;
    }
}
