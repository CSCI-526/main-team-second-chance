using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MarbleLauncher : MonoBehaviour
{
    public static MarbleLauncher ins = null;
    private void Awake()
    {
        if (ins == null)
            ins = this;
    }

    public void LaunchMarble(Vector2 Direction)
    {
        
    }
}
