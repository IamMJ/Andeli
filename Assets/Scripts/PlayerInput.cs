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
    GameController gc;
    public Vector2[] followOnMoves = new Vector2[3];
    GameObject[] moveArrows = new GameObject[3];

    //state
    Vector2 truePosition = Vector2.zero;
    [SerializeField] Vector2 rawDesMove = new Vector2(1,0);
    bool isMobile = false;
    bool isSnapped = false;

    Touch currentTouch;
    bool isValidStartPosition = false;
    Vector2 previousMove;
    Vector2 currentMove;
    Vector2 possibleMove;
    Vector2 previousTouchPosition = Vector2.zero;
    public int programmedMoves = 0;

    void Start()
    {
        sk = GetComponent<SpeedKeeper>();
        anim = GetComponent<Animator>();
        gc = FindObjectOfType<GameController>();
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
                currentMove = Vector2.zero;
                previousMove = Vector2.zero;
                programmedMoves = 0;
                followOnMoves[0] = validDesMove;
                followOnMoves[1] = validDesMove;
                followOnMoves[2] = validDesMove;
                previousTouchPosition = currentTouch.position;
                ClearMoveArrows();
                gc.SlowGame();
            }
            //reset everything

            if (currentTouch.phase == TouchPhase.Moved && isValidStartPosition == true)
            {
                possibleMove = (currentTouch.position - previousTouchPosition);

                if (possibleMove.magnitude > UIParameters.MinTouchSensitivity)
                {
                    possibleMove = CardinalizeDesiredMovement(possibleMove);
                    previousTouchPosition = currentTouch.position;
                    if (programmedMoves == 0)
                    {
                        previousMove = possibleMove;
                        followOnMoves[0] = possibleMove;
                        programmedMoves++;
                        DisplayPlannedMoveArrows();
                    }
                    else
                    {
                        if (!possibleMove.Equals(previousMove) && programmedMoves < followOnMoves.Length)
                        {
                            followOnMoves[programmedMoves] = possibleMove;
                            previousMove = possibleMove;
                            programmedMoves++;
                            DisplayPlannedMoveArrows();
                        }
                    }

                }
            }

            if (currentTouch.phase == TouchPhase.Ended)
            {
                isValidStartPosition = false;
                gc.UnpauseGame();
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
            DisplayPlannedMoveArrows();
            return;
        }
        if (Input.GetKeyDown(KeyCode.S))
        {
            followOnMoves[0].x = 0;
            followOnMoves[0].y = -1;
            programmedMoves = 1;
            DisplayPlannedMoveArrows();
            return;
        }
        if (Input.GetKeyDown(KeyCode.A))
        {
            followOnMoves[0].y = 0;
            followOnMoves[0].x = -1;
            programmedMoves = 1;
            DisplayPlannedMoveArrows();
            return;
        }
        if (Input.GetKeyDown(KeyCode.D))
        {
            followOnMoves[0].y = 0;
            followOnMoves[0].x = 1;
            programmedMoves = 1;
            DisplayPlannedMoveArrows();
            return;
        }


    }

    #endregion

    private void DisplayPlannedMoveArrows()
    {       
        if (programmedMoves > 0)
        {
            ClearMoveArrows();  //Might be redundant looping here...
            for (int i = 0; i < programmedMoves; i++)
            {
                Vector2 arrowPos;
                Vector2 earlierPos = Vector2.zero;
                if (i == 1)
                {
                    earlierPos = followOnMoves[0];
                }
                if (i == 2)
                {
                    earlierPos = followOnMoves[0] + followOnMoves[1];
                }
                if (GridHelper.CheckIfSnappedToGrid(transform.position))
                {
                    arrowPos = truePosition + followOnMoves[i] + earlierPos;
                }
                else
                {
                    arrowPos = truePosition + followOnMoves[i] + earlierPos + validDesMove / 2f;
                    // the validDesMove/2 in above line should really be a more complex calculation to get distance from player to middle of next tile.
                }

                arrowPos = GridHelper.SnapToGrid(arrowPos, 1);
                GameObject arrow = Instantiate(moveArrowPrefab, arrowPos, Quaternion.identity) as GameObject;
                arrow.GetComponent<MoveArrow>().Direction = QuantifyMoveDirection(followOnMoves[i]);
                moveArrows[i] = arrow;
            }
        }
    }

    private void ClearMoveArrows()
    {
        for (int k = 0; k < moveArrows.Length; k++)
        {
            Destroy(moveArrows[k]);
        } //Clear out all existing move arrows from previous inputs
    }

    #region Handle Movement
    private void FixedUpdate()
    {
        DetectIfSnapped();
        UpdateTruePosition();
        SnapDepictedPositionToTruePositionViaGrid();
    }
    private void DetectIfSnapped()
    {
        bool snapStatus = GridHelper.CheckIfSnappedToGrid(transform.position);
        if (snapStatus && !isSnapped)
        {
            isSnapped = true;
            FeedNextFollowOnMoveIntoRawDesMove();
            validDesMove = rawDesMove;
        }
        if (!snapStatus)
        {
            isSnapped = false;
        }
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
