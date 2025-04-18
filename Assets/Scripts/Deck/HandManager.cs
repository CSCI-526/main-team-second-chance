using System.Collections.Generic;
using UnityEngine;

public class HandManager : MonoBehaviour
{
    public static HandManager Instance;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void OnDestroy()
    {
        Instance = null;
    }

    [SerializeField]
    private Transform CardStartingPoint;
    private List<Card> Cards = new();
    private List<bool> ActiveCards = new();
    bool bShouldReanimate = false;
    bool bCanPlayerSelectMarble = false;
    public bool bAnimateFromDeck = true;
    public bool bAllHidden = false;
    private Vector3 handOffset;

    void Start()
    {
        handOffset = Vector3.zero;
    }

    private void OnEnable()
    {
        TurnStateEvents.OnTurnProgress += OnTurnProgress;
        DeckEvents.OnMarbleSelectedFromHand += OnMarbleSelectedFromHand;

    }
    private void OnDisable()
    {
        TurnStateEvents.OnTurnProgress -= OnTurnProgress;
        DeckEvents.OnMarbleSelectedFromHand -= OnMarbleSelectedFromHand;
    }

    private void OnTurnProgress(TurnState state)
    {
        ToggleHand(state == TurnState.PlayerTurn);
    }

    private void OnMarbleSelectedFromHand(MarbleTeam Team, int CardID)
    {
        if (Team == MarbleTeam.Player)
        {
            ToggleHand(false);
        }
    }

    private void ToggleHand(bool canPlayerSelectMarble)
    {
        bCanPlayerSelectMarble = canPlayerSelectMarble;
        bShouldReanimate = true;
    }

    public void AddCard(Card card)
    {
        Cards.Add(card);
        ActiveCards.Add(card.gameObject.activeSelf);
        bShouldReanimate = true;
    }

    void LateUpdate()
    {
        bool changed = false;
        bool allHidden = true;
        for (int i = 0; i < Cards.Count; i++)
        {
            Card card = Cards[i];
            bool active = card.gameObject.activeSelf;
            if (active != ActiveCards[i])
            {
                changed = true;
                ActiveCards[i] = active;
            }

            allHidden = allHidden && !active;
        }

        if (changed || bShouldReanimate)
        {
            if (bCanPlayerSelectMarble)
            {
                handOffset = new Vector3(0, Screen.height / 6f, 0);
            }
            else
            {
                handOffset = Vector3.zero;
            }

            RecalculateCardPositions(bAnimateFromDeck || (!allHidden && bAllHidden));
            bShouldReanimate = false;
            bAnimateFromDeck = false;
        }

        bAllHidden = allHidden;
    }

    private void RecalculateCardPositions(bool animateFromDeck)
    {
        Debug.Log("RecalculateCardPositions " + animateFromDeck);
        float gap = Screen.width / 10f * 1.15f;
        for (int i = 0; i < Cards.Count; i++)
        {
            float offset = gap * (i - (Cards.Count / 4.0f));
            Card card = Cards[i];
            Vector3 endPosition = new Vector3(Screen.width / 2.0f + offset, -Screen.height / 15f, 0f) + handOffset;
            if (animateFromDeck)
            {
                card.queuedMotions.Clear();
                card.AddMotion(new Card.CardMotion
                {
                    delay = i * 0.25f,
                    duration = 0.5f,
                    startPosition = CardStartingPoint.position,
                    endPosition = endPosition,
                    startScale = 0,
                    endScale = card.transform.localScale.x,
                    skipIfSame = false
                });
            }
            else
            {
                card.AddMotion(new Card.CardMotion
                {
                    delay = 0f,
                    duration = 0.25f,
                    startPosition = card.transform.position,
                    endPosition = endPosition,
                    startScale = card.transform.localScale.x,
                    endScale = card.transform.localScale.x,
                    skipIfSame = true
                });
            }
        }

    }
}
