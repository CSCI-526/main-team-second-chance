using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// To create new abilities, make a new script inheriting the Ability class, then generate a new ScriptableObject of said child
// Use "ExplosionAbility.cs" and "Explosion" scriptable object as an example
public class Ability : ScriptableObject
{
    public virtual void Cast(Marble marble)
    {
        Debug.Log("Ability Casted: DEFAULT");
    }
}
