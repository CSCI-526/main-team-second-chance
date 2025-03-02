using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum MarbleTeam
{
    Player,
    Enemy
}

public class Marble : MonoBehaviour
{
    [SerializeField]
    private MarbleData marbleData;

    public MarbleTeam Team;
}
