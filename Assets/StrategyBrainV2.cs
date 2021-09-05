using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Pathfinding;

public class StrategyBrainV2 : MonoBehaviour
{
    /// <summary>
    /// The Strategy Brain is supposed to take in Target Letter Tiles from the Word Brain.
    /// It outputs a TacticalDestination to the Move Brain
    /// </summary>

    //init
    AILerp ail;
    Seeker seeker;
    MoveBrain_NPC mb;
    WordBrain_NPC wb;
    ArenaBuilder ab;

    public Vector2 strategicDestination;

    Path path;

    //param
    float closeEnough = 1.0f;
    float nextWaypointDistance = 1;

    //state
    bool hasValidPath = false;
    public NavMeshPathStatus status;
    private int currentWaypoint = 0;
    private bool reachedEndOfPath;
    Vector2 previousDirection = Vector2.zero;



    void Start()
    {
        ail = GetComponent<AILerp>();
        seeker = GetComponent<Seeker>();
        mb = GetComponent<MoveBrain_NPC>();
        wb = GetComponent<WordBrain_NPC>();
        wb.OnNewTargetLetterTile += SetNewTargetLetterTileAsStrategicDestination;
        ab = FindObjectOfType<ArenaBuilder>();


        strategicDestination = ab.CreatePassableRandomPointWithinArena();
        seeker.StartPath(transform.position, strategicDestination, HandleCompletedPath);
        
    }

    // Update is called once per frame
    void Update()
    {
        if (!wb.TargetLetterTile)
        {
            SetRandomStrategicDestination();
        }
        if (wb.TargetLetterTile)
        {
            SetTLLasStrategicDestination();
        }

        StartPathToStrategicDestination();
        PassTacticalDestinationToMoveBrain();
    }

    private void SetRandomStrategicDestination()
    {
        if ((transform.position - (Vector3)strategicDestination).magnitude < closeEnough)
        {
            strategicDestination = ab.CreatePassableRandomPointWithinArena();
        }
    }

    private void SetTLLasStrategicDestination()
    {
        if ((strategicDestination - (Vector2)wb.TargetLetterTile.transform.position).magnitude > Mathf.Epsilon)
        {
            strategicDestination = wb.TargetLetterTile.transform.position;
        }

    }

    private void StartPathToStrategicDestination()
    {
        if (seeker.IsDone() == true)
        {
            seeker.StartPath(transform.position, strategicDestination, HandleCompletedPath);
        }
    }

    private void PassTacticalDestinationToMoveBrain()
    {
        if (path == null) { return; }
        // Check in a loop if we are close enough to the current waypoint to switch to the next one.
        // We do this in a loop because many waypoints might be close to each other and we may reach
        // several of them in the same frame.
        reachedEndOfPath = false;
        // The distance to the next waypoint in the path
        float distanceToWaypoint;
        while (true)
        {
            // If you want maximum performance you can check the squared distance instead to get rid of a
            // square root calculation. But that is outside the scope of this tutorial.
            distanceToWaypoint = Vector3.Distance(transform.position, path.vectorPath[currentWaypoint]);
            if (distanceToWaypoint < nextWaypointDistance)
            {
                // Check if there is another waypoint or if we have reached the end of the path
                if (currentWaypoint + 1 < path.vectorPath.Count)
                {
                    currentWaypoint++;
                }
                else
                {
                    // Set a status variable to indicate that the agent has reached the end of the path.
                    // You can use this to trigger some special code if your game requires that.
                    reachedEndOfPath = true;
                    break;
                }
            }
            else
            {
                break;
            }
        }

        mb.TacticalDestination = path.vectorPath[currentWaypoint];

        Debug.DrawLine(transform.position, mb.TacticalDestination, Color.red);

    }
    private void HandleCompletedPath(Path p)
    {
        //Debug.Log($"path calculated. Error? {p.error}");
        path = p;
        currentWaypoint = 0;
    }

    private void SetNewTargetLetterTileAsStrategicDestination()
    {
        //strategicDestination = wb.TargetLetterTile.transform.position;
    }



    private void OnDestroy()
    {
        wb.OnNewTargetLetterTile -= SetNewTargetLetterTileAsStrategicDestination;
    }

    //public static void DebugDrawPath(Vector3[] corners)
    //{
    //    if (corners.Length < 2) { return; }
    //    int i = 0;
    //    for (; i < corners.Length - 1; i++)
    //    {
    //        Debug.DrawLine(corners[i], corners[i + 1], Color.blue);
    //    }
    //    Debug.DrawLine(corners[0], corners[1], Color.red);
    //}

}
