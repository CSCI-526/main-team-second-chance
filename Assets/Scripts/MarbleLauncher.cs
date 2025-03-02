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
    private void Awake()
    {
        if (ins == null)
            ins = this;
    }

    public void LaunchMarble(Vector3 Direction, float Force, Vector3 Location, MarbleTeam Team)
    {
        GameObject MarbleObject = Instantiate(Marble, Location, Quaternion.identity);
        
        
        MeshRenderer MarbleRenderer = MarbleObject.GetComponent<MeshRenderer>();
        MarbleRenderer.material = Team == MarbleTeam.Player ? playerMaterial : enemyMaterial;

        Marble MarbleIns = MarbleObject.GetComponent<Marble>();
        MarbleIns.Team = Team;
        
        Rigidbody MarbleRigidBody = MarbleObject.GetComponent<Rigidbody>();
        Direction *= LaunchForceScale;
        MarbleRigidBody.AddForce(Direction, ForceMode.Impulse);
    }
}
