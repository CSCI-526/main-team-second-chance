using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// To create new abilities, make a new script inheriting the Ability class, then generate a new ScriptableObject of said child
// Use "ExplosionAbility.cs" and "Explosion" scriptable object as an example
public class Ability : ScriptableObject
{
    public AudioInfo AbilitySound;
    
    public virtual void Cast(Marble marble)
    {
        Debug.Log("Ability Casted: DEFAULT");
    }

    public virtual void CollisionCast(Marble marble, Marble other)
    {
        Debug.Log("Collision Ability Casted: DEFAULT");
    }

    // returns a float for if the game should wait for the ability to finish
    public virtual float SettledCast(Marble marble)
    {
        Debug.Log("Settle Ability Casted: DEFAULT");
        return 0.0f;
    }
}
