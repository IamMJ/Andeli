using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using Pathfinding;
[RequireComponent (typeof(Movement), typeof(Seeker))]
public class PlayerInput : MonoBehaviour
{
    //init
    [SerializeField] GameObject moveArrowPrefab = null;
    [SerializeField] GameObject strategicDestinationPrefab = null;
    GameController gc;
    DebugHelper dh;
    GraphUpdateScene gus;
    Movement movement;
    Seeker seeker;
    ConversationPanelDriver cpd;
    Path currentPath;
    Touch currentTouch;
    List<GameObject> moveArrows = new List<GameObject>();
    Camera mc;
    GameObject currentStrategicDestinationIcon;
    int layerMask_Letter = 1 << 9;
    int layerMask_NPC = 1 << 17;
    public GameObject currentTargetGO;

    //param
    float nextWaypointDistance = 1;

    //state
    bool isMobile = false;
    bool isSnapped = false;
    GraphMask graphMask = 1 << 0;
    bool reachedEndOfPath;
    int currentWaypoint = 0;

    Vector2 strategicDestination;


    bool isValidStartPosition = false;


     void Start()
    {
        movement = GetComponent<Movement>();
        seeker = GetComponent<Seeker>();
        dh = FindObjectOfType<DebugHelper>();
        gc = FindObjectOfType<GameController>();
        cpd = FindObjectOfType<ConversationPanelDriver>();
        isMobile = Application.isMobilePlatform;
        //dh.DisplayDebugLog($"isMobile: {isMobile}");
        mc = Camera.main;
        strategicDestination = transform.position;
    }

    void Update()
    {
        if (!gc.isPaused && !cpd.isDisplayed)
        {
            HandleTouchInput();
            HandleMouseInput();
        }


        PassTacticalDestinationToMoveBrain();

        //PauseWhenAtStrategicDestination();
    }


    private void HandleTouchInput()
    { 
        if (Input.touchCount == 1)
        {

            currentTouch = Input.GetTouch(0);
            if (currentTouch.phase == TouchPhase.Began &&
                !GridHelper.CheckIsTouchingWordSection(currentTouch.position, gc.isInArena, gc.isInTutorialMode))
            {
                strategicDestination = GridHelper.SnapToGrid(currentTouch.position, 1);
                // Should I check that the destination is valid/reachable?
                MoveStrategicDestinationIcon();
                ReknitLetterAtStrategicDestination();
                CheckStrategicDestinationForDialogPossibility();
                seeker.StartPath(transform.position, strategicDestination, HandleCompletedPath, graphMask);
            }
        }
    }

    private void HandleMouseInput()
    {
        if (gc.isPaused) { return; }
        if (Input.GetMouseButtonUp(0))
        {
            Vector2 mousePos = mc.ScreenToWorldPoint(Input.mousePosition);
            if (GridHelper.CheckIsTouchingWordSection(Input.mousePosition, gc.isInArena, gc.isInTutorialMode))
            {
                return;
            }
            strategicDestination = GridHelper.SnapToGrid(mousePos, 1);
            // Should I check that the destination is valid/reachable?
            MoveStrategicDestinationIcon();
            ReknitLetterAtStrategicDestination();
            CheckStrategicDestinationForDialogPossibility();
            Debug.DrawLine(transform.position, strategicDestination, Color.red, 1f);
            seeker.StartPath(transform.position, strategicDestination, HandleCompletedPath, graphMask);
        }
    }

    private void CheckStrategicDestinationForDialogPossibility()
    {
        Collider2D coll = Physics2D.OverlapCircle(strategicDestination, 0.75f, layerMask_NPC);
        //Debug.DrawLine(strategicDestination, strategicDestination + new Vector2(1.5f, 1.5f), Color.green, 2);
        NPC_Brain npcBrain;
        if (coll && coll.TryGetComponent(out npcBrain))
        {
            npcBrain.RequestNPCToHalt();
        }
    }

    private void ReknitLetterAtStrategicDestination()
    {
        if (currentTargetGO != null && currentTargetGO.GetComponent<LetterTile>().IsInactivated == false)
        {
            GridModifier.UnknitSpecificGridGraph(currentTargetGO.transform, 0);
        }
        currentTargetGO = null;
        Collider2D coll = Physics2D.OverlapCircle(strategicDestination, 0.4f, layerMask_Letter);
        if (coll)
        {
            currentTargetGO = coll.gameObject;
            GridModifier.ReknitSpecificGridGraph(currentTargetGO.transform, 0);
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
            gc.ResumeGameSpeed(false);
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
            DepictNewPathWithFootsteps();
        }

    }

    private void DepictNewPathWithFootsteps()
    {
        ClearMoveArrows();
        List<Vector3> vecPath = currentPath.vectorPath;
        Vector2 stepDir;
        for (int i = 0; i < vecPath.Count-1; i++)
        {
            if (i == 0)
            {
                stepDir = vecPath[i] - transform.position;
                GameObject stepArrow = Instantiate(moveArrowPrefab, vecPath[i], Quaternion.identity);
                stepArrow.GetComponent<MoveArrow>().Direction = Movement.QuantifyMoveDirection(stepDir);
                moveArrows.Add(stepArrow);
            }
            else
            {
                stepDir = vecPath[i] - vecPath[i-1];
                GameObject stepArrow = Instantiate(moveArrowPrefab, vecPath[i], Quaternion.identity);
                stepArrow.GetComponent<MoveArrow>().Direction = Movement.QuantifyMoveDirection(stepDir);
                moveArrows.Add(stepArrow);
            }
        }
    }

    private void PassTacticalDestinationToMoveBrain()
    {
        if (currentPath == null)
        {
            movement.TacticalDestination = transform.position;
            return;
        }
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

        movement.TacticalDestination = currentPath.vectorPath[currentWaypoint];

        Debug.DrawLine(transform.position, movement.TacticalDestination, Color.red);

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
        for (int i = moveArrows.Count-1; i >= 0; i--)
        {
            Destroy(moveArrows[i]);
        }
        moveArrows.Clear();
    }

    #endregion

    private void OnDestroy()
    {
        ClearMoveArrows();
        Destroy(currentStrategicDestinationIcon);
    }
}
