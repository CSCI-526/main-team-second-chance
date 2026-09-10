using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

[CreateAssetMenu(fileName = "NewGhostAbility", menuName = "ScriptableObjects/Abilities/Ghost")]

public class GhostAbility : Ability
{
    [SerializeField] private float rematerializeTime = 1.0f;
    [SerializeField] private float ghostColor = 0.2f;
    private static readonly int MarbleColor = Shader.PropertyToID("_MarbleColor");
    private static readonly int OutlineColor = Shader.PropertyToID("_OutlineColor");

    public override void Cast(Marble marble)
    {
        marble.GetPhysicsCollider().excludeLayers = LayerMask.GetMask("MarblePhysics");
        MeshRenderer MarbleRenderer = marble.GetComponent<MeshRenderer>();
        MarbleRenderer.materials[0].SetColor(MarbleColor,GameManager.Instance.GetColorInfo().playerMarbleColor * ghostColor);
        MarbleRenderer.materials[0].renderQueue = (int)UnityEngine.Rendering.RenderQueue.Transparent;
        MarbleRenderer.materials[1].SetColor(OutlineColor,GameManager.Instance.GetColorInfo().playerOutlineColor * ghostColor);
    }
    
    public override Sequence SettledCast(Marble marble)
    {
        if (marble.timesCasted >= abilityMaxTriggers)
            return null;
        
        marble.GetPhysicsCollider().excludeLayers = 0;
        marble.SetMarbleTeam(marble.Team);
        Sequence rematerializeSequence = DOTween.Sequence();
        rematerializeSequence.AppendInterval(rematerializeTime * Time.timeScale);
        marble.timesCasted++;
        return rematerializeSequence;
    }
}
