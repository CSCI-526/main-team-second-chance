using System;
using System.Collections.Generic;
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
        if (!Icon)
        {
            Icon = GetComponent<Image>();
        }
        Icon.color = TraversedColor;
        bIsTraversed = true;
        UILineRenderer TravelLine = GetUILineRenderer(NextNode);
        if (TravelLine)
        {
            TravelLine.SetIsTraversed(true);
            TravelLine.SetColor();
        }
    }
    public void OnPointerClick(PointerEventData eventData)
    {
        if (Icon.color == TraversedColor)
        {
            return;
        }
        OnAttemptEnterLevel(DataRep);

    }


    [SerializeField]
    private Image Icon;
    [SerializeField]
    private Transform PreviousNode;
    [SerializeField]
    private Transform NextNode;
    [SerializeField]
    private Color TraversedColor;
    [SerializeField]
    private Color DefaultColor;
    [SerializeField, Tooltip("Used for highlighting the children node paths")]
    private Color OverrideColor;
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
        if(ChildrenList == null)
        {
            ChildrenList = new List<Node>();
        }
        if (ParentList == null)
        {
            ParentList = new List<Node>();
        }
        if(NodeToLine == null)
        {
            NodeToLine = new Dictionary<Node, UILineRenderer>();
        }
        if(Icon == null)
        {
            Icon = GetComponent<Image>();
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        Icon.color = OverrideColor;
        // Highlight each child route
        foreach (Node Child in ChildrenList)
        {
            if(NodeToLine.TryGetValue(Child, out UILineRenderer value))
            {
                value.SetColor(OverrideColor);
            }
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        Icon.color = bIsTraversed ? TraversedColor : DefaultColor;
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
