using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewBlackHoleAbility", menuName = "ScriptableObjects/Abilities/BlackHole")]
public class BlackHoleAbility : Ability
{
    [SerializeField] private float radius = 1.5f;
    [SerializeField] private float power = 2.0f;
    public override void Cast(Marble marble)
    {
        Debug.Log("Ability Casted: BLACK HOLE");
        if (marble == null) return;
        BlackHole(marble, radius, power);
    }

    private void BlackHole(Marble marble, float r, float p)
    {
        Vector3 castPos = marble.gameObject.transform.position;
        Collider[] colliders = Physics.OverlapSphere(castPos, radius);

        foreach (Collider hit in colliders)
        {
            Rigidbody rb = hit.GetComponent<Rigidbody>();

            if (rb != null && hit.CompareTag("Marble") && hit != marble.GetComponent<SphereCollider>())
            {
                Vector3 otherPos = rb.gameObject.transform.position;

                Vector3 direction = new Vector3(castPos.x - otherPos.x, castPos.y - otherPos.y, castPos.z - otherPos.z);
                //rb.AddExplosionForce(-power, explosionPos, radius, 0.0f, ForceMode.Impulse);
                Debug.Log("ADDED BLACK HOLE FORCE");
                rb.AddForce(direction * power, ForceMode.Impulse);
            }
        }

        marble.GetComponentInChildren<ParticleSystem>().Play();
    }
}