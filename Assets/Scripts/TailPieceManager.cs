using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TailPieceManager : MonoBehaviour
{
    //init
    [SerializeField] GameObject tailPiecePrefab = null;
    public List<TailPiece> tailPieces = new List<TailPiece>();
    Movement wmm;

    void Start()
    {
        wmm = GetComponent<Movement>();
    }

    public void AddNewTailPiece(char letterToDisplay)
    {
        Vector2 newPos = Vector2.zero;
        if (tailPieces.Count == 0)
        {
            newPos = wmm.GetOldestBreadcrumb();
            GameObject newTailPiece_GO = Instantiate(tailPiecePrefab, newPos, Quaternion.identity) as GameObject;
            newTailPiece_GO.layer = 0;
            TailPiece newTailPiece = newTailPiece_GO.GetComponent<TailPiece>();

            newTailPiece.OnCreation(wmm, wmm, letterToDisplay);

            tailPieces.Add(newTailPiece);
        }
        else
        {
            TailPiece lastTailPiece = tailPieces[tailPieces.Count - 1];
            newPos = lastTailPiece.GetOldestBreadcrumb();
            GameObject newTailPiece_GO = Instantiate(tailPiecePrefab, newPos, Quaternion.identity) as GameObject;
            TailPiece newTailPiece = newTailPiece_GO.GetComponent<TailPiece>();
            newTailPiece.OnCreation(wmm, lastTailPiece, letterToDisplay);

            tailPieces.Add(newTailPiece);
        }
    }

    public TailPiece GetTailPieceAt(int index)
    {
        return tailPieces[index];
    }

    public void DestroyEntireTail()
    {
        int i = 0;
        foreach (var tailPiece in tailPieces)
        {
            tailPiece.DetachTailPiece();
            i++;
        }
        tailPieces.Clear();
    }

    public Vector2 ReturnLastBreadcrumbOfLastTailPiece()
    {
        if (tailPieces.Count > 0)
        {
            int lastPiece = tailPieces.Count - 1;
            TailPiece lastTail = tailPieces[lastPiece];
            return lastTail.GetOldestBreadcrumb();
        }
        else
        {
            return wmm.GetOldestBreadcrumb();
        }

    }

    private void OnDestroy()
    {
        DestroyEntireTail();
    }
}
