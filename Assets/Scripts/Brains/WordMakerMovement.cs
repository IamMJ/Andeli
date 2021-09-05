﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public abstract class WordMakerMovement : MonoBehaviour, IFollowable
{
    public Action OnLeaderMoved;

    public float moveSpeed = 1f;
    [SerializeField] List<Vector2> breadcrumbs = new List<Vector2>(8);
    [SerializeField] GameObject reknitterPrefab = null;
    protected GameController gc;
    TailPieceManager tpm;

    protected int layerMask_Impassable = 1 << 13;
    protected int layerMask_Passable = 1 << 14;
    protected int layerMask_Tile = 1 << 15;
    protected int layerMask_TileImpassable = 1 << 13 | 1 << 15;

    //state
    protected Vector2 rawDesMove = new Vector2(1, 0);
    protected Vector2 validDesMove = Vector2.zero;
    protected Vector2 previousMove = Vector2.zero;
    protected Vector2 truePosition = Vector2.zero;
    protected Vector3 previousSnappedPosition;


    protected virtual void Start()
    {
        gc = FindObjectOfType<GameController>();
        GameObject reknitterGO = Instantiate(reknitterPrefab);
        reknitterGO.GetComponent<Reknitter>().SetOwners(this, GetComponent<TailPieceManager>());
        tpm = GetComponent<TailPieceManager>();
    }

    protected virtual void PushWordMakerMovement()
    {
        OnLeaderMoved?.Invoke();
    }

    
    protected void CardinalizeDesiredMovement()
    {
        if (Mathf.Abs(validDesMove.x) > Mathf.Abs(validDesMove.y))
        {
            validDesMove.y = 0;
            if (validDesMove.x < 0)
            {
                validDesMove.x = -1;
                return;
            }
            else
            {
                validDesMove.x = 1;
                return;
            }
        }
        if (Mathf.Abs(validDesMove.x) <= Mathf.Abs(validDesMove.y))
        {
            validDesMove.x = 0;
            if (validDesMove.y < 0)
            {
                validDesMove.y = -1;
                return;
            }
            else
            {
                validDesMove.y = 1;
                return;
            }
        }
        else
        {
            //Debug.Log($"else statement on cardinalize movement. X/Y: {validDesMove.x}/{validDesMove.y}");

        }
        Debug.Log($"vdm: {validDesMove}");
    }

    public static Vector2 CardinalizeDesiredMovement(Vector2 inputDir)
    {
        Vector2 moveDir = Vector2.zero;
        if (Mathf.Abs(inputDir.x) >= Mathf.Abs(inputDir.y))
        {
            inputDir.y = 0;
            if (inputDir.x < 0)
            {
                moveDir.x = -1;
            }
            else
            {
                moveDir.x = 1;
            }
        }
        else
        {
            inputDir.x = 0;
            if (inputDir.y < 0)
            {
                moveDir.y = -1;
            }
            else
            {
                moveDir.y = 1;
            }
        }
        return moveDir;

    }

    protected bool CheckThatDesiredMoveIsntReversal(Vector2 desiredMove, Vector2 previousMove)
    {
        if ((desiredMove - previousMove).magnitude <= Mathf.Epsilon)
        {
            return false;
        }
        else return true;
    }

    protected Vector2 ApplyGraceToDesiredMovement(Vector2 desiredMove, Vector2 currentMove, Vector2 currentPosition, float grace)
    {
        if ((desiredMove - currentMove).magnitude <= Mathf.Epsilon) { return Vector3.zero; } // no grace if not changing direction
        Vector2 positionAdjustment;
        Vector2 remDist = GetDistFromPreviousGridSnap();
        if (remDist.magnitude <= grace)
        {
            positionAdjustment = desiredMove * Mathf.Abs(remDist.x + remDist.y) - remDist;
            Debug.Log($"Grace extended. was at {currentPosition.x}/{currentPosition.y} heading {currentMove}. remDist is {remDist}. adjusting by {positionAdjustment.x}/{positionAdjustment.y} to head {desiredMove}");
            return positionAdjustment;
        }
        else
        {
            Debug.Log("no grace");
            return Vector2.zero;
        }

    }

    protected Vector2 SnapToAxis(Vector2 inputPos, bool trueIfXAxis)
    {
        Vector2 output = inputPos;
        if (trueIfXAxis)
        {
            int invalidSteps = Mathf.RoundToInt((inputPos.x % 1) * 8);
            RemoveInvalidBreadcrumbs(invalidSteps);
            float amount = Mathf.RoundToInt(inputPos.x);
            output.x = amount;
        }
        else
        {
            int invalidSteps = Mathf.RoundToInt((inputPos.y % 1) * 8);
            RemoveInvalidBreadcrumbs(invalidSteps);
            float amount = Mathf.RoundToInt(inputPos.y);
            output.y = amount;
        }
        return output;
    }

    protected void GetAlternativeValidDesMoveIfReversing()
    {
        // compare ValidDesMove to previous Move. 
        //If perfectly opposite, then modify ValidDesMove either left or right randomly.
        int layerMask_ImpassableTile = layerMask_Impassable | layerMask_Tile;
        if ((validDesMove + previousMove).magnitude <= Mathf.Epsilon)
        {
            //Moves are inverts of each to equal zero.
            int rand = (UnityEngine.Random.Range(0, 2) * 2 - 1);
            Vector2 oldMove = validDesMove;
            validDesMove = new Vector2(validDesMove.y, validDesMove.x) * rand;
            RaycastHit2D hit = Physics2D.Raycast(transform.position, transform.position + (Vector3)validDesMove, 1.0f, layerMask_ImpassableTile);
            if (hit)
            {
                validDesMove = validDesMove * rand; // multiply by the same rand to reverse the "turn"
            }
            //Debug.Log($"reversed: old {oldMove} into new: {validDesMove}");
        }

    }

    protected void GetAlternativeValidDesMoveIfEnteringImpassable()
    {
        int layerMask_ImpassableTile = layerMask_Impassable | layerMask_Tile;
        RaycastHit2D hit = Physics2D.Raycast(transform.position, 
            transform.position + (Vector3)validDesMove, 1.0f, layerMask_ImpassableTile);
        if (hit)
        {
            int rand = (UnityEngine.Random.Range(0, 2) * 2 - 1);
            validDesMove = validDesMove * rand; // multiply by the same rand to reverse the "turn"
        }
    }
    protected Vector2 GetDistToNextGridSnap(Vector2 desiredMove, Vector2 currentPosition)
    {
        float value = 0;
        if (desiredMove.x > 0)
        {
            float remainder = currentPosition.x % 1;
            if (remainder > 0)
            {
                value = 1 - remainder;
            }
            if (remainder < 0)
            {
                value = -1 * remainder;
            }
            //Debug.Log($"X at {transform.position.x}, rem: {remainder}, dir: {validDesMove.x}. Returned: {value}");
            return new Vector2(value, 0);
        }
        if (desiredMove.x < 0)
        {
            float remainder = currentPosition.x % 1;
            if (remainder > 0)
            {
                value = -1 * remainder;
            }
            if (remainder < 0)
            {
                value = -1 - remainder;
            }
            //Debug.Log($"X at {transform.position.x}, rem: {remainder}, dir: {validDesMove.x}. Returned: {value}");
            return new Vector2(value, 0);
        }
        if (desiredMove.y > 0)
        {
            float remainder = currentPosition.y % 1;
            if (remainder > 0)
            {
                value = 1 - remainder;
            }
            if (remainder < 0)
            {
                value = -1 * remainder;
            }
            //Debug.Log($"Y at {transform.position.y}, rem: {remainder}, dir: {validDesMove.y}. Returned: {value}");
            return new Vector2(0, value);
        }
        if (desiredMove.y < 0)
        {
            float remainder = currentPosition.y % 1;
            if (remainder > 0)
            {
                value = -1 * remainder;
            }
            if (remainder < 0)
            {
                value = -1 - remainder;
            }
            //Debug.Log($"Y at {transform.position.y}, rem: {remainder}, dir: {validDesMove.y}. Returned: {value}");
            return new Vector2(0, value);
        }
        return Vector2.zero;

        // Pos, Dir:
        // .25, +1 = 0.75
        // -.25, +1 = 0.25
        // .25, -1 = -0.25
        // -.25, -1 = -0.75
    }

    protected Vector2 GetDistFromPreviousGridSnap()
    {
        Vector3 dist = (transform.position - previousSnappedPosition);
        return dist;
    }

    public static MoveArrow.MoveDirection QuantifyMoveDirection(Vector2 inputDir)
    {
        MoveArrow.MoveDirection direction;
        if (Mathf.Abs(inputDir.x) > Mathf.Abs(inputDir.y))
        {
            inputDir.y = 0;
            if (inputDir.x < 0)
            {
                direction = MoveArrow.MoveDirection.Left;
            }
            else
            {
                direction = MoveArrow.MoveDirection.Right;
            }
        }
        else
        {
            inputDir.x = 0;
            if (inputDir.y < 0)
            {
                direction = MoveArrow.MoveDirection.Down;
            }
            else
            {
                direction = MoveArrow.MoveDirection.Up;
            }
        }
        return direction;
    }
    public void DropBreadcrumb()
    {
        breadcrumbs.Add(GridHelper.SnapToGrid(transform.position, 8));
        if (breadcrumbs.Count > 8)
        {
            breadcrumbs.RemoveAt(0);
        }
    }

    public Vector2 GetOldestBreadcrumb()
    {
        return breadcrumbs[0];
    }

    public void RemoveInvalidBreadcrumbs(int count)
    {
        for (int i = 0; i < count; i++)
        {
            if (i >= breadcrumbs.Count)
            {
                return;
            }
            breadcrumbs.RemoveAt(i);
        }
    }

    protected void OnCollisionEnter2D(Collision2D collision)
    {

        if (collision.gameObject.layer == 13 || collision.gameObject.layer == 15)
        {
            truePosition = GridHelper.SnapToGrid(truePosition, 1);
            transform.position = truePosition;
            Vector2 turn1 = new Vector2(validDesMove.y, validDesMove.x) * (UnityEngine.Random.Range(0, 2) * 2 - 1);
            Vector2 turn2 = turn1 * -1;

            RaycastHit2D hit1 = Physics2D.Linecast(transform.position,
            transform.position + (Vector3)turn1, layerMask_TileImpassable);
            Debug.DrawLine(transform.position, transform.position + (Vector3)turn1, Color.blue, 1f);
            if (hit1)
            {
                RaycastHit2D hit2 = Physics2D.Linecast(transform.position,
                    transform.position + (Vector3)turn2, layerMask_TileImpassable);
                Debug.DrawLine(transform.position, transform.position + (Vector3)turn2, Color.cyan, 1f);
                if (hit2)
                {
                    Debug.Log($"dead end post-colliding {hit2.transform.name}");
                }
                else
                {
                    validDesMove = turn2;
                    rawDesMove = validDesMove;
                }
            }
            else
            {
                validDesMove = turn1;
                rawDesMove = validDesMove;
            }
        }
    }


}
