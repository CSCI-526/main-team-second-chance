using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MainUI : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI PlayerDeckCount;
    [SerializeField]
    private TextMeshProUGUI PlayerScore;
    [SerializeField]
    private TextMeshProUGUI EnemyScore;
    [SerializeField]
    private Transform HandStartingPoint;
    [SerializeField]
    private float CardOffsetDistance = 50.0f;
    [SerializeField]
    private GameObject CardPrefab;
    private List<GameObject> Cards = new List<GameObject>();
    private void OnEnable()
    {
        MarbleEvents.OnScoreChange += UpdateScore;
        DeckEvents.OnDeckGenerated += UpdateDeckCount;
        DeckEvents.OnMarbleUsed += UpdateDeckCount;
        DeckEvents.OnHandUpdated += UpdateHand;
    }
    private void OnDisable()
    {
        MarbleEvents.OnScoreChange -= UpdateScore;
        DeckEvents.OnDeckGenerated -= UpdateDeckCount;
        DeckEvents.OnMarbleUsed -= UpdateDeckCount;
        DeckEvents.OnHandUpdated -= UpdateHand;
    }
    private void UpdateScore(MarbleTeam Team)
    {
        if (Team == MarbleTeam.Player)
        {
            PlayerScore.text = $"Player Score: {GameManager.Instance.GetPlayerScore()}";
        }
        else
        {
            EnemyScore.text = $"Enemy Score: {GameManager.Instance.GetEnemyScore()}";
        }
    }
    private void UpdateDeckCount(MarbleTeam Team, int Count)
    {
        if (Count == 0)
        {
            PlayerDeckCount.color = Color.red;
        }
        else
        {
            PlayerDeckCount.color = Color.white;
        }
        if (Team == MarbleTeam.Player)
        {

            PlayerDeckCount.text = $"{Count}";
        }
        else
        {
            PlayerDeckCount.text = $"{Count}";
        }
    }
    private void UpdateHand()
    {
        int numCardsToRep = GameManager.Instance.GetPlayerManager().GetPlayerDeck().GetHandSize();
        List<GameObject> Hand = GameManager.Instance.GetPlayerManager().GetPlayerDeck().GetHand();
        if (Cards.Count == 0)
        {
            for (int i = 0; i < GameManager.Instance.GetPlayerManager().GetPlayerDeck().GetMaxHandSize(); i++)
            {
                GameObject prefab = Instantiate(CardPrefab, HandStartingPoint);
                prefab.transform.position = Vector3.zero;
                prefab.SetActive(false);
                Cards.Add(prefab);
            }
        }
        // if the number of cards to rep is greater than the maximum hand size
        if (Cards.Count < numCardsToRep)
        {
            Debug.LogError("MainUI.UpdateHand(): NumCardsToRep" + numCardsToRep + " is larger than the actual number of spawn points" + Cards.Count + ". This shouldn't happen");
            return;
        }

        int negation = -1;
        int offsetIter = 0;
        for (int i = 0; i < numCardsToRep; i++)
        {
            // Calculate where to put the card in UI
            Vector3 Offset = new Vector3(offsetIter * CardOffsetDistance * negation, 50.0f, 0);
            negation *= -1;
            if (i % 2 == 0)
            {
                offsetIter++;
            }
            // Activate a corresponding UI Prefab
            Card card = Cards[i].GetComponent<Card>();
            Marble marbleData = Hand[i].GetComponent<Marble>();
            if (!card || !marbleData)
            {
                Debug.LogWarning("MainUI.UpdateHand(): Card.cs is not attached to the card prefab. Or MarbleData.cs is not attached to Marble PrefabThis shouldn't happen");
                return;
            }
            card.UpdateInformation(marbleData.GetMarbleName() + marbleData.GetUniqueID(), marbleData.GetMarbleDescription());
            card.SetHandIndex(i);
            Cards[i].gameObject.transform.localPosition = Offset;
            Cards[i].SetActive(true);
        }
        // cleanup the rest of the available cards if there are more than the hand size ie if 
        if (numCardsToRep < Cards.Count)
        {
            for (int i = numCardsToRep; i < Cards.Count; i++)
            {
                if (Cards[i].activeInHierarchy)
                {
                    Cards[i].SetActive(false);
                }
            }
        }
    }
}
