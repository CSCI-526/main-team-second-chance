using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MarbleInspectUI : MonoBehaviour
{
    [SerializeField] private Card hoverView;
    private void OnEnable()
    {
        MarbleEvents.OnMarbleHover += MarbleEventsOnOnMarbleHover;
    }

    private void OnDisable()
    {
        MarbleEvents.OnMarbleHover -= MarbleEventsOnOnMarbleHover;
    }
    
    private void MarbleEventsOnOnMarbleHover(Marble obj)
    {
        if (obj == null)
        {
            hoverView.gameObject.SetActive(false);
        }
        else
        {
            hoverView.UpdateInformation(obj.GetMarbleData(),false);
            hoverView.transform.position = Camera.main.WorldToScreenPoint(obj.transform.position);
            hoverView.gameObject.SetActive(true);
        }
    }
}
