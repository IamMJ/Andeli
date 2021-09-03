using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public abstract class WordMakerMovement : MonoBehaviour, IFollowable
{
    public Action OnLeaderMoved;
    protected Vector3 trailingDir;
    public float moveSpeed = 1f;
    protected Vector2 validDesMove = Vector2.zero;
    protected Vector2 previousMove = Vector2.zero;
    [SerializeField] GameObject reknitterPrefab = null;
    protected GameController gc;
    public Vector3 previousSnappedPosition;



    [SerializeField] List<Vector2> breadcrumbs = new List<Vector2>(8);


    protected virtual void Start()
    {
        gc = FindObjectOfType<GameController>();
        GameObject reknitterGO = Instantiate(reknitterPrefab);
        reknitterGO.GetComponent<Reknitter>().SetOwners(this, GetComponent<TailPieceManager>());
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
            float amount = Mathf.RoundToInt(inputPos.x);
            output.x = amount;
        }
        else
        {
            float amount = Mathf.RoundToInt(inputPos.y);
            output.y = amount;
        }
        return output;
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



}
