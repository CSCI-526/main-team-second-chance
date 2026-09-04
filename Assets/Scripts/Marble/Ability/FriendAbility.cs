using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

[CreateAssetMenu(fileName = "NewFriendAbility", menuName = "ScriptableObjects/Abilities/Friend")]
public class FriendAbility : Ability
{
    [SerializeField] private float radius = 1.5f;

    public override Sequence SettledCast (Marble marble)
    {
        Debug.Log("Ability Casted: FRIEND");
        if (marble == null) return null;

        Vector3 marblePos = marble.gameObject.transform.position;
        //Collider[] colliders = Physics.OverlapSphere(explosionPos, r);

        //Enumerate between all marbles in scene?


        //Use raycasts?
        RaycastHit[] raycasts = Physics.SphereCastAll(marblePos, radius, Vector3.zero, 0.0f);

        foreach (RaycastHit hit in raycasts)
        {
            //Do something with raycasts?
            //Rigidbody rb = hit.GetComponent<Rigidbody>();

            //if (rb != null && hit.CompareTag("Marble") && hit != marble.GetComponent<SphereCollider>())
            //    return 0.0f;
        }

        //Make friend shaped particles? Need to set up
        //marble.GetComponentInChildren<ParticleSystem>().Play();
        AudioManager.TriggerSound(AbilitySound, marble.transform.position);
        return null;
    }

}
