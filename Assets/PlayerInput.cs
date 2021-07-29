using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInput : MonoBehaviour
{
    //init
    DebugHelper dh;
    WordBoxDriver wbd;
    Collider2D[] targetHits = new Collider2D[10];

    private enum TouchIntent { Move, EraseWord, FireWord };

    //param
    float longPressTime = 1.2f;
    float pressRadius = 100f;

    //state
    Vector2 truePosition = Vector2.zero;
    Vector2 desMove = Vector2.zero;
    Vector2 touchStartPos = Vector2.zero;
    Vector2 touchEndPos = Vector2.zero;
    float moveRate = 3f;
    Touch currentTouch;
    TouchIntent touchIntent;
    bool isMobile = false;
    float timeSpentLongPressing;
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
            DetermineTouchIntent();

            switch (touchIntent)
            {
                case TouchIntent.Move:
                    HandleMovementTouch();
                    break;

                case TouchIntent.FireWord:
                    HandleTargetingTouch();
                    break;

                case TouchIntent.EraseWord:
                    HandleEraseWordTouch();
                    break;
            }
        }

    }

    private void DetermineTouchIntent()
    {
        //if touching a target, then intent is FireWeapon
        if (Physics2D.OverlapCircleNonAlloc(currentTouch.position, pressRadius, targetHits, 1 << 10) > 0)
        {
            touchIntent = TouchIntent.FireWord;
            dh.DisplayDebugLog("targeting something");
            return;
        }

        //if touching in main area, but not on a target, then intent is Move
        if (GridHelper.CheckIfTouchingWordSection(currentTouch.position) == false)
        {
            touchIntent = TouchIntent.Move;
            dh.DisplayDebugLog("intend to move");
            return;
        }

        else
        {
            touchIntent = TouchIntent.EraseWord;
            dh.DisplayDebugLog("intend to erase word");
            return;
        }
    }
    
    private void HandleMovementTouch()
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
                desMove = (touchStartPos - touchEndPos).normalized;
                dh.DisplayDebugLog($"direction: {desMove}");
                isSelectingDirection = false;
                break;

        }
    }

    private void HandleEraseWordTouch()
    {
        if (currentTouch.phase == TouchPhase.Began)
        {
            timeSpentLongPressing = 0;
            isSelectingWord = true;
            return;
        }
        if (currentTouch.phase == TouchPhase.Stationary || currentTouch.phase == TouchPhase.Stationary)
        {
            timeSpentLongPressing += Time.deltaTime;
            wbd.FillWordEraseSlider(timeSpentLongPressing / longPressTime);
        }
        if (timeSpentLongPressing >= longPressTime)
        {
            //erase the word
            wbd.ClearOutWordBox();
            wbd.ClearWordEraseSlider();
            isSelectingWord = false;
            timeSpentLongPressing = 0;
        }
        if (currentTouch.phase == TouchPhase.Ended)
        {
            wbd.ClearWordEraseSlider();
            isSelectingWord = false;
            timeSpentLongPressing = 0;
        }

    }

    private void HandleTargetingTouch()
    {
        if (currentTouch.phase == TouchPhase.Began)
        {
            timeSpentLongPressing = 0;
            return;
        }
        if (currentTouch.phase == TouchPhase.Stationary || currentTouch.phase == TouchPhase.Moved)
        {
            timeSpentLongPressing += Time.deltaTime;
            wbd.FillWordFiringSlider(timeSpentLongPressing / longPressTime);
        }
        if (timeSpentLongPressing >= longPressTime)
        {
            //Fire the word!
            dh.DisplayDebugLog("Shooting off the word!");
            wbd.ClearOutWordBox();
            wbd.ClearWordFiringSlider();
            timeSpentLongPressing = 0;
        }
        if (currentTouch.phase == TouchPhase.Ended)
        {
            wbd.ClearWordFiringSlider();
            timeSpentLongPressing = 0;
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
            timeSpentLongPressing = 0;
            isSelectingWord = true;
        }

        if (Input.GetMouseButton(0) && isSelectingWord)
        {
            timeSpentLongPressing += Time.deltaTime;
            wbd.FillWordEraseSlider(timeSpentLongPressing / longPressTime);
            //Debug.Log($"time spent: {timeSpentTouchingWordSection}");
        }

        if (timeSpentLongPressing >= longPressTime)
        {
            Debug.Log("Fire off the word");
            //activate the word!
            wbd.ClearOutWordBox();
            wbd.ClearWordEraseSlider();
            timeSpentLongPressing = 0;
            isSelectingWord = false;
        }
        if (Input.GetMouseButtonUp(0))
        {
            timeSpentLongPressing = 0;
            //early release.
            wbd.ClearWordEraseSlider();
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
