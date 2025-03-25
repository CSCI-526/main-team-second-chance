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
    private TextMeshProUGUI PlayerRoundsWon;
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
        MarbleEvents.OnRoundsWonChanged += UpdateRoundsWon;
        DeckEvents.OnDeckGenerated += UpdateDeckCount;
        DeckEvents.OnMarbleUsed += UpdateDeckCount;
        DeckEvents.OnHandUpdated += UpdateHand;
    }
    private void OnDisable()
    {
        MarbleEvents.OnScoreChange -= UpdateScore;
        MarbleEvents.OnRoundsWonChanged -= UpdateRoundsWon;

        DeckEvents.OnDeckGenerated -= UpdateDeckCount;
        DeckEvents.OnMarbleUsed -= UpdateDeckCount;
        DeckEvents.OnHandUpdated -= UpdateHand;
    }
    private void UpdateRoundsWon(int RoundsWon)
    {
        PlayerRoundsWon.text = $"{RoundsWon} / 3 Games Won";
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
        if (Team != MarbleTeam.Player)
        {
            return;
        }
        if (Count == 0)
        {
            PlayerDeckCount.color = Color.red;
        }
        else
        {
            PlayerDeckCount.color = Color.white;
        }
        PlayerDeckCount.text = $"{Count}";

    }

    /* TO DO REFACTOR*/
    private void UpdateHand(MarbleTeam Team, List<MarbleData> dataList)
    {
        if(Team != MarbleTeam.Player)
        {
            return;
        }
        if(dataList == null)
        {
            Debug.LogError("MainUI.UpdateHand(): The marbledata list being sent in is null. This probably shouldn't happen");
            return;
        }

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
        // if the number of cards to rep is greater than the hand size
        if (Cards.Count < dataList.Count)
        {
            Debug.LogError("MainUI.UpdateHand(): NumCardsToRep is larger than the actual number of spawn points. This shouldn't happen \n" + Cards.Count + " <" + dataList.Count);
            return;
        }

        int negation = -1;
        int offsetIter = 0;
        for (int i = 0; i < dataList.Count; i++)
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
            MarbleData marbleData = dataList[i];
            if (!card || !marbleData)
            {
                Debug.LogWarning("MainUI.UpdateHand(): Card.cs is not attached to the card prefab. Or input data has is incorrect This shouldn't happen");
                return;
            }
            card.UpdateInformation(marbleData.MarbleName, marbleData.MarbleDescription);
            card.SetHandIndex(i);
            Cards[i].gameObject.transform.localPosition = Offset;
            Cards[i].SetActive(true);
        }
        // cleanup the rest of the available cards if there are more than the hand size ie if 
        if (dataList.Count < Cards.Count)
        {
            for (int i = dataList.Count; i < Cards.Count; i++)
            {
                if (Cards[i].activeInHierarchy)
                {
                    Cards[i].SetActive(false);
                }
            }
        }
    }
}
