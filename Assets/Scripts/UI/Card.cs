using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Card : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    public struct CardMotion
    {
        public Vector3 startPosition;
        public Vector3 endPosition;
        public float startScale;
        public float endScale;
        public float delay;
        public float duration;
        public bool skipIfSame;
    }
    public List<CardMotion> queuedMotions = new();
    private float currentMotionTime = 0f;

    [SerializeField]
    private AnimationCurve MotionCurve;

    private Color originalColor = new Color32(0x1C, 0x44, 0x6B, 0xFF);
    //private Outline pulseOutline;

    //private void pulseOutline()
    //{
    //    if (outlineEffect == null)
    //        outlineEffect = PanelImage.GetComponent<Outline>();

    //    if (pulseCoroutine == null && !isCardSelected)
    //        pulseCoroutine = StartCoroutine(PulseOutline());
    //}

    private void Update()
    {
        while (queuedMotions.Count > 0)
        {
            CardMotion motion = queuedMotions[0];
            currentMotionTime += Time.deltaTime;
            if (currentMotionTime >= motion.duration + motion.delay)
            {
                transform.position = motion.endPosition;
                transform.localScale = new Vector3(motion.endScale, motion.endScale, motion.endScale);
                queuedMotions.RemoveAt(0);
                currentMotionTime = 0f;

                continue;
            }

            float t = Math.Max((currentMotionTime - motion.delay) / motion.duration, 0f);
            t = MotionCurve.Evaluate(t);
            transform.position = Vector3.Lerp(motion.startPosition, motion.endPosition, t);
            float scaleLerp = Mathf.Lerp(motion.startScale, motion.endScale, t);
            transform.localScale = new Vector3(scaleLerp, scaleLerp, scaleLerp);

            break;
        }
    }

    public void AddMotion(CardMotion motion)
    {
        if (motion.skipIfSame && transform.position == motion.endPosition && transform.localScale == new Vector3(motion.endScale, motion.endScale, motion.endScale))
        {
            return;
        }

        queuedMotions.Add(motion);
        if (queuedMotions.Count == 1)
        {
            transform.position = motion.startPosition;
            transform.localScale = new Vector3(motion.startScale, motion.startScale, motion.startScale);
        }
    }

    public void UpdateInformation(string MarblePrefab, string CardDetail)
    {
        MarbleType.SetText(MarblePrefab);
        PanelImage.material = null;
        CardDescription.SetText(CardDetail);
    }
    public void UpdateInformation(MarbleData MarbleObject)
    {
        MarbleType.SetText(MarbleObject.MarbleName);
        PanelImage.material = null;
        CardDescription.SetText(MarbleObject.MarbleDescription);
        NewMarbleToAdd = MarbleObject;
    }
    /*public void OnPointerDown(PointerEventData eventData)
    {
        throw new System.NotImplementedException();
    }
    */

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (GameManager.Instance.GetTurnState() == TurnState.PlayerTurn &&
            !GameManager.Instance.GetPlayerManager().isLaunchingMarble)
        {
            GameManager.Instance.GetPlayerManager().GetPlayerDeck().bIsHoveringDeck = true;
            PanelImage.color = Color.white;
            MarbleType.color = Color.black;
            CardDescription.color = Color.black;
        }
        else if (GameManager.Instance.GetTurnState() == TurnState.CardSelect)
        {
            PanelImage.color = Color.white;
            MarbleType.color = Color.black;
            CardDescription.color = Color.black;
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (GameManager.Instance.GetTurnState() == TurnState.PlayerTurn &&
            !GameManager.Instance.GetPlayerManager().isLaunchingMarble)
        {
            GameManager.Instance.GetPlayerManager().GetPlayerDeck().bIsHoveringDeck = false;

        }
        PanelImage.color = originalColor;
        MarbleType.color = Color.white;
        CardDescription.color = Color.white;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button != PointerEventData.InputButton.Left)
        {
            return;
        }
        if (GameManager.Instance)
        {
            switch (GameManager.Instance.GetTurnState())
            {
                case TurnState.CardSelect:
                    {
                        Debug.Log("Clicked on a card with ID CardSelect: " + HandIndex);
                        PanelImage.material = SelectedMaterial;
                        DeckEvents.AddNewMarbleToDeck(NewMarbleToAdd);
                        break;
                    }
                case TurnState.PlayerTurn:
                    {
                        Debug.Log("Clicked on a card with ID PlayerTurn: " + HandIndex);

                        Deck playerDeck = GameManager.Instance.GetPlayerManager().GetPlayerDeck();
                        if (playerDeck.SelectedMarbleRef == PanelImage)
                        {
                            playerDeck.SelectedMarbleRef.material = null;
                            playerDeck.SelectedMarbleRef = null;
                            playerDeck.ResetSelectedMarbleIndex();
                        }
                        else
                        {
                            if (playerDeck.SelectedMarbleRef != null)
                            {
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
        if (bIsInCardSelect && !ViewDeck.IsDiscardingCard)
        {
            ViewDeck.DoDiscardCard(HandIndex);
        }

        if (TutorialManager.Instance.ShouldDisplayAnymore && TutorialManager.Instance.CurrentTutorialPhase == TutorialPhases.SELECT_MARBLE)
        {
            TutorialEvents.DoTutorialItemDisplayed(TutorialManager.Instance.CurrentTutorialPhase);
        }
    }

    public int GetHandIndex() { return HandIndex; }
    public void SetHandIndex(int index) { HandIndex = index; }
    public bool IsInCardSelect
    {
        get { return bIsInCardSelect; }
        set { bIsInCardSelect = value; }
    }
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
    private bool bIsInCardSelect = false;
}
