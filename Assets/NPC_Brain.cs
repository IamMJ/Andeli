using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;
using System;

public class NPC_Brain : MonoBehaviour
{
    //inits
    Seeker seeker;
    Path currentPath;
    Movement movement;

    //fixed param    
    float closeEnough = 0.5f;
    float loiterTime_RandomFactor = 0.2f;
    float nextWaypointDistance = 1;
    GraphMask graphMask = 1 << 0;


    // changeable params
    [SerializeField] bool isFlying = false;
    [SerializeField] float moveSpeed = 4f;
    [SerializeField] float wanderRange = 4f;
    [SerializeField] float loiterTime_Average = 10f;
    [SerializeField] float pathLengthMax = 20f; //20 is a good human reference

    //state
    Vector2 baseLocation;
    Vector2 movement_Flying;
    [SerializeField] Vector2 strategicDest;
    [SerializeField] bool isAtDestination = false;
    float timeToMoveOn;
    int currentWaypoint = 0;

    void Start()
    {
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
                isAtDestination = false;
                UpdateStrategicDestination();
            }
        }
        else
        {
            if (currentPath != null)
            {
                SetTacticalDestinationToCurrentWaypoint();
            }
        }
    }

    private void UpdateStrategicDestination()
    {        
        strategicDest = GridHelper.CreateValidRandomPosition(baseLocation, wanderRange, isFlying);
        if (!isFlying)
        {
            seeker.StartPath(transform.position, strategicDest, HandleCompletedPath, graphMask);
        }
    }

    private void FixedUpdate()
    {
        ExecuteMovementVector();
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

    private void ExecuteMovementVector()
    {
        if (isFlying)
        {
            movement_Flying.x = Mathf.MoveTowards(movement_Flying.x, strategicDest.x, moveSpeed * Time.deltaTime);
            movement_Flying.y = Mathf.MoveTowards(movement_Flying.y, strategicDest.y, moveSpeed * Time.deltaTime);
            transform.position = movement_Flying;
        }
        else
        {
            // Non-flying movement is handled by the Movement Component, by providing it a Tactical Destination
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
                    timeToMoveOn = Time.time + (loiterTime_Average * UnityEngine.Random.Range(1 - loiterTime_RandomFactor, 1 + loiterTime_RandomFactor));
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
}
