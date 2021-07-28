using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInput : MonoBehaviour
{
    //init
    DebugHelper dh;
    WordBoxDriver wbd;

    //param
    float longPressTime = 1.2f;

    //state
    Vector2 truePosition = Vector2.zero;
    Vector2 desMove = Vector2.zero;
    Vector2 touchStartPos = Vector2.zero;
    Vector2 touchEndPos = Vector2.zero;
    float moveRate = 3f;
    Touch currentTouch;
    bool isMobile = false;
    float timeSpentTouchingWordSection;
    bool isSelectingWord = false;
    bool isSelectingDirection = false;


    void Start()
    {
        dh = FindObjectOfType<DebugHelper>();
        wbd = FindObjectOfType<WordBoxDriver>();

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

            if (GridHelper.CheckIfTouchingWordSection(Input.mousePosition))
            {
                HandleMouseClick();
            }
        }
        CardinalizeDesiredMovement();
    }

    #region Touch Input
    private void HandleTouchInput()
    {
        if (Input.touchCount > 0)
        {
            currentTouch = Input.GetTouch(0);
            if (!isSelectingDirection && GridHelper.CheckIfTouchingWordSection(currentTouch.position) || isSelectingWord)
            {
                HandleWordSectionTouch();
            }
            else
            {
                FindTouchDirection();
            }
        }

    }

    private void HandleWordSectionTouch()
    {
        if (currentTouch.phase == TouchPhase.Began)
        {
            timeSpentTouchingWordSection = 0;
            isSelectingWord = true;
            return;
        }
        if (currentTouch.phase == TouchPhase.Stationary && isSelectingWord)
        {
            timeSpentTouchingWordSection += Time.deltaTime;
            wbd.FillWordSlider(timeSpentTouchingWordSection / longPressTime);
        }
        if (timeSpentTouchingWordSection >= longPressTime)
        {
            //activate the word!
            wbd.ClearOutWordBox();
            wbd.ClearWordSlider();
            isSelectingWord = false;
        }
        if (currentTouch.phase == TouchPhase.Ended)
        {
            wbd.ClearWordSlider();
            isSelectingWord = false;
        }
        
    }

    private void FindTouchDirection()
    {
        switch (currentTouch.phase)
        {
            case TouchPhase.Began:
                touchStartPos = currentTouch.position;
                dh.DisplayDebugLog($"touched here: {touchStartPos}");
                isSelectingDirection = true;
                break;

            case TouchPhase.Ended:
                touchEndPos = currentTouch.position;
                dh.DisplayDebugLog($"ended here: {touchEndPos}");
                desMove = (touchEndPos - touchStartPos).normalized;
                dh.DisplayDebugLog($"direction: {desMove}");
                isSelectingDirection = false;
                break;

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
        desMove.x = Input.GetAxisRaw("Horizontal");
        desMove.y = Input.GetAxisRaw("Vertical");

    }

    private void HandleMouseClick()
    {

        if (Input.GetMouseButtonDown(0))
        {
            timeSpentTouchingWordSection = 0;
            isSelectingWord = true;
        }

        if (Input.GetMouseButton(0) && isSelectingWord)
        {
            timeSpentTouchingWordSection += Time.deltaTime;
            wbd.FillWordSlider(timeSpentTouchingWordSection / longPressTime);
            //Debug.Log($"time spent: {timeSpentTouchingWordSection}");
        }

        if (timeSpentTouchingWordSection >= longPressTime)
        {
            Debug.Log("Fire off the word");
            //activate the word!
            wbd.ClearOutWordBox();
            wbd.ClearWordSlider();
            timeSpentTouchingWordSection = 0;
            isSelectingWord = false;
        }
        if (Input.GetMouseButtonUp(0))
        {
            timeSpentTouchingWordSection = 0;
            //early release.
            wbd.ClearWordSlider();
            isSelectingWord = false;
        }

    }
    #endregion

    private void CardinalizeDesiredMovement()
    {

        if (Mathf.Abs(desMove.x) > Mathf.Abs(desMove.y))
        {
            desMove.y = 0;
            if (desMove.x < 0)
            {
                desMove.x = -1;
            }
            else
            {
                desMove.x = 1;
            }
        }
        else
        {
            desMove.x = 0;
            if (desMove.y < 0)
            {
                desMove.y = -1;
            }
            else
            {
                desMove.y = 1;
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
        truePosition += desMove * moveRate * Time.deltaTime;
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
