using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HealthBarUI : MonoBehaviour
{
    [SerializeField] private Slider healthBar;
    [SerializeField] private TextMeshProUGUI healthText;
    [SerializeField] private MarbleTeam marbleTeam;

    private void Start()
    {
        if (!GameManager.UseCombatSystem)
        {
            gameObject.SetActive(false);
        }
    }

    private void OnEnable()
    {
        HealthEvents.OnHealthUpdate += UpdateHealth;
    }

    private void OnDisable()
    {
        HealthEvents.OnHealthUpdate -= UpdateHealth;
    }

    private void UpdateHealth(int curHealth, int maxHealth, MarbleTeam team)
    {
        if (team == marbleTeam)
        {
            healthBar.value = (float)curHealth / maxHealth;
            healthText.text = curHealth + "/" + maxHealth;
            Sequence jump = transform.DOJump(transform.position, 30.0f, 1, 0.3f * Time.timeScale);
        }
    }
}
