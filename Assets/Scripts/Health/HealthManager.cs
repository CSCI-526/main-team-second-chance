using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthManager : MonoBehaviour
{
    [SerializeField] private MarbleTeam marbleTeam;
    private int _maxHealth = 0;
    private int _currentHealth = 0;

    public int GetCurHealth()
    {
        return _currentHealth;
    }

    public void SetHealth(int newCur, int newMax)
    {
        _currentHealth = newCur;
        _maxHealth = newMax;
        HealthEvents.OnHealthUpdated(_currentHealth,_maxHealth,marbleTeam);
    }
    
    public void TakeDamage(int damage)
    {
        _currentHealth -= damage;
        HealthEvents.OnDamaged(damage, marbleTeam);
        HealthEvents.OnHealthUpdated(_currentHealth,_maxHealth,marbleTeam);

        if (marbleTeam == MarbleTeam.Player)
        {
            NodeManager.Instance.SetPlayerHealth(_currentHealth);
        }
        
        if (_currentHealth <= 0)
        {
            HealthEvents.OnKilled(marbleTeam);
        }
    }
}
