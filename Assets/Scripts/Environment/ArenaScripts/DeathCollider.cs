using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathCollider : MonoBehaviour
{
    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("Marble"))
        {
            Marble otherMarble = other.gameObject.GetComponent<Marble>();
            otherMarble.DestroyMarble();
        }
    }
}
