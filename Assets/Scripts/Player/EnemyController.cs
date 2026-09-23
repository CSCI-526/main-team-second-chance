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
    public static EnemyController ins = null;
    private Deck EnemyDeck;

    [SerializeField] private float ForceRandomness = 0.1f;
    [SerializeField] private float DirectionRandomness = 0.1f;
    [SerializeField] private float CenterForce = 1.2f;
    [SerializeField] private float KnockoutForce = 5.0f;
    [SerializeField] private float KnockoutTargetRatio = 0.3f;
    [SerializeField] private float ZoneSpacingBuffer = 0.2f;
    [SerializeField] private int LaunchLocationAttempts = 8;
    private float SkillLevel = 1.0f;
    private AggressionLevel Aggression = AggressionLevel.HyperAggressive;

    public void SetAggression(AggressionLevel newLevel, float newSkill)
    {
        SkillLevel = newSkill;
        Aggression = newLevel;
    }

    public void ShootMarble(MarbleData MarbleObject)
    {
        Vector3 Direction = Vector3.zero;
        Vector3 Location = Vector3.zero;
        float Force = 0.0f;
        
        bool bTryToHitOut = false;


        if (Aggression >= AggressionLevel.Aggressive)
        {
            bTryToHitOut = CalculateKnockoutLaunch(ref Location, ref Direction, ref Force);
        }
        /*
        ScoringZoneManager scoreZone = GameManager.Instance.GetScoringZoneManager();
        CapsuleCollider capsuleCollider = (CapsuleCollider)scoreZone.GetDefaultScoringZone();
           Marble HitOut = null;
           float colliderLength = capsuleCollider.radius;
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
                                Rad * 2.0f * 2f,LayerMask.GetMask("MarblePhysics","Terrain"));
                            if (!bBlocked || ((Hit.collider.gameObject == HitOut.gameObject)))
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
                                    Rad * 2.0f * 2f,LayerMask.GetMask("MarblePhysics","Terrain"));
                                if (!bBlocked || ((Hit.collider.gameObject == HitOut.gameObject)))
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
        */

        if (!bTryToHitOut)
        {
            CalculatePassiveLaunch(ref Location, ref Direction, ref Force);
        }

        Debug.DrawRay(Location, Direction,Color.red,5.0f,false);
        
        MarbleEvents.MarbleReadyToLaunch(MarbleTeam.Enemy, MarbleObject, Direction, Force, Location, false);
    }

    private bool CalculateKnockoutLaunch(ref Vector3 location, ref Vector3 direction, ref float force)
    {
        if (GameManager.Instance.GetMarblesList().Count <= 0)
        {
            return false;
        }

        ScoringZoneManager scoreZone = GameManager.Instance.GetScoringZoneManager();
        foreach (var marble in GameManager.Instance.GetMarblesList())
        {
            if(marble.Team == MarbleTeam.Enemy || !marble.isActiveAndEnabled)
                continue;
            ScoringCircle scoringCircle = scoreZone.GetHighestPriorityCircle(marble);
            if(scoringCircle == null)
                continue;
            float scoringRadius = scoringCircle.GetScoringCollider().radius;
            List<Vector3> possibleLocations = GenerateValidLaunchPositions(marble, scoringCircle, LaunchLocationAttempts,
                scoringRadius + ZoneSpacingBuffer);
            foreach (Vector3 launchPosition in possibleLocations)
            {
                float ratioOfPositionFromCenter = (marble.transform.position - scoreZone.transform.position).magnitude / scoringRadius;

                if (ratioOfPositionFromCenter > KnockoutTargetRatio)
                {
                    // we have to shoot at marbles
                    Vector3 hitOutDirection = marble.transform.position - launchPosition;
                    hitOutDirection.y = 0.0f;
                    hitOutDirection.Normalize();

                    bool bBlocked = Physics.SphereCast(launchPosition, 0.3f, hitOutDirection, out var hit,
                        scoringRadius * 4f, LayerMask.GetMask("MarblePhysics", "Terrain"));
                    if (!bBlocked || ((hit.collider.gameObject == marble.gameObject)))
                    {
                        location = launchPosition;
                        direction = hitOutDirection + GenerateDirectionOffset();
                        float scale = Random.Range(1.0f, 1.0f + ForceRandomness * SkillLevel);
                        force = KnockoutForce * scale;
                        DebugDrawX(marble.transform.position,Color.green);
                        return true;
                    }
                }
            }
        }

        return false;
    }

    private bool CalculatePassiveLaunch(ref Vector3 location, ref Vector3 direction, ref float force)
    {
        ScoringZoneManager scoreZone = GameManager.Instance.GetScoringZoneManager();
        CapsuleCollider capsuleCollider = (CapsuleCollider)scoreZone.GetDefaultScoringZone();
        float radius = capsuleCollider.radius;
        var rand = Random.insideUnitCircle;
        var position = capsuleCollider.transform.position + new Vector3(radius * rand.x,0.0f,radius * rand.y) * 0.95f;
        while (!scoreZone.CheckInScoringZone(position))
        {
            rand = Random.insideUnitCircle;
            position = capsuleCollider.transform.position + new Vector3(radius * rand.x,0.0f,radius * rand.y) * 0.95f;
        }
        List<Vector3> launchPositions = GenerateValidLaunchPositions(position, LaunchLocationAttempts,
            radius + ZoneSpacingBuffer);

        location = launchPositions[Random.Range(0, launchPositions.Count)];
        float dist = Vector3.Distance(location, position);
        float forceScaling = dist / 3.0f;
        direction = position - location + GenerateDirectionOffset();
        float scale = Random.Range(1.0f, 1.0f + ForceRandomness * SkillLevel);
        force = scale * CenterForce * forceScaling;
        
        return false;
    }

    private Vector3 GenerateDirectionOffset()
    {
        return new Vector3(Random.Range(-DirectionRandomness * SkillLevel, DirectionRandomness * SkillLevel), 0.0f,
            Random.Range(-DirectionRandomness * SkillLevel, DirectionRandomness * SkillLevel));
    }

    private List<Vector3> GenerateValidLaunchPositions(Marble target, ScoringCircle scoringCircle, int count, float radius)
    {
        List<Vector3> validPositions = new List<Vector3>();
        float angleBetween = 360.0f / count;
        ScoringZoneManager zoneManager = GameManager.Instance.GetScoringZoneManager();
        Vector3 marbleAngle = target.transform.position - scoringCircle.transform.position;
        marbleAngle.y = 0.0f;
        
        for (int i = 0; i < count; ++i)
        {
            Quaternion rotation = Quaternion.Euler(0.0f,i * angleBetween + 180.0f,0.0f);
            Vector3 objectSpace = rotation * marbleAngle * radius;
            Vector3 testLocation = target.transform.position + objectSpace;
            if (zoneManager.CheckValidLaunchZone(testLocation))
            {
                DebugDrawX(testLocation,Color.blue);
                validPositions.Add(testLocation);
                continue;
            }
            DebugDrawX(testLocation,Color.red);
            
            testLocation = target.transform.position + objectSpace * 2.0f;
            if (zoneManager.CheckValidLaunchZone(testLocation))
            {
                DebugDrawX(testLocation,Color.blue);
                validPositions.Add(testLocation);
                continue;
            }
            DebugDrawX(testLocation,Color.red);

            testLocation = target.transform.position + objectSpace * 0.5f;
            if (zoneManager.CheckValidLaunchZone(testLocation))
            {
                DebugDrawX(testLocation,Color.blue);
                validPositions.Add(testLocation);
            }
            else
            {
                DebugDrawX(testLocation,Color.red);
            }
        }


        return validPositions;
    }
    
    private List<Vector3> GenerateValidLaunchPositions(Vector3 target, int count, float radius)
    {
        List<Vector3> validPositions = new List<Vector3>();
        float angleBetween = Mathf.PI * 2.0f / count;
        for (int i = 0; i < count; ++i)
        {
            Vector3 objectSpace = new Vector3(Mathf.Cos(angleBetween * i), 0.0f, Mathf.Sin(angleBetween * i)) * radius;
            Vector3 testLocation = target + objectSpace;
            if (GameManager.Instance.GetScoringZoneManager().CheckValidLaunchZone(testLocation))
            {
                validPositions.Add(testLocation);
                continue;
            }
            

            testLocation = target + objectSpace * 2.0f;
            if (GameManager.Instance.GetScoringZoneManager().CheckValidLaunchZone(testLocation))
            {
                validPositions.Add(testLocation);
                continue;
            }
            
            testLocation = target + objectSpace * 0.5f;
            if (GameManager.Instance.GetScoringZoneManager().CheckValidLaunchZone(testLocation))
            {
                validPositions.Add(testLocation);
            }
        }

        return validPositions;
    }

    private void DebugDrawX(Vector3 position, Color color)
    {
        Debug.DrawLine(position + new Vector3(-0.25f, 0.0f, -0.25f), position + new Vector3(0.25f, 0.0f, 0.25f), color,5.0f, false);
        Debug.DrawLine(position + new Vector3(-0.25f, 0.0f, 0.25f), position + new Vector3(0.25f, 0.0f, -0.25f), color,5.0f, false);
    }
}
