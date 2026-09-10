using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public enum MarbleTeam
{
    Player,
    Enemy
}

public class Marble : MonoBehaviour
{
    private static readonly int MarbleColor = Shader.PropertyToID("_MarbleColor");
    private static readonly int OutlineColor = Shader.PropertyToID("_OutlineColor");
    
    [SerializeField]
    private MarbleData marbleData;

    public ParticleSystem particleSystem;
    [SerializeField] private Collider scoringCollider;
    [SerializeField] private Collider physicsCollider;
    [SerializeField] private SpriteRenderer marbleImage;
    public MarbleData GetMarbleData() { return marbleData; }

    public Collider GetScoringCollider() { return scoringCollider; }

    public Collider GetPhysicsCollider() { return physicsCollider; }

    public Rigidbody GetMarbleRigidbody() { return rb; }
    
    public string GetMarbleName() { return marbleData ? marbleData.MarbleName : "NULL MARBLE DATA"; }
    public string GetMarbleDescription() { return marbleData ? marbleData.MarbleDescription : "NULL MARBLE DATA"; }
    public bool bIsInsideGameplayCircle = true;
    public bool bIsInsideScoringCircle = false;
    public MarbleTeam Team;
    //public bool cool = false;

    public int timesCasted = 0;

    private Rigidbody rb;
    private void Awake()
    {
        //If not already set in prefab, set Marble properities based on MarbleData
        if (marbleData != null)
        {
            rb = GetComponent<Rigidbody>();
            var currentScale = this.gameObject.transform.localScale;

            this.gameObject.transform.localScale = new Vector3(marbleData.UniformScale, marbleData.UniformScale, marbleData.UniformScale);
            rb.mass = marbleData.Mass;
            rb.drag = marbleData.Drag;
        }
    }

    void FixedUpdate()
    {
        if (particleSystem != null)
        {
            particleSystem.transform.rotation = Quaternion.Euler(0, 0, 0);
        }
    }

    public void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("Marble"))
        {
            Marble otherMarble = other.gameObject.GetComponent<Marble>();
            if (marbleData.AbilityObject != null && otherMarble != null)
            {
                marbleData.AbilityObject.CollisionCast(this, otherMarble);
                MarbleEvents.OnMarbleAbilityCasted(this);
            }
            AudioManager.TriggerSound(marbleData.CollisionSounds,transform.position);
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
    }

    #region Abilities
    // Function call to start ability cast, can be hooked up to event/action later
    public void CastAbility()
    {
        // Marble Abilities, called via polymorphic scriptable object abilities
        if (marbleData.AbilityObject != null) 
            DOVirtual.DelayedCall(marbleData.AbilityObject.abilityTriggerDelay * Time.timeScale, () =>
            {
                marbleData.AbilityObject.Cast(this);
                MarbleEvents.OnMarbleAbilityCasted(this);
            },false);
    }

    // returns time to wait before next round
    public Sequence CastSettleAbility()
    {
        if (marbleData.AbilityObject != null)
        {
            return marbleData.AbilityObject.SettledCast(this);
        }
        return null;
    }

    // ...and we put other abilities here vvv; probably should be a separate script, but this should suffice
    #endregion

    public void SetMarbleTeam(MarbleTeam team)
    {
        Team = team;
        
        MeshRenderer MarbleRenderer = GetComponent<MeshRenderer>();
        if (!MarbleRenderer)
        {
            Debug.LogError("MarbleLauncher.LaunchMarble(): Prefab does not contain a mesh renderer is not attached to marble prefab");
            return;
        }

        ColorInfo colorInfo = GameManager.Instance.GetColorInfo();
        MarbleRenderer.materials[0].SetColor(MarbleColor, team == MarbleTeam.Player ? colorInfo.playerMarbleColor : colorInfo.enemyMarbleColor);
        MarbleRenderer.materials[0].renderQueue = (int)UnityEngine.Rendering.RenderQueue.Transparent;
        MarbleRenderer.materials[1].SetColor(OutlineColor, team == MarbleTeam.Player ? colorInfo.playerOutlineColor : colorInfo.enemyOutlineColor);
    }

    public void SetMarbleSprite(Sprite sprite)
    {
        marbleImage.sprite = sprite;
        if (sprite != null)
        {
            float scale = 0.25f * 256.0f / sprite.rect.size.y;
            marbleImage.transform.localScale = new Vector3(scale, scale, scale);
        }
    }
}
