using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInput : MonoBehaviour
{
    //init
    DebugHelper dh;

    //state
    Vector2 truePosition = Vector2.zero;
    Vector2 desMove = Vector2.zero;
    Vector2 touchStartPos = Vector2.zero;
    Vector2 touchEndPos = Vector2.zero;
    float moveRate = 4f;
    Touch currentTouch;
    bool isMobile = false;

    void Start()
    {
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
        CardinalizeDesiredMovement();
    }
    
    private void HandleTouchInput()
    {
        if (Input.touchCount > 0)
        {
            FindTouchDirection();
        }

    }

    private void FindTouchDirection()
    {
        currentTouch = Input.GetTouch(0);
        switch (currentTouch.phase)
        {
            case TouchPhase.Began:
                touchStartPos = currentTouch.position;
                dh.DisplayDebugLog($"touched here: {touchStartPos}");
                break;

            case TouchPhase.Ended:
                touchEndPos = currentTouch.position;
                dh.DisplayDebugLog($"ended here: {touchEndPos}");
                desMove = (touchEndPos - touchStartPos).normalized;
                dh.DisplayDebugLog($"direction: {desMove}");

                break;

        }
    }

    private void HandleKeyboardInput()
    {
        if (Mathf.Abs(Input.GetAxisRaw("Horizontal")) + Mathf.Abs(Input.GetAxisRaw("Vertical")) <= Mathf.Epsilon)
        {
            return;
        }
        desMove.x = Input.GetAxisRaw("Horizontal");
        desMove.y = Input.GetAxisRaw("Vertical");

    }

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
        transform.position = GridHelper.SnapToGrid(truePosition);
        Vector2 dir = transform.position - (Vector3)oldPosition;
        if (dir.x > 0)
        {
            dh.DisplayDebugLog("moved right");
        }
        if (dir.x < 0)
        {
            dh.DisplayDebugLog("moved left");
        }
        if (dir.y < 0)
        {
            dh.DisplayDebugLog("moved down");
        }
        if (dir.y > 0)
        {
            dh.DisplayDebugLog("moved up");
        }

    }
    #endregion
}
