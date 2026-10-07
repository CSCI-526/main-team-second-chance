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
    public Color accessibleLevelOutlineColor;

    public Color[] getLevelColorsByDifficulty()
    {
        return levelColorsByDifficulty;
    }

    public bool ShouldRestartOrMenu()
    {
        return NodeManagerData.GetNumberFloors() == _activeNode.layer + 1;
    }
    public LevelDataSO GetLevelData()
    {
        return _activeNode.level;
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

    public int GetPlayerCurHealth()
    {
        return NodeManagerData.GetPlayerHealth();
    }
    
    public int GetPlayerMaxHealth()
    {
        return NodeManagerData.GetPlayerMaxHealth();
    }

    public void SetPlayerHealth(int newHealth)
    {
        NodeManagerData.SetPlayerHealth(newHealth);
    }
#if UNITY_EDITOR
    public void SetSaveData(NodeManagerSO newSaveData)
    {
        NodeManagerData = newSaveData;
    }

    public void SetActiveNode(NodeInfo node)
    {
        _activeNode = node;
    }
#endif
    
    [SerializeField]
    private NodeManagerSO NodeManagerData;
    [SerializeField]
    private GameObject UIPrefab;
    [SerializeField]
    private GameObject ConnectingLine;
    private static bool bHasInitialized = false;
    public static List<NodeInfo> TraversedNodes = new List<NodeInfo>();
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
    private NodeInfo _activeNode;

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
                        SetPlayerHealth(-1);
                        bHasInitialized = true;
                    }
                    DrawLevelGraph();
                    UpdateMapToLatest();
                    bHasUIBeenInitialized = true;
                    if (TraversedNodes.Count >= Layers)
                    {
                        // all levels have been cleared
                        SceneManagerScript.Instance.loadSceneByIndex(0);
                    }
                }
                break;
            }

            // Title scene
            default:
            {
                bHasInitialized = false;
                bHasUIBeenInitialized = false;
                NodeManagerData.ClearPlayerDeck();
                TraversedNodes.Clear();
                break;
            }
        }
    }

    private void OnEnable()
    {
        Node.OnAttemptEnterLevel += DoAttemptEnterLevel;
        Node.OnCheckLevelAccessibility += CheckLevelAccess;
    }

    private void OnDisable()
    {
        Node.OnAttemptEnterLevel -= DoAttemptEnterLevel;
        Node.OnCheckLevelAccessibility -= CheckLevelAccess;
    }


    /// <summary>
    /// Traverse the <c>TraversedNodes</c> and update node and edge colors based on whether a level has been visited.
    /// </summary>
    private void UpdateMapToLatest()
    {
        if (TraversedNodes.Count == 0)
        {
            levelGraph[0][0].node.SetOutlineColor(false);
            return;
        }

        int currLayer = 0;
        Node currNode = TraversedNodes[^1].node;
        currNode.MarkTraversed();
        currLayer = currNode.GetLayer();
        

        // Mark cleared nodes on other layers.
        for (int i = 0; i < TraversedNodes.Count - 1; ++i)
        {
            TraversedNodes[i].node.MarkTraversed();
        }

        // Mark all untraversed nodes on current or lower layers as inaccessible
        foreach (var levelLayer in levelGraph)
        {
            foreach (NodeInfo nodeInfo in levelLayer)
            {
                Node node = nodeInfo.node;
                if (node.GetLayer() <= currLayer)
                {
                    node.MarkInaccessible();
                }
                else if (node.GetLayer() == currLayer + 1)
                {
                    if (!CheckLevelAccess(node.GetNodeInfo()))
                        node.MarkInaccessible();
                }
                else
                {

                    bool couldBeReached = false;
                    foreach (var parent in node.GetParents())
                    {
                        if (!parent.GetIsInaccessible())
                            couldBeReached = true;
                    }

                    if (!couldBeReached)
                        node.MarkInaccessible();
                }

                node.SetOutlineColor(false);
            }
        }
    }

    private bool CheckLevelAccess(NodeInfo nodeInfo)
    {
        if (TraversedNodes.Count == 0)
        {
            return nodeInfo.layer == 0;
        }

        // Check to see if the latest node can connect to this input level
        NodeInfo lastVisitedNode = TraversedNodes[^1];

        if (lastVisitedNode.node.GetChildren().Contains(nodeInfo.node))
        {
            return true;
        }
        
        return false;
    }

    private void DoAttemptEnterLevel(NodeInfo nodeInfo)
    {
        if (!CheckLevelAccess(nodeInfo))
        {
            return;
        }

        previousScrollPosition = ScrollZone.normalizedPosition;
        TraversedNodes.Add(nodeInfo);
        //NodeManagerData.SetActiveLevel(level);
        _activeNode = nodeInfo;
        SceneManagerScript.Instance.loadSceneByIndex(1);
    }
    private List<List<NodeInfo>> levelGraph = new List<List<NodeInfo>>();

    public class NodeInfo 
    {
        public GameObject gameObject;
        public Node node;
        public LevelDataSO level;
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
        Layers = NodeManagerData.GetNumberFloors();
        
        levelGraph.Add(new List<NodeInfo>());
        {
            NodeInfo nodeInfo = new NodeInfo();
            nodeInfo.layer = 0;
            nodeInfo.level = NodeManagerData.GetNormalLevel();
            levelGraph[0].Add(nodeInfo);
        }
        
        // Populate graph with nodes.
        for (int layer = 1; layer < Layers-1; ++layer)
        {
            levelGraph.Add(new List<NodeInfo>());
            int maxLevelsPerFloor = 4;
            int floorsThisLevel = 0;
            foreach (var parentNode in levelGraph[layer-1])
            {
                int nodeChildrenCount = Random.Range(1, 3);
                nodeChildrenCount = Mathf.Clamp(nodeChildrenCount, 0, maxLevelsPerFloor - floorsThisLevel);
                for(int levelInLayer = 0; levelInLayer < nodeChildrenCount; ++levelInLayer) 
                {
                    NodeInfo nodeInfo = new NodeInfo();
                    nodeInfo.layer = layer;
                    nodeInfo.level = NodeManagerData.GetNormalLevel();
                    levelGraph[layer].Add(nodeInfo);
                    MakeEdges(parentNode,new List<NodeInfo>(){nodeInfo});
                }

                floorsThisLevel += nodeChildrenCount;
            }

            if (floorsThisLevel == 0)
            {
                NodeInfo nodeInfo = new NodeInfo();
                nodeInfo.layer = layer;
                nodeInfo.level = NodeManagerData.GetNormalLevel();
                levelGraph[layer].Add(nodeInfo);
                MakeEdges(levelGraph[layer-1][Random.Range(0,levelGraph[layer-1].Count)],new List<NodeInfo>(){nodeInfo});
            }
        }
        
        levelGraph.Add(new List<NodeInfo>());
        {
            NodeInfo nodeInfo = new NodeInfo();
            nodeInfo.layer = 0;
            nodeInfo.level = NodeManagerData.GetBossLevel();
            levelGraph[^1].Add(nodeInfo);
        }

        // Generate edges between levels.
        /*
            Start level edges
            [0,0]
              |____[1,0]
              |____[1,1] 
        */

        //MakeEdges(levelGraph[0][0], levelGraph[1]);

        /*
            Last level edges
            [3,0]____
            [3,1]____|
                     |
                   [4,0]
        */
        foreach (var nodeInfo in levelGraph[Layers-2])
        {
            MakeEdges(nodeInfo, levelGraph[Layers-1]);
        }

        // In-between layer edges.
        for (int layer = 1; layer < Layers-2; ++layer)
        {
            foreach (var nodeInfo in levelGraph[layer])
            {
                if (nodeInfo.children.Count == 0)
                {
                    int goalIndex = Mathf.Clamp(levelGraph[layer].IndexOf(nodeInfo), 0, levelGraph[layer + 1].Count-1);
                    MakeEdges(nodeInfo, new List<NodeInfo>(){levelGraph[layer+1][goalIndex]});
                }
            }
            /*
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
            */
        }
    }

    private void MakeEdges(NodeInfo parent, List<NodeInfo> children) {
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
        for (int layer = 0; layer < Layers; ++layer)
        {
            for (int levelInLayer = 0; levelInLayer < levelGraph[layer].Count; ++levelInLayer) 
            {
                NodeInfo nodeInfo = levelGraph[layer][levelInLayer];
                nodeInfo.gameObject = Instantiate(UIPrefab, MapParent.transform, false);
                nodeInfo.node = nodeInfo.gameObject.GetComponent<Node>();

                Node nodeComp = nodeInfo.gameObject.GetComponent<Node>();
                nodeComp.SetLayer(nodeInfo.layer);
                nodeComp.SetNodeInfo(nodeInfo);
                nodeComp.CalculateDefaultColor(nodeInfo.level.GetLevelDifficulty());
                nodeComp.UpdateNameOfNode(nodeInfo.level.GetEnemyName());

                // Set position based on layer capacity.
                float verticalOffset =  AdaptedVerticalOffset * (levelInLayer - (levelGraph[layer].Count - 1) / 2.0f);
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
            for (int levelInLayer = 0; levelInLayer < levelGraph[layer].Count; ++levelInLayer) 
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
