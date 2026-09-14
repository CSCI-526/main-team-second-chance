using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class EnergyEvents
{
    public static event Action<int> OnEnergyUpdate;
    public static void OnEnergyUpdated(int energy)
    {
        OnEnergyUpdate?.Invoke(energy);
    }

    public static event Func<int, bool> OnEnergyCheck;
    public static bool? CheckValidEnergy(int energy)
    {
        return OnEnergyCheck?.Invoke(energy);
    }

    public static event Func<int, bool> OnEnergySpend;
    public static bool? SpendEnergy(int energy)
    {
        return OnEnergySpend?.Invoke(energy);
    }
}
