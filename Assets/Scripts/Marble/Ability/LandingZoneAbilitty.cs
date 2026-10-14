using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
[CreateAssetMenu(menuName = "ScriptableObjects/Abilities/LandingZone")]
public class LandingZoneAbilitty : Ability
{
    public GameObject LaunchZonePrefab;
    [SerializeField] private float spawnTime = 1.5f;

    public override Sequence SettledCast(Marble marble)
    {
        if (marble.timesCasted >= abilityMaxTriggers)
        {
            return null;
        }

        marble.timesCasted++;

        Vector3 pos = marble.transform.position;
        pos.y = 0.005f;

        GameManager.Instance.GetScoringZoneManager().AddScoringZone(Instantiate(LaunchZonePrefab, pos, Quaternion.identity));

        return WaitForSecondsSequence(spawnTime);
    }
}
