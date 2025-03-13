using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using Random = UnityEngine.Random;

public class EnemyController : MonoBehaviour
{
    // Start is called before the first frame update
    public static EnemyController ins = null;
    private Deck EnemyDeck;

    [SerializeField] private float ForceRandomness = 0.1f;
    [SerializeField] private float DirectionRandomness = 0.1f;
    [SerializeField] private float CenterForce = 0.6f;
    [SerializeField] private float KnockoutForce = 5.0f;
    [SerializeField] private float KnockoutTargetRatio = 0.3f;
    public float SkillLevel = 1.0f;
    private void Awake()
    {
        if (ins == null)
            ins = this;
    }

    public void Start()
    {
        EnemyDeck = GetComponent<Deck>();
        EnemyDeck.InitializeDeck(MarbleTeam.Enemy, 10);
    }

    IEnumerator MarbleRepeater()
    {
        while (true)
        {
            yield return new WaitForSeconds(0.5f);
            ShootMarble();
            yield return new WaitForSeconds(2.0f);
        }
    }

    public void ShootMarble()
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
        if (GameManager.Instance.GetMarblesList().Count > 0)
        {
            foreach (var Marble in GameManager.Instance.GetMarblesList())
            {
                if (Marble.Team == MarbleTeam.Player)
                {
                    testPoint = new Vector2(Marble.transform.position.x,Marble.transform.position.z);
                    zoneCenter = new Vector2(scoreZone.transform.position.x, scoreZone.transform.position.z);
                    float Mag = (testPoint - zoneCenter).magnitude;
                    
                    if (Mag / Rad > KnockoutTargetRatio)
                    {
                        HitOut = Marble;
                        // we have to shoot at marbles
                        Vector3 HitOutMarbleLocation = new Vector3(testPoint.normalized.x, 0.25f, testPoint.normalized.y);
                        Vector3 HitOutDirection = new Vector3(testPoint.normalized.x, 0.0f, testPoint.normalized.y);
                        Vector3 HitOutSpawnLocation = -1.2f * Rad * HitOutMarbleLocation;
                        HitOutSpawnLocation.y = 0.25f;


                        RaycastHit Hit;
                        bool bBlocked = Physics.SphereCast(HitOutSpawnLocation,0.3f, HitOutDirection, out Hit, Rad * 2.0f * 2f);
                        if (!bBlocked || (bBlocked && (Hit.collider.gameObject == HitOut.gameObject)))
                        {
                            Location = HitOutSpawnLocation;
                            Direction = HitOutDirection;
                            bTryToHitOut = true;
                            Force = KnockoutForce;
                            break;
                        }

                        Quaternion Rotate = Quaternion.Euler(0.0f,90.0f,0.0f);
                        HitOutSpawnLocation = Rotate * HitOutSpawnLocation;
                        //HitOutSpawnLocation = HitOutMarbleLocation + -1.2f * Rad * HitOutDirection;
                        HitOutDirection = (new Vector3(testPoint.x, 0.25f, testPoint.y) - HitOutSpawnLocation);
                        
                        bBlocked = Physics.SphereCast(HitOutSpawnLocation,0.3f, HitOutDirection, out Hit, Rad * 2.0f * 2f);
                        if (!bBlocked || (bBlocked && (Hit.collider.gameObject == HitOut.gameObject)))
                        {
                            Location = HitOutSpawnLocation;
                            Direction = HitOutDirection;
                            bTryToHitOut = true;
                            Force = KnockoutForce;
                            break;
                        }
                    }
                }
            }
        }

        if(!bTryToHitOut)
        {
            float angle = Random.Range(0.0f, 360.0f);
            Location = (new Vector3(Mathf.Cos(angle), 0.0f,Mathf.Sin(angle)) * colliderLength) + new Vector3(Random.Range(-DirectionRandomness,DirectionRandomness),0.0f,Random.Range(-DirectionRandomness,DirectionRandomness));
            Direction = capsuleCollider.center - Location + new Vector3(Random.Range(-DirectionRandomness,DirectionRandomness),0.0f,Random.Range(-DirectionRandomness,DirectionRandomness));
            float scale = Random.Range(1.0f, 1.0f+ForceRandomness);
            Force = scale * CenterForce;
        }
        
        MarbleLauncher.ins.LaunchMarble(Direction.normalized, Force,Location,MarbleTeam.Enemy,EnemyDeck.UseMarble(MarbleTeam.Enemy));
    }
}
