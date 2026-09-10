using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

[CreateAssetMenu(fileName = "NewGrowAbility", menuName = "ScriptableObjects/Abilities/Grow")]

public class GrowAbility : Ability
{
    [SerializeField] private float GrowScale = 1.5f;
    [SerializeField] private float GrowTime = 0.75f;
    public override Sequence SettledCast(Marble marble)
    {
        if (marble.timesCasted >= abilityMaxTriggers)
            return null;
        
        marble.timesCasted++;
        AudioManager.TriggerSound(AbilitySound,marble.transform.position);
        var currentScale = marble.transform.localScale;

        var finalScale = currentScale * GrowScale;

        float startMass = marble.GetMarbleRigidbody().mass;
        float finalMass = startMass * Mathf.Pow(GrowScale, 2.0f);
        float t = 0.0f;

        Sequence growSequence = DOTween.Sequence();
        growSequence.Append(
            DOTween.To(() => t, x =>
            {
                t = x;
                var newScale = Vector3.Lerp(currentScale, finalScale, t);
                marble.transform.localScale = newScale;
                marble.GetMarbleRigidbody().mass = Mathf.Lerp(startMass, finalMass, t);

            }, 1.0f, GrowTime * Time.timeScale));
        
        return growSequence;
    }
}
