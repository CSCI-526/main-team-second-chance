using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Card : MonoBehaviour, IPointerClickHandler
{
    public void UpdateInformation(string MarblePrefab, string CardDetail)
    {
        MarbleType.SetText(MarblePrefab);
        PanelImage.material = null;
        CardDescription.SetText(CardDetail);
    }
    public void UpdateInformation(string MarblePrefab, string CardDetail, GameObject MarbleObject)
    {
        MarbleType.SetText(MarblePrefab);
        PanelImage.material = null;
        CardDescription.SetText(CardDetail);
        NewMarbleToAdd = MarbleObject;
    }
    /*public void OnPointerDown(PointerEventData eventData)
    {
        throw new System.NotImplementedException();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        throw new System.NotImplementedException();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        throw new System.NotImplementedException();
    }*/
    public void OnPointerClick(PointerEventData eventData)
    {
        Debug.Log("Clicked on a card with ID: " + HandIndex);
        if (eventData.button == PointerEventData.InputButton.Left && GameManager.Instance.GetTurnState() == TurnState.CardSelect)
        {
            Debug.Log("Clicked on a card with ID: " + HandIndex);
            PanelImage.material = SelectedMaterial;
            DeckEvents.AddNewMarbleToDeck(NewMarbleToAdd);
        }
        else if (eventData.button == PointerEventData.InputButton.Left && GameManager.Instance.GetPlayerManager().GetPlayerDeck().GetSelectedMarbleIndex() < 0)
        {
            Debug.Log("Clicked on a card with ID: " + HandIndex);
            PanelImage.material = SelectedMaterial;
            DeckEvents.MarbleSelectedFromHand(HandIndex);
        }
    }

    public int GetHandIndex() { return HandIndex; }
    public void SetHandIndex(int index) { HandIndex = index; }

    [SerializeField]
    private TextMeshProUGUI MarbleType;
    [SerializeField]
    private TextMeshProUGUI CardDescription;
    [SerializeField]
    private Image PanelImage;
    [SerializeField]
    private Material SelectedMaterial;
    private int HandIndex;
    [SerializeField]
    private GameObject NewMarbleToAdd;
}
