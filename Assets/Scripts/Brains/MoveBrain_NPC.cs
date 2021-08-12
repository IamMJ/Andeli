using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveBrain_NPC : WordMakerMovement
{
    //init
    WordBrain_NPC wb;
    Animator anim;

    //param
    

    //state
    public Vector2 TacticalDestination;
    Vector2 truePosition = Vector2.one;
    Vector2 rawDesMove = Vector2.zero;


    private void Start()
    {
        wb = GetComponent<WordBrain_NPC>();
        truePosition = transform.position;
        anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        UpdateRawDesMove();
        ConvertRawDesMoveIntoValidDesMove();
        CardinalizeDesiredMovement();

        HandleAnimation();
    }



    private void UpdateRawDesMove()
    {
        rawDesMove = ((Vector3)TacticalDestination - transform.position);
    }

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

    private void HandleAnimation()
    {
        anim.SetFloat("Horiz", validDesMove.x);
        anim.SetFloat("Vert", validDesMove.y);
    }

    #region Handle Movement
    private void FixedUpdate()
    {
        UpdateTruePosition();
        SnapDepictedPositionToTruePositionViaGrid();
    }

    private void UpdateTruePosition()
    {
        truePosition += validDesMove * moveSpeed * Time.deltaTime;
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

    public void UpdateDesiredMoveDirection(Vector2 desiredMoveDirection)
    {
        rawDesMove = desiredMoveDirection;
    }

    public Vector2 GetValidDesMove()
    {
        return validDesMove;
    }
}
