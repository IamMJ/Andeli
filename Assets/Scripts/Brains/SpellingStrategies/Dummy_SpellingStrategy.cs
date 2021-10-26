using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dummy_SpellingStrategy : SpellingStrategy
{
    public override void UpdateStrategy()
    {

    }

    public override LetterTile FindBestLetterFromAllOnBoard()
    {
        return null;
    }

    protected override float GenerateValueForLetterTile(LetterTile evaluatedLT)
    {
        throw new System.NotImplementedException();
    }
}
