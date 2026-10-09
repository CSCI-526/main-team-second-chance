using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlackholeMarble : MonoBehaviour
{
    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.CompareTag("Marble"))
        {
            Vector3 dir = transform.position - other.transform.position;
            dir.Normalize();
            Vector3 offset = Vector3.Cross(dir, Vector3.up);
            other.attachedRigidbody.AddForce((dir + 0.5f * offset) * 9.8f * Time.deltaTime);
        }
    }
}
