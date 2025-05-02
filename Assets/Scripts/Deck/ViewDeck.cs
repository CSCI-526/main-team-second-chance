using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Experimental.Rendering;
using UnityEngine.UI;

public class ViewDeck : MonoBehaviour
{
    [SerializeField]
    private GameObject DiscardPanel;
    [SerializeField]
    private GameObject DiscardRemainingPanel;
    [SerializeField]
    private GameObject DeckPanel;
    TextMeshProUGUI DiscardRemainingText = null;
    [SerializeField]
    private GameObject CardPrefab;
    [SerializeField]
    private TextMeshProUGUI NoMarblesText;
    private List<GameObject> DisplayedCards = new List<GameObject>();
    private static bool bIsDiscardingCard = false;
    private int PotentialIndexToDiscard = -1;
    public static bool IsDiscardingCard
    {
        get { return bIsDiscardingCard; }
        set { bIsDiscardingCard = value; }
    }
    [SerializeField]
    private int MAX_NUM_TO_DISCARD;
    private int numDiscardedCards = 0;
    public static event Action<int> OnDiscardCardPrompted;
    public static void DoDiscardCard(int Index)
    {
        OnDiscardCardPrompted?.Invoke(Index);
    }

    private void OnEnable()
    {
        OnDiscardCardPrompted += DisplayDiscardPanel;

        if (NodeManager.Instance == null)
        {
            return;
        }
        DisplayDeck();
    }
    private void DisplayDeck()
    {
        // Pull the player deck from the node manager script
        List<MarbleData> PlayerMarbles = NodeManager.Instance.GetPlayerDeck();
        if (PlayerMarbles == null)
        {
            return;
        }
        if (DisplayedCards.Count != 0)
        {
            foreach (GameObject go in DisplayedCards)
            {
                if (go)
                {
                    Destroy(go);
                }
            }
            DisplayedCards.Clear();
        }
        if (PlayerMarbles.Count == 0)
        {
            DeckPanel.SetActive(false);
            NoMarblesText.gameObject.SetActive(true);
        }
        else
        {
            DeckPanel.SetActive(true);
            NoMarblesText.gameObject.SetActive(false);
        }
        // Populate the panel with cards 
        for (int i = 0; i < PlayerMarbles.Count; i++)
        {
            GameObject CardUI = Instantiate(CardPrefab, DeckPanel.transform, false);
            Card PrefabCard = CardUI.GetComponent<Card>();
            PrefabCard.UpdateInformation(PlayerMarbles[i]);
            PrefabCard.IsInCardSelect = true;
            PrefabCard.SetHandIndex(i);
            DisplayedCards.Add(CardUI);
        }
    }
    private void OnDisable()
    {
        OnDiscardCardPrompted -= DisplayDiscardPanel;
    }
    public void DisplayDiscardPanel(int Index)
    {
        if (DiscardPanel && DiscardRemainingPanel)
        {
            DiscardPanel.SetActive(true);
            bIsDiscardingCard = true;
            PotentialIndexToDiscard = Index;
            DiscardRemainingPanel.SetActive(true);
            if (!DiscardRemainingText)
            {
                DiscardRemainingText = DiscardRemainingPanel.GetComponentInChildren<TextMeshProUGUI>();
                DiscardRemainingText.SetText($"Discards Remaining: {MAX_NUM_TO_DISCARD - numDiscardedCards}");
            }
        }
    }
    public void HideDiscardPanel()
    {
        if (DiscardPanel && DiscardRemainingPanel)
        {
            DiscardPanel.SetActive(false);
            bIsDiscardingCard = false;
            PotentialIndexToDiscard = -1;
            DiscardRemainingPanel.SetActive(false);
            DiscardRemainingText = null;
        }
    }
    public void DiscardCard()
    {
        int DeckCount = NodeManager.Instance.GetPlayerDeck().Count;
        if (DeckCount - 1 <= 1 || PotentialIndexToDiscard < 0 || numDiscardedCards >= MAX_NUM_TO_DISCARD)
        {
            return;
        }
        numDiscardedCards++;
        NodeManager.Instance.RemoveFromPlayerDeck(PotentialIndexToDiscard);
        if (!DiscardRemainingText)
        {
            DiscardRemainingText = DiscardRemainingPanel.GetComponentInChildren<TextMeshProUGUI>();
        }
        DiscardRemainingText.SetText($"Discards Remaining: {MAX_NUM_TO_DISCARD - numDiscardedCards}");
        DisplayDeck();
    }
}
