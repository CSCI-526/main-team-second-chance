using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewCardSelectPanel : MonoBehaviour
{
    [SerializeField]
    List<GameObject> Cards;
    [SerializeField]
    List<MarbleData> MarblesReference;
    private void OnEnable()
    {
        DeckEvents.OnSelectNewMarbleToAdd += RevealCards;
        TurnStateEvents.OnTurnProgress += UpdateTurnPanel;
        DeckEvents.OnAddNewMarbleToDeck += Cleanup;

    }
    private void OnDisable()
    {
        DeckEvents.OnSelectNewMarbleToAdd -= RevealCards;
        TurnStateEvents.OnTurnProgress -= UpdateTurnPanel;
        DeckEvents.OnAddNewMarbleToDeck -= Cleanup;
    }
    private void Cleanup(MarbleData MarbleObject)
    {
        if (MarblesReference.Count == 0)
        {
            return;
        }
        MarblesReference.Clear();
    }
    void Start()
    {
        HidePanel();
    }
    private void UpdateTurnPanel(TurnState state)
    {
        if (state != TurnState.CardSelect)
        {
            HidePanel();
        }
    }

    private void RevealCards(List<MarbleData> MarblesToAdd)
    {
        MarblesReference = MarblesToAdd;
        ShowPanel();
        for (int i = 0; i < Cards.Count; i++)
        {
            // Activate a corresponding UI Prefab
            Card card = Cards[i].GetComponent<Card>();
            if (!card)
            {
                Debug.LogWarning("MainUI.UpdateHand(): Card.cs is not attached to the card prefab. This shouldn't happen");
                return;
            }
            card.UpdateInformation(MarblesReference[i].MarbleName, MarblesReference[i].MarbleDescription, MarblesReference[i]);
            card.SetHandIndex(i);
            Cards[i].SetActive(true);
        }
    }

    private void HidePanel()
    {
        gameObject.GetComponent<CanvasRenderer>().SetAlpha(0);
        for (int i = 0; i < Cards.Count; i++)
        {
            Cards[i].SetActive(false);
        }
    }

    private void ShowPanel()
    {
        gameObject.GetComponent<CanvasRenderer>().SetAlpha(1);
    }

}
