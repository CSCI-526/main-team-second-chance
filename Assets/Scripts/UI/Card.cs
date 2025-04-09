using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Card : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
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
    */

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (GameManager.Instance.GetTurnState() == TurnState.PlayerTurn) {
            GameManager.Instance.GetPlayerManager().GetPlayerDeck().bIsHoveringDeck = true;
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        Debug.Log("Exiting");
        if (GameManager.Instance.GetTurnState() == TurnState.PlayerTurn) {
            GameManager.Instance.GetPlayerManager().GetPlayerDeck().bIsHoveringDeck = false;
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
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
                Debug.Log("Clicked on a card with ID: " + HandIndex);

                Deck playerDeck = GameManager.Instance.GetPlayerManager().GetPlayerDeck();
                if (playerDeck.SelectedMarbleRef == PanelImage) {
                    playerDeck.SelectedMarbleRef.material = null;
                    playerDeck.SelectedMarbleRef = null;
                    playerDeck.ResetSelectedMarbleIndex();
                }
                else {
                    if (playerDeck.SelectedMarbleRef != null) {
                        playerDeck.SelectedMarbleRef.material = null;
                    }
                    playerDeck.SelectedMarbleRef = PanelImage;
                    playerDeck.SelectedMarbleRef.material = SelectedMaterial;
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
