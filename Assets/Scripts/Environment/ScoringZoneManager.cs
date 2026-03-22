using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class ScoringZoneManager : MonoBehaviour
{
    public static event Action<Marble,ScoringCircle,bool> OnZoneStatusChange;
    public static void ZoneStatusChange(Marble marble, ScoringCircle circle, bool insideZone)
    {
        OnZoneStatusChange?.Invoke(marble,circle,insideZone);
    }

    private Dictionary<Marble, HashSet<ScoringCircle>> _marblesStates = new Dictionary<Marble, HashSet<ScoringCircle>>();
    private List<ScoringCircle> _activeScoringCircles = new List<ScoringCircle>();
    [SerializeField] private GameObject[] arenaGameObjects;

    private void OnEnable()
    {
        OnZoneStatusChange += UpdateMarbleState;
        MarbleEvents.OnMarbleSpawned += OnMarbleSpawned;
    }

    private void OnDisable()
    {
        OnZoneStatusChange -= UpdateMarbleState;
        MarbleEvents.OnMarbleSpawned -= OnMarbleSpawned;
    }
    
    
    private void UpdateMarbleState(Marble marble, ScoringCircle circle, bool insideZone)
    {
        if(_marblesStates.TryGetValue(marble, out HashSet<ScoringCircle> scoringCircles))
        {
            if (insideZone)
            {
                scoringCircles.Add(circle);
            }
            else
            {
                scoringCircles.Remove(circle);
            }
        }
        
        CalculateMarbleState(marble);
    }

    private void CalculateMarbleState(Marble marble)
    {
        bool prevScoringCircleState = marble.bIsInsideScoringCircle;

        if (_marblesStates.TryGetValue(marble, out HashSet<ScoringCircle> scoringCircles))
        {
            int highestPrio = -1;
            ZoneType type = ZoneType.Launch;

            foreach (var scoringCircle in scoringCircles)
            {
                if (scoringCircle.Priority > highestPrio)
                {
                    highestPrio = scoringCircle.Priority;
                    type = scoringCircle.Type;
                }
            }
            
            marble.bIsInsideScoringCircle = type == ZoneType.Scoring;
        }

        if (marble.bIsInsideScoringCircle != prevScoringCircleState)
        {
            GameManager.Instance.UpdateEntityScore(marble.Team, marble.bIsInsideScoringCircle);
        }
    }

    private void OnMarbleSpawned(Marble marble)
    {
        // need to check if spawning inside of an area already
        HashSet<ScoringCircle> scoringCircles = new HashSet<ScoringCircle>();
        

        Collider marbleCollider = marble.GetComponent<Collider>();
        
        foreach (var scoringCircle in _activeScoringCircles)
        {
            Collider scoringCollider = scoringCircle.GetComponent<Collider>();

            var transform1 = scoringCollider.transform;
            var transform2 = marble.transform;
            bool collision = Physics.ComputePenetration(marbleCollider, transform2.position, transform2.rotation,
                scoringCollider, transform1.position, transform1.rotation,
                out Vector3 direction, out float distance);

            if (collision)
            {
                scoringCircles.Add(scoringCircle);
            }
        }
        
        _marblesStates.TryAdd(marble, scoringCircles);
        CalculateMarbleState(marble);
    }

    // allows for a hard reset of the marble map to stay space efficient
    public void ClearMarbleStates()
    {
        _marblesStates.Clear();
    }

    public void SetArena(int index)
    {
        _activeScoringCircles.Clear();
        arenaGameObjects[index].SetActive(true);
        _activeScoringCircles.AddRange(arenaGameObjects[index].GetComponentsInChildren<ScoringCircle>());
        _activeScoringCircles.Sort((a,b) => b.Priority.CompareTo(a.Priority)); // sort descending
    }

    public void SetScoringCircleScales(float t)
    {
        foreach (var scoringCircle in _activeScoringCircles)
        {
            scoringCircle.SetScoringRadius(t);
        }
    }

    public bool CheckValidLaunchZone(Vector3 launchPosition)
    {
        // project the position down
        launchPosition.y = 0.0f;

        for(int i = 0; i < _activeScoringCircles.Count; ++i)
        {
            Collider circleCollider = _activeScoringCircles[i].GetComponent<Collider>();
            if (circleCollider != null)
            {
                Vector3 closestPoint = circleCollider.ClosestPoint(launchPosition);
                if ((closestPoint-launchPosition).sqrMagnitude < 0.01f)
                {
                    if (_activeScoringCircles[i].Type == ZoneType.Blocked ||
                        _activeScoringCircles[i].Type == ZoneType.Scoring)
                    {
                        return false;
                    }

                    return true;
                }
            }
        }

        return true;
    }

    public Collider GetDefaultScoringZone()
    {
        List<ScoringCircle> circle = _activeScoringCircles.FindAll((zone) => zone.Type == ZoneType.Scoring);
        return circle[Random.Range(0,circle.Count)].GetComponent<Collider>();
    }
}
