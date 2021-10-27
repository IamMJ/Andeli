using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Pathfinding;
[RequireComponent(typeof(SpellingStrategy))]

public class StrategyBrainV2 : MonoBehaviour
{
    /// <summary>
    /// The Strategy Brain is supposed to take in Target Letter Tiles from the Word Brain.
    /// It outputs a TacticalDestination to the Move Brain
    /// </summary>

    //init
    AILerp ail;
    Seeker seeker;
    Movement wmm;
    WordBuilder wb;
    WordWeaponizer wwz;
    ArenaBuilder ab;
    GraphMask graphMask;
    SpellingStrategy ss;


    public Vector2 strategicDestination;

    Path path;
    Vector3 currentPathDestination;

    //param
    float closeEnough = 1.0f;
    float nextWaypointDistance = 1;


    //state
    public LetterTile TargetLetterTile; //{ get; private set; }
    [SerializeField] LetterTile previousLTT;
    int GraphIndex = 1;  // If ever have multiple enemies, will need to have the Arena Builder change this for each.
    bool hasValidPath = false;
    NavMeshPathStatus status;
    private int currentWaypoint = 0;
    private bool reachedEndOfPath;
    Vector2 previousDirection = Vector2.zero;


    void Start()
    {
        ail = GetComponent<AILerp>();
        seeker = GetComponent<Seeker>();
        wmm = GetComponent<Movement>();
        wwz = GetComponent<WordWeaponizer>();
        wb = GetComponent<WordBuilder>();

        ss = GetComponent<SpellingStrategy>();
        ss.OnRecommendedStrategyChange += ReactToStrategyChange;
        ab = FindObjectOfType<ArenaBuilder>();
        graphMask = 1 << GraphIndex;

        strategicDestination = ab.CreatePassableRandomPointWithinArena();
        seeker?.StartPath(transform.position, strategicDestination, HandleCompletedPath, graphMask);        
    } 

    // Update is called once per frame
    void Update()
    {
        if (ss.CurrentBestLTT == null)
        {
            SetRandomStrategicDestination();
        }
        if (ss.CurrentBestLTT && wmm.GetValidDesMove().magnitude <= Mathf.Epsilon)
        {
            ReactToStrategyChange(SpellingStrategy.PossibleWordStrategies.KeepBuildingCurrentWord);
        }
        PassTacticalDestinationToMoveBrain();
    }

    private void SetRandomStrategicDestination()
    {
        if ((transform.position - (Vector3)strategicDestination).magnitude < closeEnough)
        {
            strategicDestination = ab.CreatePassableRandomPointWithinArena();
            StartPathToStrategicDestination(strategicDestination);
        }
        
    }

    private void SetTLLasStrategicDestination()
    {
        if ((strategicDestination - (Vector2)TargetLetterTile.transform.position).magnitude > Mathf.Epsilon)
        {
            strategicDestination = TargetLetterTile.transform.position;
            StartPathToStrategicDestination(strategicDestination);
        }

    }

    private void ReactToStrategyChange(SpellingStrategy.PossibleWordStrategies newStrategy)
    {
        switch (newStrategy)
        {
            //case SpellingStrategy.PossibleWordStrategies.NoStrategyAvailable:
            //    SetRandomStrategicDestination();
            //    ss.ResetRecommendedStrategy();
            //    return;

            case SpellingStrategy.PossibleWordStrategies.FireWordWhenAble:
                wwz.AttemptToFireWordAsNPC();
                ss.ResetRecommendedStrategy();
                return;

            case SpellingStrategy.PossibleWordStrategies.EraseWord:
                wb.ClearCurrentWord();
                ss.ResetRecommendedStrategy();
                return;

            case SpellingStrategy.PossibleWordStrategies.KeepBuildingCurrentWord:
                TargetLetterTile = ss.CurrentBestLTT;
                GridModifier.ReknitSpecificGridGraph(TargetLetterTile.transform, GraphIndex);

                if (previousLTT && TargetLetterTile != previousLTT)
                {
                    GridModifier.UnknitSpecificGridGraph(previousLTT.transform, GraphIndex);
                    previousLTT = TargetLetterTile;
                }

                SetTLLasStrategicDestination();
                return;

        }
    }

    private void StartPathToStrategicDestination(Vector3 destination)
    {
        if ((currentPathDestination - (Vector3)strategicDestination).magnitude < Mathf.Epsilon)
        {
            //Don't create a new path to the same point.
            return;
        }
        if (seeker?.IsDone() == true)
        {
            seeker?.StartPath(transform.position, destination, HandleCompletedPath, graphMask);
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

        wmm.TacticalDestination = path.vectorPath[currentWaypoint];

        Debug.DrawLine(transform.position, wmm.TacticalDestination, Color.red);

    }
    private void HandleCompletedPath(Path p)
    {
        //Debug.Log($"path calculated. Error? {p.error}");
        path = p;
        currentWaypoint = 0;
        currentPathDestination = p.vectorPath[p.vectorPath.Count - 1];
        Debug.DrawLine(transform.position, currentPathDestination, Color.blue, 3);
    }

    private void SetNewTargetLetterTileAsStrategicDestination()
    {
        //strategicDestination = wb.TargetLetterTile.transform.position;
    }



    private void OnDestroy()
    {
       
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
