using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Billboard : MonoBehaviour
{
    [SerializeField] private Vector3 fixedAngle;
    void FixedUpdate()
    {
        transform.rotation = Quaternion.Euler(fixedAngle);
    }
}
