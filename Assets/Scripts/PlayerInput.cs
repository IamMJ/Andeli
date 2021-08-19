using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class PlayerInput : WordMakerMovement
{
    //init
    DebugHelper dh;
    Animator anim;
    SpeedKeeper sk;

    //state
    Vector2 truePosition = Vector2.zero;
    [SerializeField] Vector2 rawDesMove = Vector2.zero;
    Vector2 touchStartPos = Vector2.zero;
    bool isValidStartPosition = false;
    Vector2 touchEndPos = Vector2.zero;
    Touch currentTouch;
    bool isMobile = false;
    Vector2 previousTouchPosition;


    void Start()
    {
        sk = GetComponent<SpeedKeeper>();
        anim = GetComponent<Animator>();
        //Camera.main.GetComponentInChildren<CinemachineVirtualCamera>().Follow = gameObject.transform;

        dh = FindObjectOfType<DebugHelper>();
        isMobile = Application.isMobilePlatform;
        dh.DisplayDebugLog($"isMobile: {isMobile}");
        previousTouchPosition = Vector2.zero;

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

        ConvertRawDesMoveIntoValidDesMove();
        CardinalizeDesiredMovement();
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
            }

            if (currentTouch.phase == TouchPhase.Moved && isValidStartPosition == true)
            {
                Vector2 possibleMove = (currentTouch.position - previousTouchPosition);
                previousTouchPosition = currentTouch.position;
                if (possibleMove.magnitude > UIParameters.MinTouchSensitivity)
                {
                    rawDesMove = possibleMove;
                    dh.DisplayDebugLog($"moved {rawDesMove} with {rawDesMove.magnitude} greater than {UIParameters.MinTouchSensitivity}");
                }
                else
                {
                    dh.DisplayDebugLog($"no move since {rawDesMove.magnitude} less than {UIParameters.MinTouchSensitivity}");
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
        if (Mathf.Abs(Input.GetAxisRaw("Horizontal")) + Mathf.Abs(Input.GetAxisRaw("Vertical")) <= Mathf.Epsilon)
        {
            return;
        }
        rawDesMove.x = Input.GetAxisRaw("Horizontal");
        rawDesMove.y = Input.GetAxisRaw("Vertical");

    }

    #endregion
    
    private void ConvertRawDesMoveIntoValidDesMove()
    {
        if (Mathf.Abs(transform.position.x % 1) > 0 || Mathf.Abs(transform.position.y % 1) > 0)
        {

        }
        else
        {
            validDesMove = rawDesMove;
            rawDesMove = validDesMove;
        }
    }

    #region Handle Movement
    private void FixedUpdate()
    {
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
