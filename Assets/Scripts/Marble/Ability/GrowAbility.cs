using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewSplitAbility", menuName = "ScriptableObjects/Abilities/Grow")]

public class GrowAbility : Ability
{
    [SerializeField] private float GrowScale = 1.5f;
    [SerializeField] private float GrowTime = 0.5f;
    
    public override float SettledCast(Marble marble)
    {
        marble.Grow(GrowTime,GrowScale);
        return GrowTime;
    }
}
