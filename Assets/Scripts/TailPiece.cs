using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TailPiece : MonoBehaviour, IFollowable
{
    //init
    WordMakerMovement wmm;  //wmm is subscribed to react to movement.
    IFollowable leaderToFollow;
    [SerializeField] TextMeshPro tmp;



    //state
    [SerializeField] List<Vector2> breadcrumbs = new List<Vector2>();

    public void OnCreation(WordMakerMovement parentWMM, IFollowable newLeaderToFollow, char letterToDisplay)
    {
        wmm = parentWMM;
        wmm.OnWordMakerMoved += HandleTailPieceMovement;
        leaderToFollow = newLeaderToFollow;
        tmp.text = letterToDisplay.ToString();
    }

    private void HandleTailPieceMovement()
    {
        transform.position = leaderToFollow.GetOldestBreadcrumb();
        DropBreadcrumb();
    }

    private void OnDestroy()
    {
        wmm.OnWordMakerMoved -= HandleTailPieceMovement;
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
