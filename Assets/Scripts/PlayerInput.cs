using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInput : MonoBehaviour
{
    //init
    DebugHelper dh;
    WordBuilder wbd;
    LetterCollector lc;
    [SerializeField] Collider2D targetHit;
    SpellMaker sm;

    //param
    public float LongPressTime { get; private set; } = 0.7f;

    //state
    Vector2 truePosition = Vector2.zero;
    Vector2 validDesMove = Vector2.zero;
    Vector2 rawDesMove = Vector2.zero;
    Vector2 touchStartPos = Vector2.zero;
    bool isValidStartPosition = false;
    Vector2 touchEndPos = Vector2.zero;
    float moveRate = 3f;
    Touch currentTouch;
    bool isMobile = false;
    public float timeSpentLongPressing { get; private set; }  

    void Start()
    {
        dh = FindObjectOfType<DebugHelper>();
        wbd = FindObjectOfType<WordBuilder>();
        sm = GetComponent<SpellMaker>();
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
        if (transform.position.x % 1 > 0 && transform.position.y % 1 > 0)
        {
            return;
        }
        else
        {
            validDesMove = rawDesMove;
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
        truePosition += validDesMove * moveRate * Time.deltaTime;
    }

    private void SnapDepictedPositionToTruePositionViaGrid()
    {
        Vector2 oldPosition = transform.position;
        transform.position = GridHelper.SnapToGrid(truePosition, 4);
        Vector2 dir = transform.position - (Vector3)oldPosition;
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
}
