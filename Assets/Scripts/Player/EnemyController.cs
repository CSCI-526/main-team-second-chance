using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using Random = UnityEngine.Random;

public enum AggressionLevel
{
    Passive,
    Aggressive,
    HyperAggressive
}

public class EnemyController : MonoBehaviour
{
    [SerializeField] private float ForceRandomness = 0.1f;
    [SerializeField] private float DirectionRandomness = 0.1f;
    [SerializeField] private float CenterForce = 1.2f;
    [SerializeField] private float KnockoutForce = 5.0f;
    [SerializeField] private float KnockoutTargetRatio = 0.3f;
    [SerializeField] private int DeckSize = 10;
    private float SkillLevel = 1.0f;
    private AggressionLevel Aggression = AggressionLevel.HyperAggressive;

    public void SetAgression(AggressionLevel newLevel, float newSkill)
    {
        SkillLevel = newSkill;
        Aggression = newLevel;
    }

    private void Awake()
    {
        if (ins == null)
            ins = this;
    }

    public void ShootMarble(MarbleData MarbleObject)
    {
        Vector3 Direction = Vector3.zero;
        Vector3 Location = Vector3.zero;
        float Force = 0.0f;

        GameObject scoreZone = GameManager.Instance.GetScoringCircle();
        CapsuleCollider capsuleCollider = scoreZone.GetComponent<CapsuleCollider>();
        float colliderLength = capsuleCollider.radius;
        bool bTryToHitOut = false;

        Marble HitOut = null;
        Vector2 testPoint = Vector2.zero;
        Vector2 zoneCenter = Vector2.zero;
        float Rad = capsuleCollider.radius;
        if (Aggression >= AggressionLevel.Aggressive)
        {
            if (GameManager.Instance.GetMarblesList().Count > 0)
            {
                foreach (var Marble in GameManager.Instance.GetMarblesList())
                {
                if (!Marble)
                {
                    continue;
                }
                if (!Marble.gameObject.activeInHierarchy)
                {
                    continue;
                }
                    if (Marble.Team == MarbleTeam.Player)
                    {
                        testPoint = new Vector2(Marble.transform.position.x, Marble.transform.position.z);
                        zoneCenter = new Vector2(scoreZone.transform.position.x, scoreZone.transform.position.z);
                        float Mag = (testPoint - zoneCenter).magnitude;

                        if (Mag / Rad > KnockoutTargetRatio)
                        {
                            HitOut = Marble;
                            // we have to shoot at marbles
                            Vector3 HitOutMarbleLocation =
                                new Vector3(testPoint.normalized.x, 0.25f, testPoint.normalized.y);
                            Vector3 HitOutDirection = new Vector3(testPoint.normalized.x, 0.0f, testPoint.normalized.y);
                            Vector3 HitOutSpawnLocation = -1.2f * Rad * HitOutMarbleLocation;
                            HitOutSpawnLocation.y = 0.25f;


                            RaycastHit Hit;
                            bool bBlocked = Physics.SphereCast(HitOutSpawnLocation, 0.3f, HitOutDirection, out Hit,
                                Rad * 2.0f * 2f);
                            if (!bBlocked || (bBlocked && (Hit.collider.gameObject == HitOut.gameObject)))
                            {
                                Location = HitOutSpawnLocation;
                                Direction = HitOutDirection;
                                bTryToHitOut = true;
                                Force = KnockoutForce;
                                break;
                            }

                            if (Aggression >= AggressionLevel.HyperAggressive)
                            {
                                Quaternion Rotate = Quaternion.Euler(0.0f, 90.0f, 0.0f);
                                HitOutSpawnLocation = Rotate * HitOutSpawnLocation;
                                //HitOutSpawnLocation = HitOutMarbleLocation + -1.2f * Rad * HitOutDirection;
                                HitOutDirection = (new Vector3(testPoint.x, 0.25f, testPoint.y) - HitOutSpawnLocation);

                                bBlocked = Physics.SphereCast(HitOutSpawnLocation, 0.3f, HitOutDirection, out Hit,
                                    Rad * 2.0f * 2f);
                                if (!bBlocked || (bBlocked && (Hit.collider.gameObject == HitOut.gameObject)))
                                {
                                    Location = HitOutSpawnLocation;
                                    Direction = HitOutDirection + GenerateDirectionOffset();
                                    bTryToHitOut = true;
                                    float scale = Random.Range(1.0f, 1.0f + ForceRandomness * SkillLevel);
                                    Force = KnockoutForce * scale;
                                    break;
                                }
                            }
                        }
                    }
                }
            }
        }

        if (!bTryToHitOut)
        {
            float angle = Random.Range(0.0f, 360.0f);
            Location = (new Vector3(Mathf.Cos(angle), 0.0f, Mathf.Sin(angle)) * colliderLength) + GenerateDirectionOffset();
            Direction = capsuleCollider.center - Location + GenerateDirectionOffset();
            float scale = Random.Range(1.0f, 1.0f + ForceRandomness * SkillLevel);
            Force = scale * CenterForce;
        }
        MarbleEvents.MarbleReadyToLaunch(MarbleTeam.Enemy, MarbleObject.MarbleType, Direction, Force, Location, false);
    }

    private Vector3 GenerateDirectionOffset()
    {
        return new Vector3(Random.Range(-DirectionRandomness*SkillLevel, DirectionRandomness*SkillLevel), 0.0f,
            Random.Range(-DirectionRandomness*SkillLevel, DirectionRandomness*SkillLevel));
    }
}
