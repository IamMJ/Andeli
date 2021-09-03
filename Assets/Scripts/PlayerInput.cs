using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using Pathfinding;

public class PlayerInput : WordMakerMovement
{
    //init
    [SerializeField] GameObject moveArrowPrefab = null;
    DebugHelper dh;
    Animator anim;
    SpeedKeeper sk;
    GraphUpdateScene gus;

    //state
    Vector2 truePosition = Vector2.zero;
    [SerializeField] Vector2 rawDesMove = new Vector2(1,0);
    bool isMobile = false;
    bool isSnapped = false;

    Touch currentTouch;
    Vector2 previousTouchPosition;
    bool isValidStartPosition = false;
    GameObject moveArrow;

    Vector2 possibleMove;

    protected override void Start()
    {
        base.Start();
        sk = GetComponent<SpeedKeeper>();
        anim = GetComponent<Animator>();
        //Camera.main.GetComponentInChildren<CinemachineVirtualCamera>().Follow = gameObject.transform;

        dh = FindObjectOfType<DebugHelper>();
        isMobile = Application.isMobilePlatform;
        dh.DisplayDebugLog($"isMobile: {isMobile}");
        previousTouchPosition = Vector2.zero;
        gus = GetComponent<GraphUpdateScene>();
    }

    // Update is called once per frame
    void Update()
    {
        DetectIfSnapped();
        HandleTouchInput();
        if (isMobile)
        {
            //HandleTouchInput();
        }
        else
        {
            HandleKeyboardInput();
        }

        validDesMove = CardinalizeDesiredMovement(rawDesMove);

        UpdateAnimation();
    }


    #region Touch Input
    private void HandleTouchInput()
    { 
        if (Input.touchCount == 0)
        {
            return;
        }
        if (Input.touchCount > 0)
        {
            currentTouch = Input.GetTouch(0);
            if (currentTouch.phase == TouchPhase.Began && !GridHelper.CheckIsTouchingWordSection(currentTouch.position))
            {
                isValidStartPosition = true;
                possibleMove = Vector2.zero;
                previousTouchPosition = currentTouch.position;
                ClearMoveArrows();
            }
            //reset everything

            if (currentTouch.phase == TouchPhase.Moved && isValidStartPosition == true)
            {
                possibleMove = (currentTouch.position - previousTouchPosition);

                if (possibleMove.magnitude > UIParameters.MinTouchSensitivity)
                {
                    rawDesMove = CardinalizeDesiredMovement(possibleMove);
                    DisplayPlannedMoveArrows();
                }
            }

            if (currentTouch.phase == TouchPhase.Ended)
            {
                isValidStartPosition = false;
            }
        }
    }

    #endregion

    #region Computer Input
    private void HandleKeyboardInput()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            gc.UnpauseGame();
        }
        if (Input.GetKeyDown(KeyCode.W))
        {
            ClearMoveArrows();
            rawDesMove = new Vector2(0, 1);

            //truePosition += ApplyGraceToDesiredMovement(rawDesMove, validDesMove, transform.position, 0.5f);
            //Debug.Log($"Now at {truePosition}");
            truePosition = SnapToAxis(truePosition, true);
            DisplayPlannedMoveArrows();
            //gc.PauseGame();
            return;
        }
        if (Input.GetKeyDown(KeyCode.S))
        {
            ClearMoveArrows();
            rawDesMove = new Vector2(0, -1);
            //truePosition += ApplyGraceToDesiredMovement(rawDesMove, validDesMove, transform.position, 0.5f);
            //Debug.Log($"Now at {truePosition}");
            truePosition = SnapToAxis(truePosition, true);
            DisplayPlannedMoveArrows();
            //gc.PauseGame();
            return;
        }
        if (Input.GetKeyDown(KeyCode.A))
        {
            ClearMoveArrows();
            rawDesMove = new Vector2(-1, 0);
            //truePosition += ApplyGraceToDesiredMovement(rawDesMove, validDesMove, transform.position, 0.5f);
            //Debug.Log($"Now at {truePosition}");
            truePosition = SnapToAxis(truePosition, false);
            DisplayPlannedMoveArrows();
            //gc.PauseGame();
            return;
        }
        if (Input.GetKeyDown(KeyCode.D))
        {
            ClearMoveArrows();
            rawDesMove = new Vector2(1, 0);
            //truePosition += ApplyGraceToDesiredMovement(rawDesMove, validDesMove, transform.position, 0.5f);
            //Debug.Log($"Now at {truePosition}");
            truePosition = SnapToAxis(truePosition, false);
            DisplayPlannedMoveArrows();
            //gc.PauseGame();
            return;
        }


    }

    #endregion

    private void DisplayPlannedMoveArrows()
    {              
        Vector2 arrowPos;
                
        if (GridHelper.CheckIfSnappedToGrid(transform.position))
        {
            arrowPos = (Vector2)transform.position + rawDesMove;
                    
        }
        else
        {
            Vector2 distRemaining = GetDistToNextGridSnap(validDesMove, transform.position);
            arrowPos = (Vector2)transform.position + rawDesMove + distRemaining;
            // the validDesMove/2 in above line should really be a more complex calculation to get distance from player to middle of next tile.


        }

        arrowPos = GridHelper.SnapToGrid(arrowPos, 1);
        moveArrow = Instantiate(moveArrowPrefab, arrowPos, Quaternion.identity) as GameObject;
        moveArrow.GetComponent<MoveArrow>().Direction = QuantifyMoveDirection(rawDesMove);          
        
    }



    private void ClearMoveArrows()
    {
        Destroy(moveArrow);
    }

    #region Handle Movement
    private void FixedUpdate()
    {
        UpdateTruePosition();
        SnapDepictedPositionToTruePositionViaGrid();
    }


    private void DetectIfSnapped()
    {
        bool snapStatus = GridHelper.CheckIfSnappedToGrid(transform.position);
        if (snapStatus && !isSnapped)
        {
            isSnapped = true;
            validDesMove = rawDesMove;
            HandleEntryExitIntoNewTilesForGridGraph();
            previousSnappedPosition = transform.position;

        }
        if (!snapStatus)
        {
            isSnapped = false;
        }

    }
    private void HandleEntryExitIntoNewTilesForGridGraph()
    {
        gus.Apply();
    }

    private void UpdateTruePosition()
    {
        truePosition += validDesMove * sk.CurrentSpeed * Time.deltaTime;
    }

    private void SnapDepictedPositionToTruePositionViaGrid()
    {
        Vector3 oldPosition = transform.position;
        transform.position = GridHelper.SnapToGrid(truePosition, 8);
        float moveAmount = (oldPosition - transform.position).magnitude;
        if (moveAmount > Mathf.Epsilon)
        {
            PushWordMakerMovement();
            DropBreadcrumb();
        }
    }


    #endregion
    private void UpdateAnimation()
    {
        anim.SetFloat("Horizontal", validDesMove.x);
        anim.SetFloat("Vertical", validDesMove.y);
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.layer == 13)
        {
            validDesMove *= -1f;
            rawDesMove *= -1f;
        }
    }

}
