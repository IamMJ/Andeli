using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Simple_SS : SpellingStrategy
{
    public override void UpdateStrategy()
    {
        sb.TargetLetterTile = ltd.FindAllReachableLetterTiles(transform.position, sk.CurrentSpeed / 2)[0];

    }

    public override LetterTile FindBestLetterFromAllOnBoard()
    {
        LetterTile bestLT = ltd.FindAllReachableLetterTiles(transform.position, sk.CurrentSpeed/2)[0];
        return bestLT;
    }

    protected override float GenerateValueForLetterTile(LetterTile evaluatedLT)
    {
        throw new System.NotImplementedException();
    }
}
