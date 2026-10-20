using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

[CreateAssetMenu(fileName = "NewBlackHoleAbility", menuName = "ScriptableObjects/Abilities/BlackHole")]
public class BlackHoleAbility : Ability
{
    [SerializeField] private float radius = 1.5f;
    [SerializeField] private float power = 2.0f;
    public override void Cast(Marble marble)
    {
        Debug.Log("Ability Casted: BLACK HOLE");
        BlackHole(marble, radius, power);
    }

    private void BlackHole(Marble marble, float r, float p)
    {
        Vector3 castPos = marble.gameObject.transform.position;
        Collider[] colliders = Physics.OverlapSphere(castPos, radius, LayerMask.GetMask("MarblePhysics"));
        marble.GetMarbleRigidbody().mass *= 4.0f;
        DOVirtual.DelayedCall(2.0f, () => { marble.GetMarbleRigidbody().mass *= 0.25f; }, false);
        foreach (Collider hit in colliders)
        {
            Rigidbody rb = hit.GetComponent<Rigidbody>();

            if (rb != null && hit.CompareTag("Marble") && hit != marble.GetPhysicsCollider())
            {
                Vector3 otherPos = rb.gameObject.transform.position;

                Vector3 direction = new Vector3(castPos.x - otherPos.x, castPos.y - otherPos.y, castPos.z - otherPos.z);
                //rb.AddExplosionForce(-power, explosionPos, radius, 0.0f, ForceMode.Impulse);
                Debug.Log("ADDED BLACK HOLE FORCE");
                rb.AddForce(direction * power, ForceMode.Impulse);
            }
        }

        marble.marbleParticleSystem.Play();
        AudioManager.TriggerSound(AbilitySound,marble.transform.position);
        marble.timesCasted++;
    }
}