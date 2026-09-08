using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MarbleInspectUI : MonoBehaviour
{
    [SerializeField] private MarbleInpsectCardUI hoverView;
    private Marble _hoveredMarble = null;
    private void OnEnable()
    {
        MarbleEvents.OnMarbleHover += MarbleEventsOnOnMarbleHover;
        MarbleEvents.OnMarbleAbilityCast += MarbleEventsOnOnMarbleAbilityCast;
    }

    private void MarbleEventsOnOnMarbleAbilityCast(Marble obj)
    {
        if (_hoveredMarble == obj)
        {
            hoverView.UpdateInformation(obj);
        }
    }

    private void OnDisable()
    {
        MarbleEvents.OnMarbleHover -= MarbleEventsOnOnMarbleHover;
        MarbleEvents.OnMarbleAbilityCast -= MarbleEventsOnOnMarbleAbilityCast;
    }
    
    private void MarbleEventsOnOnMarbleHover(Marble obj)
    {
        _hoveredMarble = obj;
        if (obj == null)
        {
            hoverView.gameObject.SetActive(false);
        }
        else
        {
            hoverView.UpdateInformation(obj);
            hoverView.transform.position = Camera.main.WorldToScreenPoint(obj.transform.position);
            hoverView.gameObject.SetActive(true);
        }
    }
}
