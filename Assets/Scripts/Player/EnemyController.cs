using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    // Start is called before the first frame update
    public static EnemyController ins = null;
    private void Awake()
    {
        if (ins == null)
            ins = this;
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
        Vector3 Direction;
        Vector3 Location;
        float Force;

        GameObject scoreZone = GameManager.Instance.GetScoringCircle();
        CapsuleCollider capsuleCollider = scoreZone.GetComponent<CapsuleCollider>();
        float colliderLength = capsuleCollider.radius;
        bool bTryToHitOut = false;

        Marble HitOut = null;
        if (GameManager.Instance.GetMarblesList().Count > 0)
        {
            foreach (var Marble in GameManager.Instance.GetMarblesList())
            {
                if (Marble.Team == MarbleTeam.Player)
                {
                    Vector2 testPoint = new Vector2(Marble.transform.position.x,Marble.transform.position.z);
                    Vector2 zoneCenter = new Vector2(scoreZone.transform.position.x, scoreZone.transform.position.z);
                    float sqrMag = Vector2.SqrMagnitude(testPoint - zoneCenter);
                    float sqrRad = (capsuleCollider.radius * capsuleCollider.radius);
                    if (sqrMag + 1.0f <= sqrRad)
                    {
                        bTryToHitOut = true;
                        HitOut = Marble;
                    }
                }
            }
        }

        if (bTryToHitOut)
        {
            // we have to shoot at marbles
            Location = Vector3.zero;
            Direction = Vector3.zero;
            Force = 1.0f;
        }
        else
        {
            float angle = Random.Range(0.0f, 360.0f);
            Location = (new Vector3(Mathf.Cos(angle), 0.0f,Mathf.Sin(angle)) * colliderLength) + new Vector3(Random.Range(0.0f,0.2f),Random.Range(0.0f,0.2f),Random.Range(0.0f,0.2f));
            Direction = capsuleCollider.center - Location + new Vector3(Random.Range(0.0f,0.2f),Random.Range(0.0f,0.2f),Random.Range(0.0f,0.2f));
            float scale = Random.Range(1.0f, 1.2f);
            Force = scale * 0.6f;
        }
        
        MarbleLauncher.ins.LaunchMarble(Direction, Force,Location,MarbleTeam.Enemy,GameManager.Instance.GetPlayerManager().GetPlayerDeck().UseMarble(MarbleTeam.Player));
    }
}
