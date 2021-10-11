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
    [SerializeField] bool willHaltIfRequested_Normally = true;
    [SerializeField] bool isFlying = false;
    [SerializeField] float wanderRange = 4f;
    [SerializeField] float loiterTime_Average = 10f;
    [SerializeField] float pathLengthMax = 20f; //20 is a good human reference


    //state
    Vector2 baseLocation;
    bool willHaltIfRequested_Currently;
    public bool requestedToHalt = false;
    [SerializeField] Vector2 strategicDest;
    [SerializeField] bool isAtDestination = false;
    [SerializeField] float timeToMoveOn;
    int currentWaypoint = 0;


    void Start()
    {
        willHaltIfRequested_Currently = willHaltIfRequested_Normally;
        anim = GetComponent<Animation>();
        diaman = GetComponent<NPCDialogManager>();
        movement = GetComponent<Movement>();
        seeker = GetComponent<Seeker>();
        baseLocation = transform.position;
        UpdateStrategicDestination(GridHelper.CreateValidRandomPosition(baseLocation, wanderRange, isFlying));
    }

    // Update is called once per frame
    void Update()
    {
        //if (!isAtDestination)
        //{
        //    float dist = ((Vector3)strategicDest - transform.position).magnitude;
        //    if (dist < 1f)
        //    {
        //        isAtDestination = true;
        //    }
        //}
        if (isAtDestination)
        {
            if (Time.time >= timeToMoveOn)
            {

                UpdateStrategicDestination(GridHelper.CreateValidRandomPosition(baseLocation, wanderRange, isFlying));
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

    //private void UpdateStrategicDestination()
    //{
    //    isAtDestination = false;
    //    strategicDest = GridHelper.CreateValidRandomPosition(baseLocation, wanderRange, isFlying);
    //    if (!isFlying)
    //    {
    //        seeker.StartPath(transform.position, strategicDest, HandleCompletedPath, graphMask);
    //    }
    //    else
    //    {
    //        movement.TacticalDestination = strategicDest;
    //    }
    //}

    private void UpdateStrategicDestination(Vector2 inputStrategicDest)
    {
        currentPath = null;
        strategicDest = inputStrategicDest;
        isAtDestination = false;
        if (!isFlying)
        {
            seeker.StartPath(transform.position, strategicDest, HandleCompletedPath, graphMask);
        }
        else
        {
            movement.TacticalDestination = strategicDest;
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
            //Debug.Log($"{newPath.duration} seconds to calculate path");
            if (CheckIfPathLengthIsShortEnough(pathLengthMax))
            {
                currentPath = newPath;
                currentWaypoint = 0;
            }
            else
            {
                UpdateStrategicDestination(GridHelper.CreateValidRandomPosition(baseLocation, wanderRange, isFlying));
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
                    requestedToHalt = false;
                    willHaltIfRequested_Currently = willHaltIfRequested_Normally;
                    
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
        diaman.ProvideReplyBarkToPlayer();
        requestedToHalt = true;
        UpdateStrategicDestination(GridHelper.SnapToGrid(transform.position, 1)); // Halt by setting current pos as dest

    }

    #region Public Methods
    public bool RequestNPCToHalt()
    {
        if (willHaltIfRequested_Currently)
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
        willHaltIfRequested_Currently = false;
        Debug.Log("requesting an NPC move to specific location");   
        UpdateStrategicDestination(specificDest);
        timeToMoveOn = Time.time;
    }

    #endregion
}
