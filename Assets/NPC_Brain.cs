using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;
using System;
[RequireComponent(typeof(Movement))]

public class NPC_Brain : MonoBehaviour
{
    //inits
    Seeker seeker;
    Path currentPath;
    Movement movement;
    NPCDialogManager diaman;
    Animation anim;

    //fixed param    
    float closeEnough = 0.5f;
    float loiterTime_RandomFactor = 0.2f;
    float nextWaypointDistance = 1;
    GraphMask graphMask = 1 << 0;


    // changeable params
    [SerializeField] bool willHaltIfRequested = true;
    [SerializeField] bool isFlying = false;
    [SerializeField] float wanderRange = 4f;
    [SerializeField] float loiterTime_Average = 10f;
    [SerializeField] float pathLengthMax = 20f; //20 is a good human reference


    //state
    Vector2 baseLocation;
    public bool requestedToHalt = false;
    [SerializeField] Vector2 strategicDest;
    [SerializeField] bool isAtDestination = false;
    float timeToMoveOn;
    int currentWaypoint = 0;


    void Start()
    {
        anim = GetComponent<Animation>();
        diaman = GetComponent<NPCDialogManager>();
        movement = GetComponent<Movement>();
        seeker = GetComponent<Seeker>();
        baseLocation = transform.position;
        UpdateStrategicDestination();
    }

    // Update is called once per frame
    void Update()
    {
        if (isAtDestination)
        {
            if (Time.time >= timeToMoveOn)
            {
                requestedToHalt = false;
                UpdateStrategicDestination();
            }
        }
        else
        {
            if (currentPath != null) // if there is a current path, then should be flying.
            {
                SetTacticalDestinationToCurrentWaypoint();
            }
            if (isFlying && (transform.position - (Vector3)strategicDest).magnitude <= closeEnough)
            {
                isAtDestination = true;
            }
        }
    }

    private void UpdateStrategicDestination()
    {
        isAtDestination = false;
        strategicDest = GridHelper.CreateValidRandomPosition(baseLocation, wanderRange, isFlying);
        if (!isFlying)
        {
            seeker.StartPath(transform.position, strategicDest, HandleCompletedPath, graphMask);
        }
        else
        {
            movement.TacticalDestination = strategicDest;
        }
    }
    private void UpdateStrategicDestination(Vector2 inputStrategicDest)
    {
        isAtDestination = false;
        if (!isFlying)
        {
            seeker.StartPath(transform.position, inputStrategicDest, HandleCompletedPath, graphMask);
        }
        else
        {
            movement.TacticalDestination = inputStrategicDest;
        }
    }


    private void HandleCompletedPath(Path newPath)
    {
        if (newPath.error)
        {
            Debug.Log($"Error: {newPath.errorLog}");
        }
        else
        {

            if (CheckIfPathLengthIsShortEnough(pathLengthMax))
            {
                currentPath = newPath;
                currentWaypoint = 0;
            }
            else
            {
                UpdateStrategicDestination();
            }


        }
    }

    private bool CheckIfPathLengthIsShortEnough(float pathLengthMax)
    {
        if (currentPath?.GetTotalLength() > pathLengthMax)
        {
            return false;
        }
        else
        {
            return true;
        }
    }


    private void SetTacticalDestinationToCurrentWaypoint()
    {
        isAtDestination = false;
        float distanceToWaypoint;
        while (true)
        {
            distanceToWaypoint = Vector3.Distance(transform.position, currentPath.vectorPath[currentWaypoint]);
            if (distanceToWaypoint < nextWaypointDistance)
            {
                if (currentWaypoint + 1 < currentPath.vectorPath.Count)
                {
                    currentWaypoint++;
                }
                else
                {
                    isAtDestination = true;
                    if (requestedToHalt)
                    {
                        timeToMoveOn = Time.time + loiterTime_Average * 3;
                    }
                    else
                    {
                        timeToMoveOn = Time.time + (loiterTime_Average * UnityEngine.Random.Range(1 - loiterTime_RandomFactor, 1 + loiterTime_RandomFactor));
                    }
                    
                    break;
                }
            }
            else
            {
                break;
            }
        }

        movement.TacticalDestination = currentPath.vectorPath[currentWaypoint];
    }

    private void HaltInResponseToPlayer()
    {
        Debug.Log("requested to halt");
        diaman.ProvideReplyBarkToPlayer();
        requestedToHalt = true;
        strategicDest = GridHelper.SnapToGrid(transform.position, 1);
    }

    #region Public Methods
    public bool RequestNPCToHalt()
    {

        if (willHaltIfRequested)
        {            
            HaltInResponseToPlayer();
            return true;
        }
        else
        {
            return false;
        }
    }

    public void RequestNPCToMoveToSpecificDestination(Vector2 specificDest)
    {
        UpdateStrategicDestination(specificDest);
    }

    #endregion
}
