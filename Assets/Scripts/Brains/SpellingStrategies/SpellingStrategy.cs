using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class SpellingStrategy : MonoBehaviour
{
    protected WordBrain_NPC wb;
    protected MoveBrain_NPC mb;
    protected SpeedKeeper sk;
    protected StrategyBrain_NPC sb;
    protected WordValidater wv;
    protected DebugHelper dh;
    protected LetterTileDropper ltd;

    //state
    protected bool shouldFireOrEraseNow = false;
    public virtual void Start()
    {
        wb = GetComponent<WordBrain_NPC>();
        mb = GetComponent<MoveBrain_NPC>();
        sb = GetComponent<StrategyBrain_NPC>();
        wv = FindObjectOfType<WordValidater>();
        dh = FindObjectOfType<DebugHelper>();
        ltd = FindObjectOfType<LetterTileDropper>();
        sk = GetComponent<SpeedKeeper>();
    }
    public abstract void EvaluateWordAfterGainingALetter();

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
                wb.FireOffWord(); // ...so fire it off now.
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
            wb.FireOffWord();
        }
    }


}
