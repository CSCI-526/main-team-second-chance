using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public static class MarbleEvents
{
    // some stuff 
    public static event Action OnMarbleSpawned;
    public static void OnMarbleSpawn()
    {
        OnMarbleSpawned?.Invoke();
    }
}
