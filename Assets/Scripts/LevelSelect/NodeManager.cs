using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;

public class NodeManager : MonoBehaviour
{
    private void OnApplicationQuit()
    {
        NodeManagerData.ClearPlayerDeck();
    }

    public static NodeManager Instance;
    public Color[] levelColorsByDifficulty = new Color[5];
    public Color hoverLevelOutlineColor;
    public Color clearedLevelOutlineColor;
    public Color lockedLevelOutlineColor;

    public Color[] getLevelColorsByDifficulty()
    {
        return levelColorsByDifficulty;
    }

    public bool ShouldRestartOrMenu()
    {
        int LevelsLength = NodeManagerData.GetLevels().Count;
        LevelDataSO LastLevel = NodeManagerData.GetLevels()[LevelsLength - 1];
        if (LastLevel == NodeManagerData.GetActiveLevel())
        {
            return true;
        }
        return false;
    }
    public LevelDataSO GetLevelData()
    {
        return NodeManagerData.GetActiveLevel();
    }
    public List<MarbleData> GetPlayerDeck()
    {
        return NodeManagerData.GetPlayerDeck();
    }
    public void RemoveFromPlayerDeck(int Index)
    {
        NodeManagerData.RemoveCardFromPlayerDeck(Index);
    }
    public void ResetPlayerDeck()
    {
        NodeManagerData.ClearPlayerDeck();
    }
    public void UpdatePlayerDeck(List<MarbleData> playerDeck)
    {
        NodeManagerData.UpdatePlayerDeck(playerDeck);
    }
    [SerializeField]
    private NodeManagerSO NodeManagerData;
    [SerializeField]
    private GameObject UIPrefab;
    [SerializeField]
    private GameObject ConnectingLine;
    private static bool bHasInitialized = false;
    public static List<int> TraversedNodes = new List<int>();
    private List<List<GameObject>> LevelsUI;
    private Dictionary<int, Node> LevelNumToNodeComp = new Dictionary<int, Node>();
    [SerializeField]
    private Transform StartingPosition;
    [SerializeField]
    private ScrollRect ScrollZone;
    [SerializeField]
    private float VerticalOffset = 2.0f;
    [SerializeField]
    private float HorizontalOffset = 2.0f;
    private float AdaptedVerticalOffset;
    private float AdaptedHorizontalOffset;
    [SerializeField]
    private float Padding = 20.0f;
    [SerializeField]
    private UILineRenderer UILinePrefab;
    // Container to hold the entire map
    private GameObject MapContainer;
    // Parent that actually does all the map information
    private GameObject MapParent;
    private bool bHasUIBeenInitialized = false;
    // Number of "Layers" that the map has
    private int Layers = 0;
    private static Vector2 previousScrollPosition = new Vector2(0, 0.5f);

    private void Awake()
    {
        if (Instance != null && Instance != this)
            Destroy(this.gameObject);
        else
        {
            DontDestroyOnLoad(gameObject);
            Instance = this;
        }
    }
    private void OnLevelWasLoaded(int level)
    {
        // if level is the map level, we reinitialize
        if (level == 2)
        {
            if (!bHasUIBeenInitialized)
            {
                if (!bHasInitialized)
                {
                    NodeManagerData.InitializeLevelData();
                    bHasInitialized = true;
                }
                Initialization();
                bHasUIBeenInitialized = true;
            }
        }
        else if (level == 1)
        {
            bHasUIBeenInitialized = false;
        }
        else
        {
            // Assumes we are going back to title screen which means that we should reinitialize levels bc "roguelike"
            bHasInitialized = false;
            NodeManagerData.ClearPlayerDeck();
            TraversedNodes.Clear();
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        if (!bHasUIBeenInitialized)
        {
            if (!bHasInitialized)
            {
                NodeManagerData.InitializeLevelData();
                bHasInitialized = true;
            }
            Initialization();
            bHasUIBeenInitialized = true;
        }
    }
    private void OnDestroy()
    {
    }
    private void Initialization()
    {
        if (!StartingPosition)
        {
            GameObject StartingPosGO = GameObject.Find("StartingPosition");
            if (StartingPosGO)
            {
                StartingPosition = StartingPosGO.transform;
            }
        }
        if (!ScrollZone)
        {
            GameObject ScrollZoneGO = GameObject.Find("Scroll View");
            if (ScrollZoneGO)
            {
                ScrollZone = ScrollZoneGO.GetComponent<ScrollRect>();
            }
        }
        // Zero Out StartingPosition Z just in case
        if (StartingPosition.position.z != 0)
        {
            StartingPosition.position.Set(StartingPosition.position.x, StartingPosition.position.y, 0);
        }
        LevelsUI = new List<List<GameObject>>();
        Layers = 0;

        // Adapt offsets based on screen size
        AdaptedVerticalOffset = VerticalOffset * (Screen.height / 1080f);
        AdaptedHorizontalOffset = HorizontalOffset * (Screen.width / 1920f);
        
        InitializeParentContainer();
        PopulateMapData();
        ConnectMap();
        UpdateMapToLatest();
    }
    private void OnEnable()
    {
        Node.OnAttemptEnterLevel += DoAttemptEnterLevel;
    }
    private void OnDisable()
    {
        Node.OnAttemptEnterLevel -= DoAttemptEnterLevel;

    }
    private void InitializeParentContainer()
    {
        RectTransform ScrollZoneTransform = ScrollZone.content;

        // Setup Map Container
        MapContainer = new GameObject("MapContainer");
        MapContainer.transform.SetParent(ScrollZoneTransform);
        MapContainer.transform.localScale = Vector3.one;
        RectTransform MapContainerTransform = MapContainer.AddComponent<RectTransform>();
        Stretch(MapContainerTransform);

        // Init Map Parent
        MapParent = new GameObject("MapParent");
        MapParent.transform.SetParent(MapContainer.transform);
        MapParent.transform.localScale = Vector3.one;
        RectTransform MapParentTransform = MapParent.AddComponent<RectTransform>();
        Stretch(MapParentTransform);
    }

    /// <summary>
    /// Create a <c>Node</c> for each level and position them in the scene.
    /// </summary>
    private void PopulateMapData()
    {
        // Hardcoding :P
        int[] CapacitiesByLayer = new int[]{1, 2, 4, 2, 1};
        Layers = CapacitiesByLayer.Length;
        
        // Populate layers with nodes
        List<LevelDataSO> Levels = NodeManagerData.GetLevels();
        int currLevel = 0;
        for (int layer = 0; layer < Layers; ++layer)
        {
            LevelsUI.Add(new List<GameObject>());

            for (int levelInLayer = 0; levelInLayer < CapacitiesByLayer[layer]; ++levelInLayer) 
            {
                // Update Node data
                GameObject NodePrefab = Instantiate(UIPrefab, MapParent.transform, false);
                Node nodeComp = NodePrefab.GetComponent<Node>();
                nodeComp.SetLayer(layer);
                nodeComp.SetCorrespondingLevelSO(currLevel);
                nodeComp.CalculateDefaultColor(Levels[currLevel].GetLevelDifficulty());
                nodeComp.UpdateNameOfNode(Levels[currLevel].GetEnemyName());

                if (LevelNumToNodeComp.TryGetValue(currLevel, out _))
                {
                    LevelNumToNodeComp.Remove(currLevel);
                }
                LevelNumToNodeComp.TryAdd(currLevel, nodeComp);

                // Add to layer
                LevelsUI[layer].Add(NodePrefab);
                currLevel++;
            }
        }

        // Update node position based on offsets to form a tree structure
        for (int layer = 0; layer < Layers; ++layer)
        {
            for (int levelInLayer = 0; levelInLayer < CapacitiesByLayer[layer]; ++levelInLayer) 
            {
                float verticalOffset =  AdaptedVerticalOffset * (levelInLayer - (CapacitiesByLayer[layer] - 1) / 2.0f);
                Vector3 newOffset = new Vector3(layer * AdaptedHorizontalOffset, verticalOffset);
                LevelsUI[layer][levelInLayer].GetComponent<Node>().transform.position = StartingPosition.position + newOffset;
            }
        }

        ResizeScrollView();
    }

    /// <summary>
    /// Generate edge relationships between Nodes on adjacent layers and create 
    /// <c>UILinePrefab</c>s for each edge.
    /// </summary>
    private void ConnectMap()
    {
        List<LevelDataSO> Levels = NodeManagerData.GetLevels();

        // For the first index, we want two children so we just hardcode this 
        // in. Lowkey... if it's a map of size 2 then like it'll break but 
        // whatever lol
        Assert.IsTrue(LevelsUI[0].Count == 1);
        Assert.IsTrue(LevelsUI[1].Count == 2);

        // Draw first layer
        Node BaseNode = LevelsUI[0][0].GetComponent<Node>();
        Node LeftChildNode = LevelsUI[1][0].GetComponent<Node>();
        Node RightChildNode = LevelsUI[1][1].GetComponent<Node>();
        DrawLines(BaseNode, LeftChildNode, true);
        DrawLines(BaseNode, RightChildNode, true);

        // Set icon for level 1
        BaseNode.ShowLevel1Icon();

        // From second layer, Only traverse up to third to last layer
        for (int i = 1; i < LevelsUI.Count - 2; ++i)
        {
            float ProbToDrawLine = 0.6f;
            for (int j = 0; j < LevelsUI[i].Count; ++j)
            {
                Node UINode = LevelsUI[i][j].GetComponent<Node>();
                for (int k = 0; k < LevelsUI[i + 1].Count; ++k)
                {
                    Node ChildNode = LevelsUI[i + 1][k].GetComponent<Node>();
                    if (Random.Range(0f, 1f) <= ProbToDrawLine || 
                        !UINode.GetHasChildren() || 
                        ChildNode.GetNumParents() < 1)
                    {
                        DrawLines(UINode, ChildNode, true);
                        ChildNode.IncrementNumParents();
                        UINode.SetHasChildren(true);
                    }
                }
                ProbToDrawLine -= Random.Range(0.05f, 0.4f);
                ProbToDrawLine = Mathf.Clamp(ProbToDrawLine, 0.3f, 1.0f);
            }
        }
        // From second to last layer to last layer just attach each 
        int secondToLast = LevelsUI.Count - 2;
        int lastLayer = LevelsUI.Count - 1;
        for (int i = 0; i < LevelsUI[secondToLast].Count; ++i)
        {
            Node UINode = LevelsUI[secondToLast][i].GetComponent<Node>();
            Node ChildNode = LevelsUI[lastLayer][0].GetComponent<Node>();
            DrawLines(UINode, ChildNode, true);
            ChildNode.IncrementNumParents();
            UINode.SetHasChildren(true);
        }
    }
    public void DrawLines(Node UINode, Node NextNode, bool bIsLeft)
    {
        Vector3 startPos = UINode.GetNextNode().position;
        UILineRenderer LineRenderer = Instantiate(UILinePrefab, MapParent.transform);
        LineRenderer.transform.SetAsFirstSibling();
        LineRenderer.SetPoints(startPos, NextNode.GetPreviousNode().position);
        LineRenderer.AdjustDimensions();

        UINode.SetHasChildren(true);
        UINode.AddChild(NextNode, LineRenderer);
        NextNode.AddParent(UINode, LineRenderer);
    }

    /// <summary>
    /// Traverse the <c>TraversedNodes</c> and update node and edge colors based on whether a level has been visited.
    /// </summary>
    private void UpdateMapToLatest()
    {
        if (TraversedNodes.Count == 0)
        {
            return;
        }

        int currLayer = 0;
        if (LevelNumToNodeComp.TryGetValue(TraversedNodes[TraversedNodes.Count - 1], out Node currNode))
        {
            currNode.MarkTraversed();
            currLayer = currNode.GetLayer();
        }

        // Mark cleared nodes on other layers.
        for (int i = 0; i < TraversedNodes.Count - 1; ++i)
        {
            if (LevelNumToNodeComp.TryGetValue(TraversedNodes[i], out Node val))
            {
                if (LevelNumToNodeComp.TryGetValue(TraversedNodes[i + 1], out _))
                {
                    val.MarkTraversed();
                }
            }
        }

        // Mark all untraversed nodes on current or lower layers as inaccessible
        foreach (Node node in LevelNumToNodeComp.Values) {
            if (node.GetLayer() <= currLayer) {
                node.MarkInaccessible();
            }
        }
    }

    private void ResizeScrollView()
    {
        int dynamicPaddingWidth = Layers * 3;
        int dynamicPaddingHeight = Layers * 2;
        Vector2 sizeDelta = ScrollZone.content.sizeDelta;
        RectTransform rectTransform = UIPrefab.GetComponent<RectTransform>();
        float HorizontalLength = Padding + rectTransform.rect.width * dynamicPaddingWidth;
        float VerticalLength = Padding + rectTransform.rect.height * dynamicPaddingHeight;
        sizeDelta.x = HorizontalLength;
        sizeDelta.y = VerticalLength;
        ScrollZone.content.sizeDelta = sizeDelta;
        ScrollZone.normalizedPosition = previousScrollPosition;
    }

    private void DoAttemptEnterLevel(int level)
    {
        List<LevelDataSO> Levels = NodeManagerData.GetLevels();
        if (level >= Levels.Count)
        {
            // not a valid index
            return;
        }
        if (TraversedNodes.Count == 0)
        {
            if (level != 0)
            {
                // We don't want to get anything except the first level if we haven't done anything yet
                return;
            }
            else
            {
                Levels[level].SetIsLevelVisited(true);
            }
        }
        else
        {
            // check to see if the latest node can connect to this input level
            LevelNumToNodeComp.TryGetValue(TraversedNodes[TraversedNodes.Count - 1], out Node LatestNode);
            Node ProspectiveNode = null;
            if (LevelNumToNodeComp.TryGetValue(level, out Node val))
            {
                ProspectiveNode = val;
            }
            else
            {
                return;
            }
            if (LatestNode.GetChildren().Contains(ProspectiveNode))
            {
                Levels[level].SetIsLevelVisited(true);
            }
            else
            {
                return;
            }
        }
        // If we get here we have a correct level;
        // Set the gamemanager's reference to leveldataSO
        // Load into gameplay scene
        previousScrollPosition = ScrollZone.normalizedPosition;
        TraversedNodes.Add(level);
        NodeManagerData.SetActiveLevel(level);
        SceneManagerScript.Instance.loadSceneByIndex(1);
    }
    
    /// UTILITY FUNCS
    private static void Stretch(RectTransform Transform)
    {
        Transform.localPosition = Vector3.zero;
        Transform.anchorMin = Vector2.zero;
        Transform.anchorMax = Vector2.one;
        Transform.sizeDelta = Vector2.zero;
        Transform.anchoredPosition = Vector2.zero;
    }
}
