using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class PlayerInput : WordMakerMovement
{
    //init
    [SerializeField] GameObject moveArrowPrefab = null;
    DebugHelper dh;
    Animator anim;
    SpeedKeeper sk;
    public Vector2[] followOnMoves = new Vector2[3];
    GameObject[] moveArrows = new GameObject[3];

    //state
    Vector2 truePosition = Vector2.zero;
    [SerializeField] Vector2 rawDesMove = Vector2.zero;
    Vector2 touchStartPos = Vector2.zero;
    bool isValidStartPosition = false;
    Vector2 touchEndPos = Vector2.zero;
    Touch currentTouch;
    bool isMobile = false;
    Vector2 previousTouchPosition;
    public int programmedMoves = 0;
    int movesToDisplay = 0;


    void Start()
    {
        sk = GetComponent<SpeedKeeper>();
        anim = GetComponent<Animator>();
        //Camera.main.GetComponentInChildren<CinemachineVirtualCamera>().Follow = gameObject.transform;

        dh = FindObjectOfType<DebugHelper>();
        isMobile = Application.isMobilePlatform;
        dh.DisplayDebugLog($"isMobile: {isMobile}");
        previousTouchPosition = Vector2.zero;
        followOnMoves[0] = Vector2.zero;
        followOnMoves[1] = Vector2.zero;
        followOnMoves[2] = Vector2.zero;
    }

    // Update is called once per frame
    void Update()
    {
        if (isMobile)
        {
            HandleTouchInput();
        }
        else
        {
            HandleKeyboardInput();
        }
        validDesMove = CardinalizeDesiredMovement(rawDesMove);
        DisplayPlannedMoveArrows();
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
                programmedMoves = 0;
                followOnMoves[0] = validDesMove;
                followOnMoves[1] = validDesMove;
                followOnMoves[2] = validDesMove;
            }

            if (currentTouch.phase == TouchPhase.Moved && isValidStartPosition == true)
            {
                Vector2 possibleMove = (currentTouch.position - previousTouchPosition);
                previousTouchPosition = currentTouch.position;
                possibleMove = CardinalizeDesiredMovement(possibleMove);

                if (possibleMove.magnitude > UIParameters.MinTouchSensitivity)
                {
                    if (programmedMoves == 0)
                    {
                        followOnMoves[0] = possibleMove;
                        programmedMoves++;
                        movesToDisplay++;
                    }
                    else
                    {
                        if (!possibleMove.Equals(followOnMoves[programmedMoves - 1]) && programmedMoves < followOnMoves.Length)
                        {
                            followOnMoves[programmedMoves] = possibleMove;
                            programmedMoves++;
                            movesToDisplay++;
                        }
                    }
                    //dh.DisplayDebugLog($"moved {rawDesMove} with {rawDesMove.magnitude} greater than {UIParameters.MinTouchSensitivity}");
                }
                else
                {
                    //dh.DisplayDebugLog($"no move since {rawDesMove.magnitude} less than {UIParameters.MinTouchSensitivity}");
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
        //if (Mathf.Abs(Input.GetAxisRaw("Horizontal")) + Mathf.Abs(Input.GetAxisRaw("Vertical")) <= Mathf.Epsilon)
        //{
        //    movesToDisplay = 0;
        //    return;
        //}
        //else
        //{
        //    followOnMoves[0].x = Input.GetAxisRaw("Horizontal");
        //    followOnMoves[0].y = Input.GetAxisRaw("Vertical");
        //    followOnMoves[0] = CardinalizeDesiredMovement(followOnMoves[0]);
        //    programmedMoves = 1;
        //    movesToDisplay = 1;
        //}

        if (Input.GetKeyDown(KeyCode.W))
        {
            followOnMoves[0].x = 0;
            followOnMoves[0].y = 1;
            programmedMoves = 1;
            movesToDisplay = 1;
            return;
        }
        if (Input.GetKeyDown(KeyCode.S))
        {
            followOnMoves[0].x = 0;
            followOnMoves[0].y = -1;
            programmedMoves = 1;
            movesToDisplay = 1;
            return;
        }
        if (Input.GetKeyDown(KeyCode.A))
        {
            followOnMoves[0].y = 0;
            followOnMoves[0].x = -1;
            programmedMoves = 1;
            movesToDisplay = 1;
            return;
        }
        if (Input.GetKeyDown(KeyCode.D))
        {
            followOnMoves[0].y = 0;
            followOnMoves[0].x = 1;
            programmedMoves = 1;
            movesToDisplay = 1;
            return;
        }


    }

    #endregion

    private void DisplayPlannedMoveArrows()
    {
        
        if (movesToDisplay > 0)
        {
            for (int k = 0; k < moveArrows.Length; k++)
            {
                Destroy(moveArrows[k]);
            }
            for (int i = 0; i < movesToDisplay; i++)
            {
                Vector2 arrowPos;
                if (GridHelper.CheckIfSnappedToGrid(transform.position))
                {
                    arrowPos = truePosition + followOnMoves[i];

                }
                else
                {
                    arrowPos = truePosition + followOnMoves[i] + validDesMove/2f;
                }

                arrowPos = GridHelper.SnapToGrid(arrowPos, 1);
                GameObject arrow = Instantiate(moveArrowPrefab, arrowPos, Quaternion.identity) as GameObject;
                arrow.GetComponent<MoveArrow>().Direction = QuantifyMoveDirection(followOnMoves[i]);
                moveArrows[i] = arrow;
                movesToDisplay--;
                //Debug.Log($"Arrow placed at: {arrowPos}, snapped pos: {snappedPosition}, follow: {followOnMoves[0]}, curr: {validDesMove}");
            }
        }

    }

    #region Handle Movement
    private void FixedUpdate()
    {
        DetectIfSnapped();
        UpdateTruePosition();
        SnapDepictedPositionToTruePositionViaGrid();

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
    private void DetectIfSnapped()
    {
        if (GridHelper.CheckIfSnappedToGrid(transform.position))
        {
            //Debug.Log($"thinks it is snapped at {transform.position}");
            FeedNextFollowOnMoveIntoRawDesMove();

            validDesMove = rawDesMove;
        }
    }
    private void FeedNextFollowOnMoveIntoRawDesMove()
    {
        if (programmedMoves > 0)
        {
            rawDesMove = followOnMoves[0];
            followOnMoves[0] = followOnMoves[1];
            followOnMoves[1] = followOnMoves[2];
            programmedMoves--;
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
