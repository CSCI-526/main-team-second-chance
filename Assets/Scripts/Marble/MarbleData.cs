using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum MarbleType
{
    DEFAULT,
    EXPLOSIVE,
    GHOST,
    SPLITTER,
    BLACKHOLE,
    THICC
}

// Think of the "MarbleData" ScriptableObject as a wrapper for the Marble prefab/prefab variant data
[CreateAssetMenu(fileName = "NewMarbleData", menuName = "ScriptableObjects/MarbleData")]
public class MarbleData : ScriptableObject
{
    // Prefab & Type
    [Header("Marble Identification")]
    public GameObject MarblePrefab; //Redundent under current implementation, might delete
    public MarbleType MarbleType
    {
        get
        {
            return marbleType;
        }
        set
        {
            marbleType = value;
        }
    }
    [SerializeField] private MarbleType marbleType = MarbleType.DEFAULT;
    public Image Sprite; // For later reference when we add UI/bag

    [Header("Marble Properties")]
    [Min(0.1f)] public float Mass = 1f;
    [Min(0.1f)] public float UniformScale = 1f;
    [Min(0.1f)] public float Drag = 0.7f;
    //...and whatever properties we want to individually adjust

    [Header("Ability Properties")]
    [Range(0f,10f)] public float abilityTriggerDelay = 1.5f;
    public Ability AbilityObject;

}
