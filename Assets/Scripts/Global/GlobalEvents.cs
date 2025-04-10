using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GlobalEvents
{
    public static event Action OnLevelLoadedIn;
    public static void LevelLoadedIn()
    {
        OnLevelLoadedIn?.Invoke();
    }
}
