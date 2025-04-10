using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NodeSO : ScriptableObject
{
    [SerializeField]
    private int NumberLevels = 5;
    private LevelDataSO[] Levels;
    public void InitializeLevelData()
    {
        if (Levels.Length != 0)
        {
            return;
        }

    }
}
