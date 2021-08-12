﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public abstract class WordMakerMovement : MonoBehaviour, IFollowable
{
    public Action OnWordMakerMoved;
    protected Vector3 trailingDir;
    public float moveSpeed = 3.0f;

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

    public static Vector2 CardinalizeDesiredMovement(Vector2 inputDir)
    {
        Vector2 moveDir = Vector2.zero;
        if (Mathf.Abs(inputDir.x) > Mathf.Abs(inputDir.y))
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

    public void DropBreadcrumb()
    {
        breadcrumbs.Add(transform.position);
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