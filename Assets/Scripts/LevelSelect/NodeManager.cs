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
        // Debug.Log("NodeManager.Awake() " + gameObject.GetInstanceID());
        if (Instance)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }
    private void OnLevelWasLoaded(int sceneNum)
    {
        if (Instance != this) 
        {
            return;
        }
        
        switch (sceneNum) {
            // Marbles scene
            case 1:
            {
                // Force level select UI redraw on next visit.
                bHasUIBeenInitialized = false;
                break; 
            }
            
            // LevelSelect scene
            case 2:
            {
                if (!bHasUIBeenInitialized)
                {
                    if (!bHasInitialized)
                    {
                        NodeManagerData.InitializeLevelData();
                        CreateLevelGraph();
                        bHasInitialized = true;
                    }
                    DrawLevelGraph();
                    UpdateMapToLatest();
                    bHasUIBeenInitialized = true;
                }
                break;
            }

            // Title scene
            default:
            {
                bHasInitialized = false;
                NodeManagerData.ClearPlayerDeck();
                TraversedNodes.Clear();
                break;
            }
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        if (Instance != this) 
        {
            return;
        }
    }

    private void OnEnable()
    {
        Node.OnAttemptEnterLevel += DoAttemptEnterLevel;
    }

    private void OnDisable()
    {
        Node.OnAttemptEnterLevel -= DoAttemptEnterLevel;
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

    private void DoAttemptEnterLevel(int level)
    {
        List<LevelDataSO> Levels = NodeManagerData.GetLevels();
        if (level >= Levels.Count || level < 0 || (TraversedNodes.Count == 0 && level != 0))
        {
            return;
        }

        bool validLevel = false;
        if (TraversedNodes.Count == 0)
        {
            Levels[level].SetIsLevelVisited(true);
            validLevel = true;
        }
        else
        {
            // Check to see if the latest node can connect to this input level
            int lastVisitedLevel = TraversedNodes[TraversedNodes.Count - 1];
            LevelNumToNodeComp.TryGetValue(lastVisitedLevel, out Node lastTraversedNode);
            
            if (LevelNumToNodeComp.TryGetValue(level, out Node node))
            {
                if (lastTraversedNode.GetChildren().Contains(node))
                {
                    Levels[level].SetIsLevelVisited(true);
                    validLevel = true;
                }
            }
        }

        if (!validLevel)
        {
            return;
        }

        previousScrollPosition = ScrollZone.normalizedPosition;
        TraversedNodes.Add(level);
        NodeManagerData.SetActiveLevel(level);
        SceneManagerScript.Instance.loadSceneByIndex(1);
    }

    int[] LevelGraphCapacitiesByLayer = new int[]{1, 2, 4, 2, 1};
    private List<List<NodeInfo>> levelGraph = new List<List<NodeInfo>>();

    private class NodeInfo 
    {
        public GameObject gameObject;
        public Node node;
        public int level;
        public int layer;
        public List<NodeInfo> parents = new List<NodeInfo>();
        public List<NodeInfo> children = new List<NodeInfo>();
    }

    /// <summary>
    /// Initialize and store level data. Randomly generate edges between levels.
    /// </summary>
    private void CreateLevelGraph() 
    {
        levelGraph.Clear();
        Layers = LevelGraphCapacitiesByLayer.Length;
        
        // Populate graph with nodes.
        int currLevel = 0;
        for (int layer = 0; layer < Layers; ++layer)
        {
            levelGraph.Add(new List<NodeInfo>());
            for (int levelInLayer = 0; levelInLayer < LevelGraphCapacitiesByLayer[layer]; ++levelInLayer) 
            {
                NodeInfo nodeInfo = new NodeInfo();
                nodeInfo.layer = layer;
                nodeInfo.level = currLevel;
                levelGraph[layer].Add(nodeInfo);
                currLevel++;
            }
        }

        // Generate edges between levels.
        /*
            Start level edges
            [0,0]
              |____[1,0]
              |____[1,1] 
        */
        MakeEdges(
            levelGraph[0][0], 
            new NodeInfo[]{levelGraph[1][0], levelGraph[1][1]}
        );

        /*
            Last level edges
            [3,0]____
            [3,1]____|
                     |
                   [4,0]
        */
        MakeEdges(levelGraph[Layers-2][0], new NodeInfo[]{levelGraph[Layers-1][0]});
        MakeEdges(levelGraph[Layers-2][1], new NodeInfo[]{levelGraph[Layers-1][0]});

        // In-between layer edges.
        for (int layer = 1; layer < Layers-2; ++layer)
        {
            float ProbToDrawLine = 0.6f;
            for (int levelInLayer = 0; levelInLayer < LevelGraphCapacitiesByLayer[layer]; ++levelInLayer)
            {
                NodeInfo parent = levelGraph[layer][levelInLayer];
                for (int k = 0; k < levelGraph[layer + 1].Count; ++k)
                {
                    NodeInfo child = levelGraph[layer + 1][k];
                    if (Random.Range(0f, 1f) <= ProbToDrawLine || 
                        parent.children.Count == 0 || 
                        child.parents.Count < 1)
                    {
                        MakeEdges(parent, new NodeInfo[]{child});
                    }
                }
                ProbToDrawLine -= Random.Range(0.05f, 0.4f);
                ProbToDrawLine = Mathf.Clamp(ProbToDrawLine, 0.3f, 1.0f);
            }
        }
    }

    private void MakeEdges(NodeInfo parent, NodeInfo[] children) {
        parent.children.AddRange(children);
        foreach (NodeInfo child in children) {
            child.parents.Add(parent);
        }
    }

    /// <summary>
    /// Use <c>levelGraph</c> to draw the graph displayed in LevelSelect.
    /// </summary>
    private void DrawLevelGraph()
    {
        // Adapt offsets based on screen size
        AdaptedVerticalOffset = VerticalOffset * (Screen.height / 1080f);
        AdaptedHorizontalOffset = HorizontalOffset * (Screen.width / 1920f);

        // Destroyed if leaving LevelSelect scene, need to find again.
        if (!StartingPosition)
        {
            StartingPosition = GameObject.Find("StartingPosition").transform;
        }
        if (!ScrollZone)
        {
            ScrollZone = GameObject.Find("Scroll View").GetComponent<ScrollRect>();
        }

        InitializeParentContainer();
        DrawNodes();
        ResizeScrollView();
        DrawEdges();
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

    private static void Stretch(RectTransform Transform)
    {
        Transform.localPosition = Vector3.zero;
        Transform.anchorMin = Vector2.zero;
        Transform.anchorMax = Vector2.one;
        Transform.sizeDelta = Vector2.zero;
        Transform.anchoredPosition = Vector2.zero;
    }

    private void DrawNodes() {
        LevelNumToNodeComp.Clear();
        List<LevelDataSO> Levels = NodeManagerData.GetLevels();
        for (int layer = 0; layer < Layers; ++layer)
        {
            for (int levelInLayer = 0; levelInLayer < LevelGraphCapacitiesByLayer[layer]; ++levelInLayer) 
            {
                NodeInfo nodeInfo = levelGraph[layer][levelInLayer];
                nodeInfo.gameObject = Instantiate(UIPrefab, MapParent.transform, false);
                nodeInfo.node = nodeInfo.gameObject.GetComponent<Node>();

                Node nodeComp = nodeInfo.gameObject.GetComponent<Node>();
                nodeComp.SetLayer(nodeInfo.layer);
                nodeComp.SetCorrespondingLevelSO(nodeInfo.level);
                nodeComp.CalculateDefaultColor(Levels[nodeInfo.level].GetLevelDifficulty());
                nodeComp.UpdateNameOfNode(Levels[nodeInfo.level].GetEnemyName());

                LevelNumToNodeComp.TryAdd(nodeInfo.level, nodeComp);

                // Set position based on layer capacity.
                float verticalOffset =  AdaptedVerticalOffset * (levelInLayer - (LevelGraphCapacitiesByLayer[layer] - 1) / 2.0f);
                Vector3 newOffset = new Vector3(layer * AdaptedHorizontalOffset, verticalOffset);
                nodeComp.transform.position = StartingPosition.position + newOffset;
            }
        }

        levelGraph[0][0].node.ShowLevel1Icon();
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

    private void DrawEdges() {
        for (int layer = 0; layer < Layers; ++layer)
        {
            for (int levelInLayer = 0; levelInLayer < LevelGraphCapacitiesByLayer[layer]; ++levelInLayer) 
            {
                NodeInfo parent = levelGraph[layer][levelInLayer];
                foreach(NodeInfo child in parent.children) 
                {
                    DrawLines(parent.node, child.node);
                }
            }
        }
    }

    /// <summary>
    /// Update Node parent-child relationship and draws a line between them.
    /// </summary>
    public void DrawLines(Node parent, Node child)
    {
        Vector3 startPos = parent.GetNextNode().position;
        UILineRenderer LineRenderer = Instantiate(UILinePrefab, MapParent.transform);
        LineRenderer.transform.SetAsFirstSibling();
        LineRenderer.SetPoints(startPos, child.GetPreviousNode().position);
        LineRenderer.AdjustDimensions();

        parent.SetHasChildren(true);
        parent.AddChild(child, LineRenderer);
        child.AddParent(parent, LineRenderer);
    }
}
