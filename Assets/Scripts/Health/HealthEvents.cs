using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class HealthEvents
{
    public static event Action<int, int, MarbleTeam> OnHealthUpdate;
    public static void OnHealthUpdated(int curHealth, int maxHealth, MarbleTeam team)
    {
        OnHealthUpdate?.Invoke(curHealth,maxHealth,team);
    }

    public static event Action<MarbleTeam> OnKill;
    public static void OnKilled(MarbleTeam team)
    {
        OnKill?.Invoke(team);
    }

    public static event Action<int, MarbleTeam> OnDamage;
    public static void OnDamaged(int damage, MarbleTeam team)
    {
        OnDamage?.Invoke(damage,team);
    }
}
