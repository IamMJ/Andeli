using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using Pathfinding;
[RequireComponent (typeof(WordMakerMovement), typeof(Seeker))]
public class PlayerInput : MonoBehaviour
{
    //init
    [SerializeField] GameObject moveArrowPrefab = null;
    [SerializeField] GameObject strategicDestinationPrefab = null;
    GameController gc;
    DebugHelper dh;
    GraphUpdateScene gus;
    WordMakerMovement wmm;
    Seeker seeker;
    Path currentPath;
    Touch currentTouch;
    GameObject moveArrow;
    Camera mc;
    GameObject currentStrategicDestinationIcon;
    int layerMask_Letter = 1 << 9;
    public LetterTile currentTargetedLetter;

    //param
    float nextWaypointDistance = 1;

    //state
    bool isMobile = false;
    bool isSnapped = false;
    GraphMask graphMask = 1 << 0;
    bool reachedEndOfPath;
    int currentWaypoint = 0;

    Vector2 strategicDestination = Vector2.zero;


    bool isValidStartPosition = false;


     void Start()
    {
        wmm = GetComponent<WordMakerMovement>();
        seeker = GetComponent<Seeker>();
        dh = FindObjectOfType<DebugHelper>();
        gc = FindObjectOfType<GameController>();
        isMobile = Application.isMobilePlatform;
        dh.DisplayDebugLog($"isMobile: {isMobile}");
        mc = Camera.main;

    }

    void Update()
    {
        HandleTouchInput();
        HandleMouseInput();
        PassTacticalDestinationToMoveBrain();

        //PauseWhenAtStrategicDestination();
    }


    private void HandleTouchInput()
    { 
        if (Input.touchCount == 1)
        {
            currentTouch = Input.GetTouch(0);
            if (currentTouch.phase == TouchPhase.Began && !GridHelper.CheckIsTouchingWordSection(currentTouch.position))
            {
                strategicDestination = GridHelper.SnapToGrid(currentTouch.position, 1);
                // Should I check that the destination is valid/reachable?
                MoveStrategicDestinationIcon();
                seeker.StartPath(transform.position, strategicDestination, HandleCompletedPath, graphMask);
            }
        }
    }

    private void HandleMouseInput()
    {
        if (Input.GetMouseButtonUp(0))
        {
            Vector2 mousePos = mc.ScreenToWorldPoint(Input.mousePosition);
            if (GridHelper.CheckIsTouchingWordSection(Input.mousePosition))
            {
                return;
            }
            strategicDestination = GridHelper.SnapToGrid(mousePos, 1);
            // Should I check that the destination is valid/reachable?
            MoveStrategicDestinationIcon();
            ReknitLetterAtStrategicDestination();
            Debug.DrawLine(transform.position, strategicDestination, Color.red, 1f);
            seeker.StartPath(transform.position, strategicDestination, HandleCompletedPath, graphMask);
        }
    }

    private void ReknitLetterAtStrategicDestination()
    {
        if (currentTargetedLetter)
        {
            currentTargetedLetter.UnknitSpecificGridGraph(0);
        }
        currentTargetedLetter = null;
        Collider2D coll = Physics2D.OverlapCircle(strategicDestination, 0.4f, layerMask_Letter);
        if (coll)
        {

            currentTargetedLetter = coll.GetComponent<LetterTile>();
            currentTargetedLetter.ReknitSpecificGridGraph(0);

        }
    }

    private void PauseWhenAtStrategicDestination()
    {
        if (((Vector2)transform.position - strategicDestination).magnitude <= Mathf.Epsilon)
        {
            gc.PauseGame();
        }
        else
        {
            gc.ResumeGameSpeed();
        }
    }

    #region Helpers
    private void MoveStrategicDestinationIcon()
    {
        if (currentStrategicDestinationIcon)
        {
            currentStrategicDestinationIcon.transform.position = strategicDestination;
        }
        else
        {
            currentStrategicDestinationIcon =
                Instantiate(strategicDestinationPrefab, strategicDestination, Quaternion.identity);
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
            currentPath = newPath;
            currentWaypoint = 0;
            //Show footsteps from current pos along path to destination.
        }

    }

    private void PassTacticalDestinationToMoveBrain()
    {
        if (currentPath == null) { return; }
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
            distanceToWaypoint = Vector3.Distance(transform.position, currentPath.vectorPath[currentWaypoint]);
            if (distanceToWaypoint < nextWaypointDistance)
            {
                // Check if there is another waypoint or if we have reached the end of the path
                if (currentWaypoint + 1 < currentPath.vectorPath.Count)
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

        wmm.TacticalDestination = currentPath.vectorPath[currentWaypoint];

        Debug.DrawLine(transform.position, wmm.TacticalDestination, Color.red);

    }

    //private void DisplayPlannedMoveArrows()
    //{              
    //    Vector2 arrowPos;
                
    //    if (GridHelper.CheckIfSnappedToGrid(transform.position))
    //    {
    //        arrowPos = (Vector2)transform.position + rawDesMove;                    
    //    }
    //    else
    //    {
    //        Vector2 distRemaining = GetDistToNextGridSnap(validDesMove, transform.position);
    //        arrowPos = (Vector2)transform.position + rawDesMove + distRemaining;
    //        // the validDesMove/2 in above line should really be a more complex calculation to get distance from player to middle of next tile.

    //    }

    //    arrowPos = GridHelper.SnapToGrid(arrowPos, 1);
    //    moveArrow = Instantiate(moveArrowPrefab, arrowPos, Quaternion.identity) as GameObject;
    //    moveArrow.GetComponent<MoveArrow>().Direction = QuantifyMoveDirection(rawDesMove);          
        
    //}

    private void ClearMoveArrows()
    {
        Destroy(moveArrow);
    }

    #endregion
}
