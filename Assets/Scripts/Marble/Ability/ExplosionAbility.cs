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
        marble.Explode(radius, power);
    }
}
