using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class PlayerInput : MonoBehaviour
{
    //init
    DebugHelper dh;
    Animator anim;
    SpeedKeeper sk;

    //UI param
    public float LongPressTime { get; private set; } = 0.7f;

    //game param
    float startingSpeed = 2f;
    float acceleration = 0.05f; //speed gain per second;

    //state
    Vector2 truePosition = Vector2.zero;
    [SerializeField] Vector2 validDesMove = Vector2.zero;
    [SerializeField] Vector2 rawDesMove = Vector2.zero;
    Vector2 touchStartPos = Vector2.zero;
    bool isValidStartPosition = false;
    Vector2 touchEndPos = Vector2.zero;
    Touch currentTouch;
    bool isMobile = false;
    public float timeSpentLongPressing { get; private set; }  

    void Start()
    {
        sk = GetComponent<SpeedKeeper>();
        anim = GetComponent<Animator>();
        //Camera.main.GetComponentInChildren<CinemachineVirtualCamera>().Follow = gameObject.transform;
        Screen.autorotateToLandscapeLeft = false;
        Screen.autorotateToLandscapeRight = false;
        Screen.autorotateToPortraitUpsideDown = false;

        dh = FindObjectOfType<DebugHelper>();
        isMobile = Application.isMobilePlatform;
        dh.DisplayDebugLog($"isMobile: {isMobile}");

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

            if (currentTouch.phase == TouchPhase.Began)
            {
                if (GridHelper.CheckIfTouchingWordSection(currentTouch.position) == false)
                {
                    touchStartPos = currentTouch.position;
                    isValidStartPosition = true;
                }
            }

            if (isValidStartPosition && currentTouch.phase == TouchPhase.Ended)
            {
                touchEndPos = currentTouch.position;
                rawDesMove = (touchStartPos - touchEndPos).normalized;
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
    private void CardinalizeDesiredMovement()
    {
        if (Mathf.Abs(validDesMove.x) > Mathf.Abs(validDesMove.y))
        {
            validDesMove.y = 0;
            if (validDesMove.x < 0)
            {
                validDesMove.x = -1;
            }
            else
            {
                validDesMove.x = 1;
            }
        }
        else
        {
            validDesMove.x = 0;
            if (validDesMove.y < 0)
            {
                validDesMove.y = -1;
            }
            else
            {
                validDesMove.y = 1;
            }
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
        Vector2 oldPosition = transform.position;
        transform.position = GridHelper.SnapToGrid(truePosition, 8);
        //Vector2 dir = transform.position - (Vector3)oldPosition;
        //if (dir.x > 0)
        //{
        //    dh.DisplayDebugLog("moved right");
        //}
        //if (dir.x < 0)
        //{
        //    dh.DisplayDebugLog("moved left");
        //}
        //if (dir.y < 0)
        //{
        //    dh.DisplayDebugLog("moved down");
        //}
        //if (dir.y > 0)
        //{
        //    dh.DisplayDebugLog("moved up");
        //}

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
