using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MarbleLauncher : MonoBehaviour
{
    public static MarbleLauncher ins = null;
    [SerializeField] private GameObject Marble;
    [SerializeField] private float LaunchForceScale = 1.0f;
    [SerializeField] private Material playerMaterial;
    [SerializeField] private Material enemyMaterial;

    private List<Marble> ActiveGameplayMarbles = new List<Marble>();
    private bool bIsMarblesMoving = false;
    
    private void Awake()
    {
        if (ins == null)
            ins = this;
    }

    public void LaunchMarble(Vector3 Direction, float Force, Vector3 Location, MarbleTeam Team)
    {
        if (bIsMarblesMoving)
            return;
        Location.y = 0.25f;
        GameObject MarbleObject = Instantiate(Marble, Location, Quaternion.identity);
        
        
        MeshRenderer MarbleRenderer = MarbleObject.GetComponent<MeshRenderer>();
        MarbleRenderer.material = Team == MarbleTeam.Player ? playerMaterial : enemyMaterial;

        Marble MarbleIns = MarbleObject.GetComponent<Marble>();
        MarbleIns.Team = Team;
        ActiveGameplayMarbles.Add(MarbleIns);
        
        Rigidbody MarbleRigidBody = MarbleObject.GetComponent<Rigidbody>();
        Direction *= LaunchForceScale * Force;
        MarbleRigidBody.AddForce(Direction, ForceMode.Impulse);
        StartCoroutine(WaitForMarblesToSettle());
    }

    private IEnumerator WaitForMarblesToSettle()
    {
        bIsMarblesMoving = true;
        yield return new WaitForSeconds(1.0f);

        
        while (true)
        {
            bool bMarblesSettled = true;
            foreach(Marble marble in ActiveGameplayMarbles)
            {
                if (marble.bIsInsideGameplayCircle)
                {
                    Rigidbody physics = marble.GetComponent<Rigidbody>();
                    if (physics.velocity.sqrMagnitude > 0.1f)
                    {
                        bMarblesSettled = false;
                    }
                }
            }

            if (bMarblesSettled)
            {
                break;
            }

            yield return new WaitForSeconds(2.0f);
        }
            
        yield return new WaitForSeconds(1.0f);
        bIsMarblesMoving = false;

        List<Marble> MarblesToDelete = new List<Marble>();
        foreach (Marble marble in ActiveGameplayMarbles)
        {
            if (!marble.bIsInsideScoringCircle)
            {
                MarblesToDelete.Add(marble);
            }
        }

        foreach (Marble marble in MarblesToDelete)
        {
            ActiveGameplayMarbles.Remove(marble);
            Destroy(marble.gameObject);
        }

        yield return null;
    }
}
