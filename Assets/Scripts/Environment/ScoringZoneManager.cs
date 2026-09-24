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
        ZoneType type = ZoneType.Launch;
        ScoringCircle highestPrio = GetHighestPriorityCircle(marble);
        if (highestPrio != null)
        {
            type = highestPrio.Type;
        }
            
        marble.bIsInsideScoringCircle = type == ZoneType.Scoring;
        
        if (marble.bIsInsideScoringCircle != prevScoringCircleState)
        {
            GameManager.Instance.UpdateEntityScore(marble.Team, marble.GetMarbleData().Points, marble.bIsInsideScoringCircle);
        }
    }

    public ScoringCircle GetHighestPriorityCircle(Marble marble)
    {
        ScoringCircle highestPrio = null;
        if (_marblesStates.TryGetValue(marble, out HashSet<ScoringCircle> scoringCircles))
        {
            foreach (var scoringCircle in scoringCircles)
            {
                if (highestPrio == null)
                {
                    highestPrio = scoringCircle;
                }
                else if (scoringCircle.Priority > highestPrio.Priority)
                {
                    highestPrio = scoringCircle;
                }
            }
        }

        return highestPrio;
    }

    private void OnMarbleSpawned(Marble marble)
    {
        // need to check if spawning inside of an area already
        HashSet<ScoringCircle> scoringCircles = new HashSet<ScoringCircle>();
        

        Collider marbleCollider = marble.GetScoringCollider();
        
        foreach (var scoringCircle in _activeScoringCircles)
        {
            Collider scoringCollider = scoringCircle.GetScoringCollider();

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
        ScoringCircle highestPrio = null;
        for(int i = 0; i < _activeScoringCircles.Count; ++i)
        {
            Collider circleCollider = _activeScoringCircles[i].GetScoringCollider();
            if (circleCollider != null)
            {
                Vector3 closestPoint = circleCollider.ClosestPoint(launchPosition);
                if ((closestPoint - launchPosition).sqrMagnitude < 0.01f)
                {
                    if (highestPrio == null)
                    {
                        highestPrio = _activeScoringCircles[i];
                    }
                    else if (_activeScoringCircles[i].Priority > highestPrio.Priority)
                    {
                        highestPrio = _activeScoringCircles[i];
                    }
                }
            }
        }
        
        if (highestPrio != null && (highestPrio.Type == ZoneType.Blocked || highestPrio.Type == ZoneType.Scoring))
        {
            return false;
        }
        
        return !Physics.Raycast(launchPosition - new Vector3(0.0f,2.0f,0.0f), Vector3.up, 5.0f, LayerMask.GetMask("Terrain"));
    }
    
    public bool CheckInScoringZone(Vector3 position)
    {
        // project the position down
        position.y = 0.0f;
        ScoringCircle highestPrio = null;
        for(int i = 0; i < _activeScoringCircles.Count; ++i)
        {
            Collider circleCollider = _activeScoringCircles[i].GetScoringCollider();
            if (circleCollider != null)
            {
                Vector3 closestPoint = circleCollider.ClosestPoint(position);
                if ((closestPoint - position).sqrMagnitude < 0.01f)
                {
                    if (highestPrio == null)
                    {
                        highestPrio = _activeScoringCircles[i];
                    }
                    else if (_activeScoringCircles[i].Priority > highestPrio.Priority)
                    {
                        highestPrio = _activeScoringCircles[i];
                    }
                }
            }
        }
        
        if (highestPrio != null && highestPrio.Type == ZoneType.Scoring)
        {
            return true;
        }
        return false;
    }

    public Collider GetDefaultScoringZone()
    {
        List<ScoringCircle> circle = _activeScoringCircles.FindAll((zone) => zone.Type == ZoneType.Scoring);
        return circle[Random.Range(0,circle.Count)].GetScoringCollider();
    }
}
