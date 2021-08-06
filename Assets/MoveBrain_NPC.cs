using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveBrain_NPC : MonoBehaviour
{
    //init
    WordBrain_NPC wb;

    //param
    public float moveSpeed { get; private set; } = 3.0f;

    //state
    Vector2 destination;
    Vector2 truePosition = Vector2.one;
    Vector2 rawDesMove = Vector2.zero;
    Vector2 validDesMove = Vector2.zero;

    private void Start()
    {
        wb = GetComponent<WordBrain_NPC>();
    }

    // Update is called once per frame
    void Update()
    {
        UpdateRawDesMove();
        ConvertRawDesMoveIntoValidDesMove();
        CardinalizeDesiredMovement();
    }

    private void UpdateRawDesMove()
    {
        if (wb.TargetLetterTile)
        {
            destination = wb.TargetLetterTile.transform.position;
            rawDesMove = ((Vector3)destination - transform.position);
        }
        else
        {
            destination = Vector2.one * 2;
        }

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
        truePosition += validDesMove * moveSpeed * Time.deltaTime;
    }

    private void SnapDepictedPositionToTruePositionViaGrid()
    {
        Vector2 oldPosition = transform.position;
        transform.position = GridHelper.SnapToGrid(truePosition, 8);
    }
    #endregion

    public void UpdateDesiredMoveDirection(Vector2 desiredMoveDirection)
    {
        rawDesMove = desiredMoveDirection;
    }
}
