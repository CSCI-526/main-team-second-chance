using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewLevelData", menuName = "ScriptableObjects/LevelData")]
public class LevelDataSO : ScriptableObject
{
    public float GetEnemyDifficulty() { return EnemyDifficulty; }
    public AggressionLevel GetAggressionLevel() { return EnemyAggressionLevel; }
    public EnemyDeckType GetEnemyDeckType() { return EnemyDeckType; }
    public bool GetIsLevelVisited() { return bIsLevelVisited; }
    public void SetIsLevelVisited(bool value) { bIsLevelVisited = value; }
    public int GetLevelDifficulty() { return LevelDifficulty; }
    public string GetEnemyName() { return EnemyName; }
    [SerializeField, Range(0.0f, 10.0f), Tooltip("How accurate the enemy will shoot")]
    private float EnemyDifficulty = 1.0f;
    [SerializeField]
    private AggressionLevel EnemyAggressionLevel;
    [SerializeField]
    private EnemyDeckType EnemyDeckType = EnemyDeckType.DEFAULT;
    [SerializeField]
    private string EnemyName = "The Defaulter";
    // maybe we might want to modify how many like marbles also in here which could b cool 

    // Overall Rating of the level, 1 being easiest, 5 being hardest
    [SerializeField, Range(1, 5), Tooltip("Overall rating of the level, 1 being the easiest, 5 being the hardest")]
    private int LevelDifficulty = 1;

    private bool bIsLevelVisited = false;
}
