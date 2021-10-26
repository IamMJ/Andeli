using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmallWords_SS : SpellingStrategy
{

    //param
    int minWordOptions = 15;

    public override void UpdateStrategy()
    {
        FireOffCurrentWordIfPossible();
        EraseWordIfLowChanceOfFinishing(minWordOptions);

    }

    public override LetterTile FindBestLetterFromAllOnBoard()
    {
        List<LetterTile> letterTilesToEvaluate = ltd.FindAllReachableLetterTiles(transform.position, sk.CurrentSpeed);
        LetterTile currentBestOption = null;
        float currentBestValue = 0;
        foreach (var letterTile in letterTilesToEvaluate)
        {
            float hValue = CalculatePowerDistanceValue(letterTile) * CalculateFollowOnWordPotential(letterTile);
            //Debug.Log($"adding a {letterTile.Letter} to {currentWord} is worth {hValue} hValue. Follow-on Words: {possibleRefinedWordBand.Range}");
            if (hValue > currentBestValue)
            {
                currentBestOption = letterTile;
                currentBestValue = hValue;
                //Debug.Log($"best Option: {letterTile.Letter} at hValue: {currentBestValue}");
                //Debug.Log($"updated current word band to {currentWordBandToSearch.StartIndex}, {currentWordBandToSearch.Range}");
            }
            else
            {
                //Debug.Log("couldn't find a better option for TargetLetter");
            }
        }
        return currentBestOption;

    }

    private float CalculatePowerDistanceValue(LetterTile letterTile)
    {
        float dist = (letterTile.transform.position - transform.position).magnitude * 1.5f;
        float value = (letterTile.Power_Player / dist);

        return value;
    }
    private float CalculateFollowOnWordPotential(LetterTile letterTile)
    {
        string hypotheticalWord;
        if (wb.GetCurrentWord().Length == 0)
        {
            hypotheticalWord = letterTile.Letter.ToString();
        }
        else
        {
            hypotheticalWord = wb.GetCurrentWord() + letterTile.Letter;
        }

        int count = wv.FindWordBandWithStubWord(hypotheticalWord).Range;
        //Debug.Log($"{letterTile.Letter} hypothetical option: {hypotheticalWordBand.StartIndex}, {hypotheticalWordBand.Range}");

        return count / 10000f;
    }

    protected override float GenerateValueForLetterTile(LetterTile evaluatedLT)
    {
        throw new System.NotImplementedException();
    }
}
