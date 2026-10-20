using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

[CreateAssetMenu(fileName = "NodeManagerSO", menuName = "ScriptableObjects/NodeManagerSO")]
public class NodeManagerSO : ScriptableObject
{
    public void InitializeLevelData()
    {
        if (PossibleEasyLevels.Count == 0 || PossibleMediumLevels.Count == 0 || PossibleEliteLevels.Count == 0 || PossibleStartLevels.Count == 0)
        {
            Debug.LogError("NodeManagerSO contains a list of levels that is empty. Please add in the correct levels.");
            return;
        }

        BossLevels.Clear();
        NormalLevels.Clear();
        EliteLevels.Clear();

        EliteLevels.AddRange(PossibleEliteLevels);
        NormalLevels.AddRange(PossibleEasyLevels);
        NormalLevels.AddRange(PossibleMediumLevels);
        
        ShuffleLevelList(NormalLevels);
        ShuffleLevelList(EliteLevels);
        // Forcibly make the first level default passive easy
        NormalLevels[0] = PossibleStartLevels[Random.Range(0, PossibleStartLevels.Count)];
        
        // add in a random boss level
        BossLevels.Add(PossibleBossLevels[Random.Range(0, PossibleBossLevels.Count)]);
    }
    
    private void ShuffleLevelList(List<LevelDataSO> levelDataSOs)
    {
        for (int i = levelDataSOs.Count - 1; i > 0; i--)
        {
            int randomIndex = Random.Range(0, i + 1);
            (levelDataSOs[i], levelDataSOs[randomIndex]) = (levelDataSOs[randomIndex], levelDataSOs[i]); // Swap
        }
    }
    
    public int GetNumberFloors() { return NumberFloors;}

    public LevelDataSO GetBossLevel()
    {
        if (BossLevels.Count == 0)
        {
            BossLevels.AddRange(PossibleBossLevels);
            ShuffleLevelList(BossLevels);
        }
        
        LevelDataSO level = BossLevels[0];
        BossLevels.RemoveAt(0);
        return level;
    }

    public LevelDataSO GetEliteLevel()
    {
        if (EliteLevels.Count == 0)
        {
            EliteLevels.AddRange(PossibleEliteLevels);
            ShuffleLevelList(EliteLevels);
        }
        
        LevelDataSO level = EliteLevels[0];
        EliteLevels.RemoveAt(0);
        return level;
    }

    public LevelDataSO GetNormalLevel()
    {
        if (NormalLevels.Count == 0)
        {
            NormalLevels.AddRange(PossibleMediumLevels);
            ShuffleLevelList(NormalLevels);
        }
        
        LevelDataSO level = NormalLevels[0];
        NormalLevels.RemoveAt(0);
        return level;
    }

    public List<MarbleData> GetPlayerDeck() { return PlayerDeck; }

    public int GetPlayerHealth() { return _playerCurHealth; }
    public int GetPlayerMaxHealth() { return _playerMaxHealth; }

    public void SetPlayerHealth(int newHealth) { _playerCurHealth = newHealth; }
    
    public void UpdatePlayerDeck(List<MarbleData> playerDeck)
    {
        PlayerDeck = new List<MarbleData>(playerDeck); 
    }
    public void ClearPlayerDeck()
    {
        PlayerDeck.Clear();
    }
    public void RemoveCardFromPlayerDeck(int Index)
    {
        PlayerDeck.RemoveAt(Index);
    }
    [SerializeField]
    private int NumberFloors = 6;
    [SerializeField, Tooltip("Possible Start Levels")]
    private List<LevelDataSO> PossibleStartLevels = new List<LevelDataSO>();
    [SerializeField, Tooltip("Possible Easy Levels")]
    private List<LevelDataSO> PossibleEasyLevels = new List<LevelDataSO>();
    [SerializeField, Tooltip("Possible Medium Levels")]
    private List<LevelDataSO> PossibleMediumLevels = new List<LevelDataSO>();
    [SerializeField, Tooltip("Possible Elite Levels")]
    private List<LevelDataSO> PossibleEliteLevels = new List<LevelDataSO>();
    [SerializeField, Tooltip("Possible Boss Levels")]
    private List<LevelDataSO> PossibleBossLevels = new List<LevelDataSO>();
    
    private List<LevelDataSO> NormalLevels = new List<LevelDataSO>();
    private List<LevelDataSO> EliteLevels = new List<LevelDataSO>();
    private List<LevelDataSO> BossLevels = new List<LevelDataSO>();
    private List<MarbleData> PlayerDeck = new List<MarbleData>();
    private int _playerCurHealth = -1;
    private int _playerMaxHealth = 30;
}
