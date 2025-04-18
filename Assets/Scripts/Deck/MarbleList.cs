using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "NewMarbleList", menuName = "ScriptableObjects/MarbleList")]
public class MarbleList : ScriptableObject
{
    public List<MarbleData> MarblePrefabs;
}
