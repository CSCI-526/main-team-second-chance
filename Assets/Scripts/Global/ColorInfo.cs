using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "NewColorInfo", menuName = "ScriptableObjects/ColorInfo")]
public class ColorInfo : ScriptableObject
{
    public Color playerMarbleColor;
    public Color enemyMarbleColor;
    public Color playerOutlineColor;
    public Color enemyOutlineColor;
    public Color playerUIColor;
    public Color enemyUIColor;
}
