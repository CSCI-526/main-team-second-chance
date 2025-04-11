using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

[CreateAssetMenu(fileName = "NodeManagerSO", menuName = "ScriptableObjects/NodeManagerSO")]
public class NodeManagerSO : ScriptableObject
{
    public void InitializeLevelData()
    {
        if (PossibleLevels.Count == 0)
        {
            return;
        }
        Debug.Log(PossibleLevels.Count);
        Levels.Clear();
        // Testing Purposes just add in like 5 of first index
        int firstThird = (int)(NumberLevels * (1.0f / 3.0f));
        int secondThird = (int)(NumberLevels * (2.0f / 3.0f));
        // this kinda assumes that levels are added in based on difficulty this is super hardcoded
        for (int i = 0; i < NumberLevels; i++)
        {
            if (i < firstThird)
            {
                Levels.Add(PossibleLevels[Random.Range(0, firstThird)]);
            }
            else if (i < secondThird)
            {
                Levels.Add(PossibleLevels[Random.Range(firstThird, secondThird)]);
            }
            else
            {
                Levels.Add(PossibleLevels[Random.Range(secondThird, PossibleLevels.Count)]);
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
    [SerializeField]
    private int NumberLevels = 5;
    [SerializeField, Tooltip("Insert all LevelDataSO's into this list. We will pseudo randomly pick out levels to use")]
    private List<LevelDataSO> PossibleLevels = new List<LevelDataSO>();
    [SerializeField]
    private List<LevelDataSO> Levels = new List<LevelDataSO>();
    private int ActiveLevel = 0;
    private List<MarbleData> PlayerDeck = new List<MarbleData>();
}
