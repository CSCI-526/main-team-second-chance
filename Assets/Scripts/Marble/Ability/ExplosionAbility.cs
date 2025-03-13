using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewExplosionAbility", menuName = "ScriptableObjects/Abilities/Explosion")]
public class ExplosionAbility : Ability
{
    [SerializeField] private float radius = 1.5f;
    [SerializeField] private float power = 10.0f;
    public override void Cast(Marble marble)
    {
        Debug.Log("Ability Casted: EXPLODE");
        if (marble == null) return;
        Explode(marble, radius, power);
    }

    // Explosive Marble Ability
    private void Explode(Marble marble, float r, float p)
    {
        Vector3 explosionPos = marble.gameObject.transform.position;
        Collider[] colliders = Physics.OverlapSphere(explosionPos, r);

        foreach (Collider hit in colliders)
        {
            Rigidbody rb = hit.GetComponent<Rigidbody>();

            if (rb != null && hit.CompareTag("Marble") && hit != marble.GetComponent<SphereCollider>())
                rb.AddExplosionForce(p, explosionPos, r, 0.0f, ForceMode.Impulse);
        }
    }
}
