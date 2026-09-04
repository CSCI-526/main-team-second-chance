using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

[CreateAssetMenu(fileName = "NewGhostAbility", menuName = "ScriptableObjects/Abilities/Ghost")]

public class GhostAbility : Ability
{
    [SerializeField] private float rematerializeTime = 1.0f;
    private Material[] materialCopies;
    
    public override void Cast(Marble marble)
    {
        marble.GetPhysicsCollider().excludeLayers = LayerMask.GetMask("MarblePhysics");
        MeshRenderer MarbleRenderer = marble.GetComponent<MeshRenderer>();
        materialCopies = MarbleRenderer.materials;
        Material[] outlineOnly = { materialCopies[1] };
        MarbleRenderer.materials = outlineOnly;
    }
    
    public override Sequence SettledCast(Marble marble)
    {
        if (marble.timesCasted >= abilityMaxTriggers)
            return null;
        
        marble.GetPhysicsCollider().excludeLayers = 0;
        MeshRenderer MarbleRenderer = marble.GetComponent<MeshRenderer>();
        MarbleRenderer.materials = materialCopies;
        Sequence rematerializeSequence = DOTween.Sequence();
        rematerializeSequence.AppendInterval(rematerializeTime * Time.timeScale);
        marble.timesCasted++;
        return rematerializeSequence;
    }
}
