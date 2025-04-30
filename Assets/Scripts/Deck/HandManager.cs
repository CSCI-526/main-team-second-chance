using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class HandManager : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
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
    [SerializeField]
    private Transform HandManagerPosition;
    private List<Card> Cards = new();
    private List<bool> ActiveCards = new();
    bool bShouldReanimate = false;
    bool bShouldRaiseHand = false;
    private bool bIsHovered = false;
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

    private void ToggleHand(bool shouldRaiseHand)
    {
        bShouldRaiseHand = shouldRaiseHand;
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
            if (bShouldRaiseHand)
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
        
        if (Cards.Count > 0 && Cards[0].queuedMotions.Count > 0) {
            Vector3 newPosition = HandManagerPosition.transform.position;
            newPosition.y = Cards[0].transform.position.y;
            HandManagerPosition.transform.position = newPosition;
        }
    }

    private void RecalculateCardPositions(bool animateFromDeck)
    {
        Debug.Log("RecalculateCardPositions " + animateFromDeck);
        
        int numActiveCards = 0;
        foreach (bool active in ActiveCards) {
            numActiveCards += active ? 1 : 0;
        }

        // float gap = Screen.width / 10f * 1.15f;
        float gap = 30;
        int cardWidth = 200;
        for (int i = 0; i < Cards.Count; i++)
        {
            if (!ActiveCards[i]) 
            {
                continue;
            }

            float offset = (gap + cardWidth) * (i - (numActiveCards - 1) / 2.0f);
            Vector3 endPosition = new Vector3(Screen.width / 2.0f + offset, -Screen.height / 15f, 0f) + handOffset;

            Card card = Cards[i];
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

    public void OnPointerEnter(PointerEventData eventData)
    {
        // Ignore if animation is currently playing.
        if (Cards.Count > 0 && Cards[0].queuedMotions.Count > 0) {
            return;
        }

        if (GameManager.Instance.GetTurnState() == TurnState.PlayerTurn && 
            GameManager.Instance.PlayerHasSelectedMarble() && 
            !GameManager.Instance.GetPlayerManager().isLaunchingMarble &&
            !bIsHovered)
        {
            bIsHovered = true;
            ToggleHand(true);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        // Ignore if animation is currently playing.
        if (Cards.Count > 0 && Cards[0].queuedMotions.Count > 0) {
            return;
        }
        
        if (GameManager.Instance.GetTurnState() == TurnState.PlayerTurn &&
            GameManager.Instance.PlayerHasSelectedMarble() && 
            !GameManager.Instance.GetPlayerManager().isLaunchingMarble &&
            bIsHovered)
        {
            bIsHovered = false;
            ToggleHand(false);
        }
    }
}
