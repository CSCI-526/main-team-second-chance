using System;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Node : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    public static event Action<int> OnAttemptEnterLevel;
    public static void AttemptEnterLevel(int level)
    {
        OnAttemptEnterLevel?.Invoke(level);
    }
    public Transform GetPreviousNode() { return PreviousNode; }
    public Transform GetNextNode() { return NextNode; }

    public bool GetIsTraversed() { return bIsTraversed; }
    public bool GetHasChildren() { return bHasChildren; }
    public void SetHasChildren(bool Value) { bHasChildren = Value; }
    public void SetLayer(int Value) { Layer = Value; }
    public int GetLayer() { return Layer; }
    public int GetNumParents() { return numParents; }
    public void IncrementNumParents() { numParents++; }
    public void SetCorrespondingLevelSO(int Value) { DataRep = Value; }
    public int GetCorrespondingLevelSO() { return DataRep; }
    public void AddChild(Node Child, UILineRenderer Line)
    {
        ChildrenList.Add(Child);
        NodeToLine.Add(Child, Line);
    }
    public void AddParent(Node Parent, UILineRenderer Line)
    {
        ParentList.Add(Parent);
        NodeToLine.Add(Parent, Line);
    }
    public List<Node> GetChildren() { return ChildrenList; }
    public UILineRenderer GetUILineRenderer(Node Key)
    {
        if (Key == null)
        {
            return null;
        }
        if (NodeToLine.TryGetValue(Key, out var Value))
        {
            return Value;
        }
        return null;
    }
    public void MarkTraversed()
    {
        if (!image)
        {
            image = GetComponent<Image>();
        }
        outline.effectColor = NodeManager.Instance.clearedLevelOutlineColor;
        bIsTraversed = true;
        foreach (Node Child in ChildrenList)
        {
            if (NodeToLine.TryGetValue(Child, out UILineRenderer value))
            {
                value.SetIsTraversed(true);
                value.SetColor();
            }
        }
    }

    public void MarkInaccessible()
    {
        if (!image)
        {
            image = GetComponent<Image>();
        }

        if (bIsTraversed) {
            return;
        }

        bIsInaccessible = true;

        SetOutlineColor(false);
        float grayscaleImage = image.color.grayscale;
        image.color = new Color(grayscaleImage, grayscaleImage, grayscaleImage);
        
        foreach (Node Child in ChildrenList)
        {
            if (NodeToLine.TryGetValue(Child, out UILineRenderer value))
            {
                value.SetIsInaccessible(true);
                value.SetColor();
            }
        }
    }
    public void OnPointerClick(PointerEventData eventData)
    {
        if (image.color == NodeManager.Instance.clearedLevelOutlineColor)
        {
            return;
        }
        AudioManager.TriggerSound(AudioManager.Instance.ClickSound,Vector3.zero);
        OnAttemptEnterLevel(DataRep);
    }
    public void UpdateNameOfNode(string Name)
    {
        NodeName.text = Name;
    }
    [SerializeField]
    private Image image;

    public void ShowLevel1Icon() {
        Color newColor = level1Icon.color;
        newColor.a = 1;
        level1Icon.color = newColor;
    }
    [SerializeField]
    private Image level1Icon;
    [SerializeField]
    private Outline outline;
    [SerializeField]
    private Transform PreviousNode;
    [SerializeField]
    private Transform NextNode;
    [SerializeField]
    private Color DefaultColor;
    [SerializeField]
    private TextMeshProUGUI NodeName;
    [SerializeField, Tooltip("Used for highlighting the children node paths")]
    private int Layer = 0;
    private int numParents = 0;
    private List<Node> ChildrenList = new List<Node>();
    private List<Node> ParentList = new List<Node>();
    private Dictionary<Node, UILineRenderer> NodeToLine = new Dictionary<Node, UILineRenderer>();
    private int DataRep;
    private bool bHasChildren = false;
    private bool bIsTraversed = false;
    private bool bIsInaccessible = false;
    private void Start()
    {
        if (ChildrenList == null)
        {
            ChildrenList = new List<Node>();
        }
        if (ParentList == null)
        {
            ParentList = new List<Node>();
        }
        if (NodeToLine == null)
        {
            NodeToLine = new Dictionary<Node, UILineRenderer>();
        }
        if (image == null)
        {
            image = GetComponent<Image>();
        }
    }
    public void CalculateDefaultColor(int difficulty)
    {
        DefaultColor = NodeManager.Instance.levelColorsByDifficulty[difficulty - 1];
        image.color = DefaultColor;
    }

    public void SetOutlineColor(bool isHovered) 
    {
        if (bIsInaccessible) 
        {
            float grayscale = NodeManager.Instance.clearedLevelOutlineColor.grayscale;
            outline.effectColor = new Color(grayscale, grayscale, grayscale);
        }
        else if (isHovered)
        {
            outline.effectColor = NodeManager.Instance.hoverLevelOutlineColor;
        }
        else if (bIsTraversed)
        {
            outline.effectColor = NodeManager.Instance.clearedLevelOutlineColor;
        }
        else
        {
            outline.effectColor = NodeManager.Instance.lockedLevelOutlineColor;
        }
    }
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (bIsInaccessible || bIsTraversed) {
            return;
        }

        SetOutlineColor(true);

        // Highlight each child route
        foreach (Node Child in ChildrenList)
        {
            if (NodeToLine.TryGetValue(Child, out UILineRenderer value))
            {
                value.SetColor(NodeManager.Instance.hoverLevelOutlineColor);
            }
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        SetOutlineColor(false);

        // Unhighlight each child route
        foreach (Node Child in ChildrenList)
        {
            if (NodeToLine.TryGetValue(Child, out UILineRenderer value))
            {
                value.SetColor();
            }
        }
    }
}
