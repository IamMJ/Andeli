using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class SpellingStrategy : MonoBehaviour
{
    protected WordBrain_NPC wb;
    protected MoveBrain_NPC mb;
    protected StrategyBrain_NPC sb;
    protected WordValidater wv;
    protected DebugHelper dh;
    protected LetterTileDropper ltd;
    public virtual void Start()
    {
        wb = GetComponent<WordBrain_NPC>();
        mb = GetComponent<MoveBrain_NPC>();
        sb = GetComponent<StrategyBrain_NPC>();
        wv = FindObjectOfType<WordValidater>();
        dh = FindObjectOfType<DebugHelper>();
        ltd = FindObjectOfType<LetterTileDropper>();
    }
    public abstract void EvaluateWordAfterGainingALetter();

    public abstract LetterTile FindBestLetterFromAllOnBoard();


    protected void EraseWordIfLowChanceOfFinishing(int threshold)
    {
        string currentWord = wb.GetCurrentWord();
        int count = wv.FindWordBandWithStubWord(currentWord).Range;
        if (count < threshold)
        {
            //erase word;
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
