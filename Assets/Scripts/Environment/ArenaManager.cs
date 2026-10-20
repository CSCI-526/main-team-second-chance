using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class ArenaManager : MonoBehaviour
{
    private Func<Coroutine> _roundStartSequence;
    protected void OnEnable()
    {
        _roundStartSequence += OnRoundStart;
        GameManager.RoundStartSequences.Add(_roundStartSequence);
    }

    protected virtual Coroutine OnRoundStart()
    {
        return null;
    }
}
