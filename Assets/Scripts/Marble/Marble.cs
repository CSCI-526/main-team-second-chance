using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum MarbleTeam
{
    Player,
    Enemy
}

public class Marble : MonoBehaviour
{
    [SerializeField]
    private MarbleData marbleData;

    private float maxSize = 2.0f;

    public ParticleSystem particleSystem;

    public MarbleData GetMarbleData() { return marbleData; }
    public string GetMarbleName() { return marbleData ? marbleData.MarbleName : "NULL MARBLE DATA"; }
    public string GetMarbleDescription() { return marbleData ? marbleData.MarbleDescription : "NULL MARBLE DATA"; }
    public bool bIsInsideGameplayCircle = true;
    public bool bIsInsideScoringCircle = false;
    public MarbleTeam Team;
    //public bool cool = false;

    public bool OneTimeCasted = false;

    private Rigidbody rb;
    private void Awake()
    {
        //If not already set in prefab, set Marble properities based on MarbleData
        if (marbleData != null)
        {
            rb = this.GetComponent<Rigidbody>();
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

    private void OnEnable()
    {
        OneTimeCasted = false;
        CastAbility();
    }

    public void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("Marble"))
        {
            Marble otherMarble = other.gameObject.GetComponent<Marble>();
            if (marbleData.AbilityObject != null && otherMarble != null)
            {
                marbleData.AbilityObject.CollisionCast(this, otherMarble);
            }
            AudioManager.TriggerSound(marbleData.CollisionSounds,transform.position);
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, 1.5f); //Temp debug render explosion radius
    }

    #region Abilities
    // Function call to start ability cast, can be hooked up to event/action later
    public void CastAbility()
    {
        if (marbleData.AbilityObject != null) StartCoroutine(AbilityCoroutine());
    }

    // Marble Abilities, called via polymorphic scriptable object abilities
    private IEnumerator AbilityCoroutine()
    {
        // Delay ability cast        
        yield return new WaitForSeconds(marbleData.abilityTriggerDelay);
        marbleData.AbilityObject.Cast(this);
        yield return null;
    }

    // returns time to wait before next round
    public float CastSettleAbility()
    {
        if (marbleData.AbilityObject != null)
        {
            return marbleData.AbilityObject.SettledCast(this);
        }
        return 0.0f;
    }

    // Explosive Marble Ability
    public void Explode(float radius, float power)
    {
        Vector3 explosionPos = transform.position;
        Collider[] colliders = Physics.OverlapSphere(explosionPos, radius);

        foreach (Collider hit in colliders)
        {
            Rigidbody rb = hit.GetComponent<Rigidbody>();

            if (rb != null && hit.CompareTag("Marble") && hit != this.GetComponent<SphereCollider>())
                rb.AddExplosionForce(power, explosionPos, radius, 0.0f, ForceMode.Impulse);
        }
    }

    public void Grow(float time, float scale)
    {
        StartCoroutine(GrowRoutine(time, scale));
    }

    IEnumerator GrowRoutine(float time, float scale)
    {
        var currentScale = this.gameObject.transform.localScale;
        if (this.gameObject.transform.localScale.x > maxSize)
            yield break;
        var finalScale = currentScale * scale;
        Vector3.ClampMagnitude(finalScale, maxSize);
        float startMass = rb.mass;
        float finalMass = startMass * Mathf.Pow(scale, 2.0f);
        float timer = 0.0f;
        while (timer < time)
        {
            var newScale = Vector3.Lerp(currentScale, finalScale, timer / time);
            this.gameObject.transform.localScale = newScale;

            rb.mass = Mathf.Lerp(startMass, finalMass, timer / time);
            yield return null;
            timer += Time.deltaTime;
        }
    }

    // ...and we put other abilities here vvv; probably should be a separate script, but this should suffice
    #endregion
}
