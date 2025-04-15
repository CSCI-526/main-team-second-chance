using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Card : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    private Color originalColor = new Color32(0x1C, 0x44, 0x6B, 0xFF);
    //private Outline pulseOutline;

    //private void pulseOutline()
    //{
    //    if (outlineEffect == null)
    //        outlineEffect = PanelImage.GetComponent<Outline>();

    //    if (pulseCoroutine == null && !isCardSelected)
    //        pulseCoroutine = StartCoroutine(PulseOutline());
    //}

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
