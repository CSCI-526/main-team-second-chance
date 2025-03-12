using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BarStretch : MonoBehaviour
{
    public Transform mouse; 
    private Vector3 startPosition;

    void Start()
    {
        startPosition = mouse.position; 
        transform.localScale = new Vector3(1, 1, 1); 
        transform.rotation = Quaternion.Euler(90f, 0f, 0f); 
    }

    void Update()
    {
        if (mouse != null)
        {
            float distance = Vector3.Distance(startPosition, mouse.position);

            if (distance > 0.01f)
            {
                transform.localScale = new Vector3(distance, 1f, 1);
            }
        }
    }
}
