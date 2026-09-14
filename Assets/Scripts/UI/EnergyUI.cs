using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class EnergyUI : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI energyText;
    
    private void Awake()
    {
        gameObject.SetActive(GameManager.UseEnergy);
    }

    private void OnEnable()
    {
        EnergyEvents.OnEnergyUpdate += OnEnergyUpdate;
    }

    private void OnDisable()
    {
        EnergyEvents.OnEnergyUpdate -= OnEnergyUpdate;
    }

    private void OnEnergyUpdate(int energy)
    {
        energyText.text = $"{energy}/{3} Energy";
    }
}
