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
        Levels.Clear();
        // Testing Purposes just add in like 5 of first index
        for (int i = 0; i < NumberLevels; i++)
        {
            Levels.Add(PossibleLevels[Random.Range(0, PossibleLevels.Count - 1)]);
        }
    }
    public void SetActiveLevel(int index) { ActiveLevel = index; }
    public LevelDataSO GetActiveLevel() { return Levels[ActiveLevel]; }
    public List<LevelDataSO> GetLevels() { return Levels; }
    [SerializeField]
    private int NumberLevels = 5;
    [SerializeField, Tooltip("Insert all LevelDataSO's into this list. We will pseudo randomly pick out levels to use")]
    private List<LevelDataSO> PossibleLevels = new List<LevelDataSO>();
    [SerializeField]
    private List<LevelDataSO> Levels = new List<LevelDataSO>();
    private int ActiveLevel = 0;
}
