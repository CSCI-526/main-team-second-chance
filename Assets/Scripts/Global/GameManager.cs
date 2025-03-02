using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance = null;
    public GameObject GetScoringCircle() { return ScoringCircle; }
    public bool GetAreMarblesMoving() { return bAreMarblesMoving; }
    public List<Marble> GetMarblesList() { return MarblesList; }
    public void RegisterMarble(Marble MarbleObject)
    {
        MarblesList.Add(MarbleObject);
    }

    public void RemoveMarble(Marble MarbleObject)
    {
        MarblesList.Remove(MarbleObject);
        Destroy(MarbleObject.gameObject);
    }

    public IEnumerator WaitForMarblesToSettle()
    {
        bAreMarblesMoving = true;
        yield return new WaitForSeconds(1.0f);


        while (true)
        {
            bool bMarblesSettled = true;
            foreach (Marble marble in MarblesList)
            {
                if (marble.bIsInsideGameplayCircle)
                {
                    Rigidbody physics = marble.GetComponent<Rigidbody>();
                    if (physics.velocity.sqrMagnitude > 0.1f)
                    {
                        bMarblesSettled = false;
                    }
                }
            }

            if (bMarblesSettled)
            {
                break;
            }

            yield return new WaitForSeconds(2.0f);
        }

        yield return new WaitForSeconds(1.0f);
        bAreMarblesMoving = false;
        CleanupMarbles();
    }


    [SerializeField]
    private GameObject ScoringCircle;
    private List<Marble> MarblesList = new List<Marble>();
    private List<Marble> MarblesToDelete = new List<Marble>();

    private bool bAreMarblesMoving = false;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }
    private void Start()
    {
        if (!ScoringCircle)
        {
            Debug.LogError("Scoring Circle Reference is null (GameManager)");
        }
    }

    private void CleanupMarbles()
    {
        if (MarblesList.Count != 0)
        {
            foreach (Marble marble in MarblesList)
            {
                if (!marble.bIsInsideScoringCircle)
                {
                    MarblesToDelete.Add(marble);
                }
            }
        }


        if (MarblesToDelete.Count != 0)
        {
            foreach (Marble marble in MarblesToDelete)
            {
                MarblesList.Remove(marble);
                Destroy(marble.gameObject);
            }
            MarblesToDelete.Clear();
        }
    }
}
