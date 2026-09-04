using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

// To create new abilities, make a new script inheriting the Ability class, then generate a new ScriptableObject of said child
// Use "ExplosionAbility.cs" and "Explosion" scriptable object as an example
public class Ability : ScriptableObject
{
    public AudioInfo AbilitySound;
    [Range(0f, 10f)] public float abilityTriggerDelay = 0.75f;
    [SerializeField] public int abilityMaxTriggers = 1;
    
    public virtual void Cast(Marble marble)
    {
        Debug.Log("Ability Casted: DEFAULT");
    }

    public virtual void CollisionCast(Marble marble, Marble other)
    {
        Debug.Log("Collision Ability Casted: DEFAULT");
    }

    // returns a float for if the game should wait for the ability to finish
    public virtual Sequence SettledCast(Marble marble)
    {
        Debug.Log("Settle Ability Casted: DEFAULT");
        return null;
    }
}
