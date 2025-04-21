using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

[CreateAssetMenu(fileName = "NodeManagerSO", menuName = "ScriptableObjects/NodeManagerSO")]
public class NodeManagerSO : ScriptableObject
{
    public void InitializeLevelData()
    {
        if (PossibleEasyLevels.Count == 0 || PossibleMediumLevels.Count == 0 || PossibleHardLevels.Count == 0)
        {
            Debug.LogError("NodeManagerSO contains a list of levels that is empty. Please add in the correct levels.");
            return;
        }

        Levels.Clear();

        // Testing Purposes just add in like 5 of first index
        int firstThird = (int)(NumberLevels * (1.0f / 3.0f));
        int secondThird = (int)(NumberLevels * (2.0f / 3.0f));
        for (int i = 0; i < NumberLevels; i++)
        {
            if (i < firstThird)
            {
                Levels.Add(PossibleEasyLevels[Random.Range(0, PossibleEasyLevels.Count)]);
            }
            else if (i < secondThird)
            {
                Levels.Add(PossibleMediumLevels[Random.Range(0, PossibleMediumLevels.Count)]);
            }
            else
            {
                Levels.Add(PossibleHardLevels[Random.Range(0, PossibleHardLevels.Count)]);
            }
        }
    }
    public void SetActiveLevel(int index) { ActiveLevel = index; }
    public LevelDataSO GetActiveLevel() { return Levels[ActiveLevel]; }
    public List<LevelDataSO> GetLevels() { return Levels; }
    public List<MarbleData> GetPlayerDeck() { return PlayerDeck; }
    public void UpdatePlayerDeck(List<MarbleData> playerDeck) { PlayerDeck = playerDeck; }
    public void ClearPlayerDeck()
    {
        PlayerDeck.Clear();
    }
    public void RemoveCardFromPlayerDeck(int Index)
    {
        PlayerDeck.RemoveAt(Index);
    }
    [SerializeField]
    private int NumberLevels = 5;
    [SerializeField, Tooltip("Possible Easy Levels")]
    private List<LevelDataSO> PossibleEasyLevels = new List<LevelDataSO>();
    [SerializeField, Tooltip("Possible Medium Levels")]
    private List<LevelDataSO> PossibleMediumLevels = new List<LevelDataSO>();
    [SerializeField, Tooltip("Possible Hard Levels")]
    private List<LevelDataSO> PossibleHardLevels = new List<LevelDataSO>();

    [SerializeField]
    private List<LevelDataSO> Levels = new List<LevelDataSO>();
    private int ActiveLevel = 0;
    private List<MarbleData> PlayerDeck = new List<MarbleData>();
}
