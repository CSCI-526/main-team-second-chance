using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DiceMarble : MonoBehaviour
{
    [SerializeField] private int number = 1;
    [SerializeField] private Marble marbleRef;
    
    public void OnTriggerEnter(Collider other)
    {
        marbleRef.SetMarblePoints(marbleRef.Team, number);
        MarbleEvents.OnMarbleAbilityCasted(marbleRef,Ability.AbilityCastType.Collision);
    }
}
