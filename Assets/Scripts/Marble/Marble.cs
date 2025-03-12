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
    public string GetMarbleName() { return marbleData.MarbleName; }
    public string GetMarbleDescription() { return marbleData.MarbleDescription; }
    public void SetUniqueID(int ID) { UNIQUE_ID = ID; }
    public int GetUniqueID() { return UNIQUE_ID; }
    public MarbleTeam Team;
    public bool bIsInsideGameplayCircle = true;
    public bool bIsInsideScoringCircle = false;
    //public bool cool = false;

    private Rigidbody rb;
    private int UNIQUE_ID;
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

    private void Start()
    {
        // For now, cast ability once marble is instantiated
        CastAbility();
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

    // ...and we put other abilities here vvv; probably should be a separate script, but this should suffice
    #endregion
}
