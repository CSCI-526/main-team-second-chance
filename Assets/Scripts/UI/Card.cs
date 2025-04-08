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
    public void UpdateInformation(string MarblePrefab, string CardDetail, MarbleData MarbleObject)
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
        if (eventData.button != PointerEventData.InputButton.Left) {
            return;
        }

        switch (GameManager.Instance.GetTurnState()) {
            case TurnState.CardSelect:
            {
                Debug.Log("Clicked on a card with ID: " + HandIndex);
                PanelImage.material = SelectedMaterial;
                DeckEvents.AddNewMarbleToDeck(NewMarbleToAdd);
                break;
            }
            case TurnState.PlayerTurn:
            {
                if (PanelImage.material == SelectedMaterial) {
                    PanelImage.material = null;
                    GameManager.Instance.GetPlayerManager().GetPlayerDeck().ResetSelectedMarbleIndex();
                }
                else if (GameManager.Instance.GetPlayerManager().GetPlayerDeck().GetSelectedMarbleIndex() < 0) {
                    Debug.Log("Clicked on a card with ID: " + HandIndex);
                    PanelImage.material = SelectedMaterial;
                    DeckEvents.MarbleSelectedFromHand(MarbleTeam.Player, HandIndex);
                }
                break;
            }
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
    private MarbleData NewMarbleToAdd;
}
