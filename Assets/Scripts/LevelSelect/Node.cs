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
    public void MarkTraversed(Node NextNode)
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
    public void OnPointerClick(PointerEventData eventData)
    {
        if (image.color == NodeManager.Instance.clearedLevelOutlineColor)
        {
            return;
        }
        OnAttemptEnterLevel(DataRep);
    }
    public void UpdateNameOfNode(string Name)
    {
        NodeName.text = Name;
        NodeName.color = NodeManager.Instance.clearedLevelOutlineColor;
    }
    [SerializeField]
    private Image image;
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
    public void OnPointerEnter(PointerEventData eventData)
    {
        outline.effectColor = NodeManager.Instance.hoverLevelOutlineColor;
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
        outline.effectColor = bIsTraversed ? NodeManager.Instance.clearedLevelOutlineColor : NodeManager.Instance.lockedLevelOutlineColor;
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
