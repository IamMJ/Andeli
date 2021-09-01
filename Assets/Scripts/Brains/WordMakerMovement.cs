using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public abstract class WordMakerMovement : MonoBehaviour, IFollowable
{
    public Action OnWordMakerMoved;
    protected Vector3 trailingDir;
    public float moveSpeed = 1f;
    protected Vector2 validDesMove = Vector2.zero;

    [SerializeField] List<Vector2> breadcrumbs = new List<Vector2>();


    protected virtual void PushWordMakerMovement()
    {
        OnWordMakerMoved?.Invoke();
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
