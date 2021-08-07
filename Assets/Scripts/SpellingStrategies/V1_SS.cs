﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class V1_SS : SpellingStrategy
{
    //This SS focuses on 2nd order possible word counts exclusively. It ignores point value and distance.
    // It examines each reachable tile, adding that letters word count to the word count of each of its possible second
    // follow-on letters. The pairing with the highest sum will have its first letter chosen. Presumably, the second letter should still be there.

               
    //param
    int minPossibleWordsToConsider = 10;
    float timeBetweenPickups = 10f;

    //state
    float timeToShiftStrategy = Mathf.Infinity;



    private void Update()
    {
        if (Time.time > timeToShiftStrategy)
        {
            shouldFireOrEraseNow = true;
        }
    }

    public override void EvaluateWordAfterGainingALetter()
    {
        FireOffOREraseCurrentWordIfFutureWordsUnlikely(minPossibleWordsToConsider);
        timeToShiftStrategy = Time.time + timeBetweenPickups;
        shouldFireOrEraseNow = false;
    }

    public override LetterTile FindBestLetterFromAllOnBoard()
    {
        List<LetterTile> possibleLetters =  ltd.FindAllReachableLetterTiles(transform.position, mb.moveSpeed);

        LetterTile bestLetterTile = null;
        int bestWordCount = 0;
        foreach(var firstLetter in possibleLetters)
        {
            string hypotheticalWord_1Deep = wb.GetCurrentWord() + firstLetter.Letter.ToString();
            int possibleWordCount_1Deep = wv.FindWordBandWithStubWord(hypotheticalWord_1Deep).Range;

            if (possibleLetters.Count == 0)
            {
                return null;
            }
            if (possibleLetters.Count == 1)
            {
                if (possibleWordCount_1Deep > bestWordCount)
                {
                    bestLetterTile = firstLetter;
                    bestWordCount = possibleWordCount_1Deep;
                }
                continue; ;
            }
            else
            {
                int possibleWordCount_2Deep = 0;
                List<LetterTile> nextPossibleLetters = ltd.FindAllReachableLetterTiles(firstLetter.transform.position, mb.moveSpeed/2f);
                foreach (var nextLetter in nextPossibleLetters)
                {
                    if (nextLetter == firstLetter) { continue; }
                    string hypotheticalWord_2Deep = hypotheticalWord_1Deep + nextLetter.Letter.ToString();
                    possibleWordCount_2Deep = wv.FindWordBandWithStubWord(hypotheticalWord_2Deep).Range;
                }
                if (possibleWordCount_1Deep + possibleWordCount_2Deep > bestWordCount)
                {
                    bestLetterTile = firstLetter;
                    bestWordCount = possibleWordCount_1Deep + possibleWordCount_2Deep;
                }
            }
        }

        if (bestLetterTile)
        {
            shouldFireOrEraseNow = false;
        }
        else
        {
            EvaluateWordAfterGainingALetter();
        }

        Debug.Log($"Selected {bestLetterTile?.Letter}, with {bestWordCount} possible words");
        return bestLetterTile;
  
    }

}
