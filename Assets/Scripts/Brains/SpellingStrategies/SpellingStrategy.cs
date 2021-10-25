﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class SpellingStrategy : MonoBehaviour
{
    protected WordBuilder_NPC wb;
    protected MoveBrain_NPC mb;
    protected WordWeaponizer wwz;
    protected SpeedKeeper sk;
    protected StrategyBrain_NPC sb;
    protected WordValidater wv;
    protected DebugHelper dh;
    protected LetterTileDropper ltd;

    //state
    protected bool shouldFireOrEraseNow = false;
    public virtual void Start()
    {
        wb = GetComponent<WordBuilder_NPC>();
        mb = GetComponent<MoveBrain_NPC>();
        sb = GetComponent<StrategyBrain_NPC>();
        sk = GetComponent<SpeedKeeper>();

        wwz = GetComponent<WordWeaponizer>();
        wv = FindObjectOfType<WordValidater>();
        dh = FindObjectOfType<DebugHelper>();
        ltd = FindObjectOfType<LetterTileDropper>();

    }
    /// <summary>
    /// This should determine the AI's next course of action after gaining a letter.
    /// </summary>
    public abstract void EvaluateWordAfterGainingALetter();

    /// <summary>
    /// This should pull all letter tiles currently on board, regardless of remaining lifetime, and then output a 'best' LTT
    /// </summary>
    /// <returns></returns>
    public abstract LetterTile FindBestLetterFromAllOnBoard();

    protected void FireOffOREraseCurrentWordIfFutureWordsUnlikely(int thresholdForTooUnlikely)
    {
        string currentWord = wb.GetCurrentWord();
        int currentWordOptions = wv.FindWordBandWithStubWord(currentWord).Range;
        if (wv.CheckWordValidity(currentWord)) //Current word is valid....
        {
            if (shouldFireOrEraseNow || currentWordOptions < thresholdForTooUnlikely) // ...but its future is too unpromising...
            {
                dh.DisplayDebugLog($"Firing {currentWord} with only {currentWordOptions} possible options ahead");
                wwz.AttemptToFireWordAsNPC();
                return;
            }
        }
        else // current words is invalid...
        {
            if (shouldFireOrEraseNow || currentWordOptions < thresholdForTooUnlikely) /// ...and its future is too unpromising...
            {
                dh.DisplayDebugLog($"Erasing {currentWord} with only {currentWordOptions} possible options");
                wb.EraseWord(); // ...so erase it now.  How did we get here though?
                return;
            }
        }
    }
    protected void EraseWordIfLowChanceOfFinishing(int threshold)
    {
        string currentWord = wb.GetCurrentWord();
        int count = wv.FindWordBandWithStubWord(currentWord).Range;
        if (count < threshold)
        {

            dh.DisplayDebugLog($"erasing {currentWord} with only {count} options");
            wb.EraseWord();
        }
    }
    protected void FireOffCurrentWordIfPossible()
    {
        string currentWord = wb.GetCurrentWord();
        if (wv.CheckWordValidity(currentWord))
        {
            dh.DisplayDebugLog("firing off " + currentWord);
            wwz.AttemptToFireWordAsNPC();
        }
    }


}
