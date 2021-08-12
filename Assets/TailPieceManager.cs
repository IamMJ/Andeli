﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TailPieceManager : MonoBehaviour
{
    //init
    [SerializeField] GameObject tailPiecePrefab = null;
    List<TailPiece> tailPieces = new List<TailPiece>();
    WordMakerMovement wmm;

    void Start()
    {
        wmm = GetComponent<WordMakerMovement>();
    }

    public void AddNewTailPiece(char letterToDisplay)
    {
        Vector2 newPos = Vector2.zero;
        if (tailPieces.Count == 0)
        {
            newPos = wmm.GetOldestBreadcrumb();

            GameObject newTailPiece_GO = Instantiate(tailPiecePrefab, newPos, Quaternion.identity) as GameObject;

            TailPiece newTailPiece = newTailPiece_GO.GetComponent<TailPiece>();

            newTailPiece.OnCreation(wmm, wmm, letterToDisplay);

            tailPieces.Add(newTailPiece);
        }
        else
        {
            Debug.Log($"break 1");
            TailPiece lastTailPiece = tailPieces[tailPieces.Count - 1];
            Debug.Log($"break 2");
            newPos = lastTailPiece.GetOldestBreadcrumb();
            Debug.Log($"break 3");
            GameObject newTailPiece_GO = Instantiate(tailPiecePrefab, newPos, Quaternion.identity) as GameObject;
            Debug.Log($"break 4");
            TailPiece newTailPiece = newTailPiece_GO.GetComponent<TailPiece>();
            newTailPiece.OnCreation(wmm, lastTailPiece, letterToDisplay);

            tailPieces.Add(newTailPiece);
        }
    }

    public void DestroyEntireTail()
    {
        foreach (var tailPiece in tailPieces)
        {
            Destroy(tailPiece.gameObject);
        }

        tailPieces.Clear();
    }
}