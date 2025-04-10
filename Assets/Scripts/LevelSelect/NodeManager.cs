using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NodeManager : MonoBehaviour
{
    private void OnApplicationQuit()
    {
        NodeManagerData.ClearPlayerDeck();
    }
    public static NodeManager Instance;
    public bool ShouldRestartOrMenu()
    {
        int LevelsLength = NodeManagerData.GetLevels().Count;
        LevelDataSO LastLevel = NodeManagerData.GetLevels()[LevelsLength - 1];
        if(LastLevel == NodeManagerData.GetActiveLevel())
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
    private Dictionary<int, Node> DataToUIRep = new Dictionary<int, Node>();
    [SerializeField]
    private Transform StartingPosition;
    [SerializeField]
    private ScrollRect ScrollZone;
    [SerializeField]
    private float VerticalOffset = 2.0f;
    [SerializeField]
    private float HorizontalOffset = 2.0f;
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
    private float MinYPos = float.MaxValue;
    private float MaxYPos = float.MinValue;
    private float MinXPos = float.MaxValue;
    private float MaxXPos = float.MinValue;
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
        if(level == 2)
        {
            if(!bHasUIBeenInitialized)
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
        else if(level == 1)
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
        if(!bHasUIBeenInitialized)
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
            GameObject StartingPosGO= GameObject.Find("StartingPosition");
            if(StartingPosGO)
            {
                StartingPosition = StartingPosGO.transform;
            }
        }
        // Zero Out StartingPosition Z just in case
        if (StartingPosition.position.z != 0)
        {
            StartingPosition.position.Set(StartingPosition.position.x, StartingPosition.position.y, 0);
        }
        LevelsUI = new List<List<GameObject>>();
        // Create the container 
        InitializeParentContainer();

        // Populate Levels into UI Form and put into correct positions
        PopulateMapData();

        // Draw Lines Between Node Positions
        DrawMap();

        // Use Data to draw current state of affairs
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
    private void PopulateMapData()
    {
        // We know that this will be the minimum X Pos since we are moving to the right
        MinXPos = StartingPosition.transform.position.x;

        int negation = 1;
        int LayerStartingIndex = 0;
        int LayerCap = 1;
        int LayerOffsetValue = 0;
        LevelsUI.Add(new List<GameObject>());
        // Populate Levels into UI Form and Adjust Nodes to fit into tree structure
        List<LevelDataSO> Levels = NodeManagerData.GetLevels();
        for (int i = 0; i < Levels.Count - 1; ++i)
        {
            if (i - LayerStartingIndex == LayerCap)
            {
                Layers++;
                LayerCap = Mathf.NextPowerOfTwo(LayerCap + 1);
                LayerStartingIndex = i;
                negation = 1;
                LayerOffsetValue = 0;
                LevelsUI.Add(new List<GameObject>());
            }
            GameObject UIPrefabObj = Instantiate(UIPrefab, MapParent.transform, false);
            LevelsUI[Layers].Add(UIPrefabObj);
            int LayerOffset = i - LayerStartingIndex;

            Vector3 ModifiedPosition = StartingPosition.position;
            if (Layers > 0)
            {
                if (LayerOffset % 2 == 0)
                {
                    LayerOffsetValue += 1;
                }
                ModifiedPosition += new Vector3(Layers * HorizontalOffset, LayerOffsetValue * negation * VerticalOffset);
            }
            UIPrefabObj.transform.position = ModifiedPosition;
            Node UINode = UIPrefabObj.GetComponent<Node>();
            UINode.SetLayer(Layers);
            UINode.SetCorrespondingLevelSO(i);
            DataToUIRep.Add(i, UINode);
            // Store values for scroll rect
            if (ModifiedPosition.y > MaxYPos)
            {
                MaxYPos = ModifiedPosition.y;
            }
            if (ModifiedPosition.y < MinYPos)
            {
                MinYPos = ModifiedPosition.y;
            }

            negation *= -1;
        }

        // Increase Layers by 1 because Need one Last Layer
        GameObject LastUIPrefab = Instantiate(UIPrefab, MapParent.transform, false);
        Layers += 1;
        Vector3 FinalModifiedPosition = StartingPosition.position + new Vector3(Layers * HorizontalOffset, 0);
        LastUIPrefab.transform.position = FinalModifiedPosition;
        Node LastNode = LastUIPrefab.GetComponent<Node>();
        LastNode.SetCorrespondingLevelSO(Levels.Count - 1);
        LastNode.SetLayer(Layers);
        DataToUIRep.Add(Levels.Count - 1, LastNode);
        LevelsUI.Add(new List<GameObject>());
        LevelsUI[Layers].Add(LastUIPrefab);
        // Similarly, we know that this must be the maxXPos
        MaxXPos = FinalModifiedPosition.x;

        AdjustMapSize();
    }
    private void DrawMap()
    {
        List<LevelDataSO> Levels = NodeManagerData.GetLevels();

        // For the first index, we want two children so we just hardcode this in.
        // Lowkey... if it's a map of size 2 then like it'll break but whatever lol
        if (LevelsUI[0].Count != 1)
        {
            Debug.LogError("DrawMap(): LevelUI First Layer is not singular. This means something wrong has happened");
        }
        if (LevelsUI[1].Count != 2)
        {
            Debug.LogError("DrawMap(): Second Layer is not of size 2. This means somethign wrong happened");
        }
        Node BaseNode = LevelsUI[0][0].GetComponent<Node>();
        Node LeftChildNode = LevelsUI[1][0].GetComponent<Node>();
        Node RightChildNode = LevelsUI[1][1].GetComponent<Node>();
        DrawLines(BaseNode, LeftChildNode, true);
        DrawLines(BaseNode, RightChildNode, true);
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
                    if (Random.Range(0f, 1f) <= ProbToDrawLine || !UINode.GetHasChildren() || ChildNode.GetNumParents() < 1)
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
        LineRenderer.SetPoints(startPos, NextNode.GetNextNode().position);
        LineRenderer.AdjustDimensions();

        UINode.SetHasChildren(true);
        UINode.AddChild(NextNode, LineRenderer);
        NextNode.AddParent(UINode, LineRenderer);
    }
    private void UpdateMapToLatest()
    {
        // We need to traverse the LevelUI to determine whether or not which level has been visited
        if (TraversedNodes.Count == 0)
        {
            return;
        }
        for (int i = 0; i < TraversedNodes.Count - 1; ++i)
        {
            DataToUIRep.TryGetValue(TraversedNodes[i], out Node val);
            DataToUIRep.TryGetValue(TraversedNodes[i + 1], out Node val2);
            val.MarkTraversed(val2);
        }
        DataToUIRep.TryGetValue(TraversedNodes[TraversedNodes.Count - 1], out Node value);

        value.MarkTraversed(null);
    }
    private void AdjustMapSize()
    {
        RectTransform RectTransfm = ScrollZone.content;
        Vector2 sizeDelta = ScrollZone.content.sizeDelta;
        float HorizontalLength = Padding + (MaxXPos - MinXPos) / 2;
        float VerticalLength = Padding + (MaxYPos - MinYPos) / 2;
        sizeDelta.x = HorizontalLength;
        sizeDelta.y = VerticalLength;
        ScrollZone.content.sizeDelta = sizeDelta;
        ScrollZone.normalizedPosition = new Vector2(0, 0.5f);
    }

    private void DoAttemptEnterLevel(int level)
    {
        List<LevelDataSO> Levels = NodeManagerData.GetLevels();
        if(level >= Levels.Count)
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
            DataToUIRep.TryGetValue(TraversedNodes[TraversedNodes.Count - 1], out Node LatestNode);
            Node ProspectiveNode = null;
            if (DataToUIRep.TryGetValue(level, out Node val))
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
