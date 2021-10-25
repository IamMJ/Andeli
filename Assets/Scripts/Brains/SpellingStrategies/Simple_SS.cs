using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Simple_SS : SpellingStrategy
{
    public override void EvaluateWordAfterGainingALetter()
    {
        wb.TargetLetterTile = ltd.FindAllReachableLetterTiles(transform.position, sk.CurrentSpeed / 2)[0];
    }

    public override LetterTile FindBestLetterFromAllOnBoard()
    {
        LetterTile bestLT = ltd.FindAllReachableLetterTiles(transform.position, sk.CurrentSpeed/2)[0];
        return bestLT;
    }

}
