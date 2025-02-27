using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum MarbleType
{
    DEFAULT,
    EXPLOSIVE,
    GHOST,
    SPLITTER,
    BLACKHOLE,
    THICC
}
public class MarbleData : ScriptableObject
{
    // Type of marble
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
    [SerializeField]
    private MarbleType marbleType = MarbleType.DEFAULT;

    // not sure what other stuff we need... 
}
