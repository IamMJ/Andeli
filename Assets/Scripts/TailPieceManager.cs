using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TailPieceManager : MonoBehaviour
{
    //init
    [SerializeField] GameObject tailPiecePrefab = null;
    List<TailPiece> tailPieces = new List<TailPiece>();
    WordMakerMovement wmm;

    [SerializeField] GameObject letterFX_Shiny = null;

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
            TailPiece lastTailPiece = tailPieces[tailPieces.Count - 1];
            newPos = lastTailPiece.GetOldestBreadcrumb();
            GameObject newTailPiece_GO = Instantiate(tailPiecePrefab, newPos, Quaternion.identity) as GameObject;
            TailPiece newTailPiece = newTailPiece_GO.GetComponent<TailPiece>();
            newTailPiece.OnCreation(wmm, lastTailPiece, letterToDisplay);

            tailPieces.Add(newTailPiece);
        }
    }

    public void AddFXToSelectedTailPiece(TrueLetter.Ability letterAbility, int index)
    {
        switch (letterAbility)
        {
            case TrueLetter.Ability.Shiny:
                Instantiate(letterFX_Shiny, tailPieces[index].transform);
                return;
        }
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
