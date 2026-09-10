using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace Editor
{
    public class DebugLevelData : ScriptableObject
    {
        public List<MarbleData> playerDeck;
        
        [FormerlySerializedAs("EnemyDifficulty")] [SerializeField, Range(0.0f, 10.0f), Tooltip("How accurate the enemy will shoot")]
        public float enemyDifficulty = 1.0f;
        [FormerlySerializedAs("EnemyAggressionLevel")] [SerializeField]
        public AggressionLevel enemyAggressionLevel;
        [FormerlySerializedAs("EnemyDeckType")] [SerializeField]
        public EnemyDeckType enemyDeckType = EnemyDeckType.DEFAULT;
        [FormerlySerializedAs("EnemyName")] [SerializeField]
        public string enemyName = "The Defaulter";
        // maybe we might want to modify how many like marbles also in here which could b cool 
        [SerializeField] public int arena = 0;

        // Overall Rating of the level, 1 being easiest, 5 being hardest
        [FormerlySerializedAs("LevelDifficulty")] [SerializeField, Range(1, 5), Tooltip("Overall rating of the level, 1 being the easiest, 5 being the hardest")]
        public int levelDifficulty = 1;
    }
}
