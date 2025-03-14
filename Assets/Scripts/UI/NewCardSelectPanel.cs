using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewCardSelectPanel : MonoBehaviour
{
    [SerializeField]
    List<GameObject> Cards;
    [SerializeField]
    List<GameObject> MarblesReference;
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
    private void Cleanup(GameObject MarbleObject)
    {
        if (MarblesReference.Count == 0)
        {
            return;
        }
        List<GameObject> DeleteList = new List<GameObject>();
        for (int i = 0; i < MarblesReference.Count; ++i)
        {
            if (MarblesReference[i] != MarbleObject)
            {
                DeleteList.Add(MarblesReference[i]);
            }
        }
        for (int i = 0; i < DeleteList.Count; ++i)
        {
            MarblesReference.Remove(DeleteList[i]);
            Destroy(DeleteList[i]);
        }
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

    private void RevealCards(List<GameObject> MarblesToAdd)
    {
        MarblesReference = MarblesToAdd;
        ShowPanel();
        for (int i = 0; i < Cards.Count; i++)
        {
            // Activate a corresponding UI Prefab
            Card card = Cards[i].GetComponent<Card>();
            Marble marbleData = MarblesReference[i].GetComponent<Marble>();
            if (!card || !marbleData)
            {
                Debug.LogWarning("MainUI.UpdateHand(): Card.cs is not attached to the card prefab. Or MarbleData.cs is not attached to Marble PrefabThis shouldn't happen");
                return;
            }
            card.UpdateInformation(marbleData.GetMarbleName(), marbleData.GetMarbleDescription(), MarblesReference[i]);
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
