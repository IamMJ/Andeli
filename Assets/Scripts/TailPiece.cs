using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Pathfinding;

public class TailPiece : MonoBehaviour, IFollowable
{
    //init
    Movement wmm;  //wmm is subscribed to react to movement.
    IFollowable leaderToFollow;
    [SerializeField] TextMeshPro tmp;
    GraphUpdateScene gus;

    Vector2 lastSnappedPos = Vector2.zero;



    //state
    [SerializeField] List<Vector2> breadcrumbs = new List<Vector2>();

    public void OnCreation(Movement parentWMM, IFollowable newLeaderToFollow, char letterToDisplay)
    {
        lastSnappedPos = transform.position;
        wmm = parentWMM;
        //wmm.OnLeaderMoved += HandleTailPieceMovement;
        leaderToFollow = newLeaderToFollow;
        tmp.text = letterToDisplay.ToString();
        gus = GetComponent<GraphUpdateScene>();
    }

    private void HandleTailPieceMovement()
    {
        transform.position = leaderToFollow.GetOldestBreadcrumb();
        DropBreadcrumb();
        //gus.Apply();
        if (GridHelper.CheckIfSnappedToGrid(transform.position))
        {
            lastSnappedPos = transform.position;
        }

    }

    public void DetachTailPiece()
    {
        //wmm.OnLeaderMoved -= HandleTailPieceMovement;
        GetComponent<SpriteRenderer>().enabled = false;
        GetComponentInChildren<TextMeshPro>().enabled = false;
        GetComponent<Collider2D>().enabled = false;
        GameObject particles = GetComponentInChildren<ParticleSystem>()?.gameObject;
        Destroy(particles);
        transform.position = lastSnappedPos;
        gus.setWalkability = true;
        //gus.Apply();
        Destroy(gameObject, 4f);
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
