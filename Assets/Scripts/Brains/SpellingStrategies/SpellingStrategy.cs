using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;
public abstract class SpellingStrategy : MonoBehaviour
{
    /// <summary>
    /// A spelling strategy should hold some preset values to determine min point goal,
    /// if it will accept a non-valid word enroute to a stronger valid word, and what it considers a 
    /// "good enough" future for a word stub, plus how it weights those different factors.
    /// 
    /// It should always strive to have a Current Best LTT available for the strategy brain. 
    /// </summary>
    protected WordBuilder wb;
    protected MoveBrain_NPC mb;
    protected WordWeaponizer wwz;
    protected SpeedKeeper sk;
    protected StrategyBrainV2 sb;
    protected WordValidater wv;
    protected DebugHelper dh;
    protected LetterTileDropper ltd;
    //protected List<(string, float)> evaluatedList = new List<(string, float)>();
    protected Dictionary<LetterTile, float> evaluatedLTs = new Dictionary<LetterTile, float>();
    [SerializeField] protected EnemyProfile ep;

    public enum PossibleWordStrategies {EraseWord, FireWordWhenAble, KeepBuildingCurrentWord, NoStrategyAvailable};
    public Action<PossibleWordStrategies> OnRecommendedStrategyChange;
    public enum DeadEndSubstrategy { TrimRecent, EraseAll, Anagram};

    //state
    public PossibleWordStrategies CurrentRecommendedStrategy = PossibleWordStrategies.NoStrategyAvailable;
    public LetterTile CurrentBestLTT = null;
    protected bool shouldFireOrEraseNow = false;

    public virtual void Start()
    {
        wb = GetComponent<WordBuilder>();
        wb.OnAddLetterToSword += UpdateStrategy;
        mb = GetComponent<MoveBrain_NPC>();
        sb = GetComponent<StrategyBrainV2>();
        sk = GetComponent<SpeedKeeper>();

        wwz = GetComponent<WordWeaponizer>();
        wv = FindObjectOfType<WordValidater>();
        dh = FindObjectOfType<DebugHelper>();
        ltd = FindObjectOfType<LetterTileDropper>();
        if (ltd)
        {
            ltd.OnLetterListModified += ModifyEvaluatedLetterDictionary;
        }

    }

    /// <summary>
    /// This should determine the AI's next course of action after gaining a letter. Strat Brain calls this
    /// after picking up a letter
    /// </summary>
    public abstract void UpdateStrategy();

    public virtual void ResetRecommendedStrategy()
    {
        CurrentRecommendedStrategy = PossibleWordStrategies.NoStrategyAvailable;
    }


    /// <summary>
    /// Generate a utility value for a given Letter Tile. This should vary between different strategies.
    /// </summary>
    /// <param name="evaluatedLT"></param>
    /// <returns></returns>
    protected abstract float GenerateValueForLetterTile(LetterTile evaluatedLT);
       
    /// <summary>
    /// This is called whenever the LTD's OnLetterChanged event is fired.
    /// </summary>
    /// <param name="modifiedLT"></param>
    /// <param name="wasAdded"></param>
    protected virtual void ModifyEvaluatedLetterDictionary(LetterTile modifiedLT, bool wasAdded)
    {
        evaluatedLTs.Clear();
        if (true) // TODO hook this into a debug thing later
        {
            List<LetterTile> allLTs = ltd.FindAllReachableLetterTiles(transform.position, 100);
            foreach (var el in allLTs)
            {
                el.AssignAIValueForDebug(0f);
            }
            foreach (var elem in ltd.FindAllReachableLetterTiles(transform.position, sk.CurrentSpeed))
            {
                evaluatedLTs.Add(elem, GenerateValueForLetterTile(elem));
            }
        }

        UpdateBestLTTOnDictionaryValueCalculated();
    }

    protected virtual void UpdateBestLTTOnDictionaryValueCalculated()
    {
        float currentBestPower = 0;
        LetterTile currentBestLT = null;
        foreach (var lt in evaluatedLTs)
        {
            if (lt.Value > currentBestPower)
            {
                currentBestPower = lt.Value;
                currentBestLT = lt.Key;
            }
        }
        CurrentBestLTT = currentBestLT;
        //Debug.Log("best letter: " + CurrentBestLTT.Letter);
        UpdateStrategy();

       
    }


    protected virtual void OnDestroy()
    {
        ltd.OnLetterListModified -= ModifyEvaluatedLetterDictionary;
    }

    public virtual void ImplementSpeedEnergySettingsFromEP()
    {
        GetComponent<WordWeaponizer>().ModifyEnergyRate(ep.BaseEnergyRegenMultiplier);
        GetComponent<SpeedKeeper>().ModifyTargetSpeed(ep.BaseSpeedMultiplier);
    }

    public virtual EnemyProfile GetEnemyProfile()
    {
        return ep;
    }


    #region Legacy methods

    public virtual LetterTile FindBestLetterFromAllOnBoard()
    {
        return null;
    }
    protected virtual void FireOffOREraseCurrentWordIfFutureWordsUnlikely(int thresholdForTooUnlikely)
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
                wb.ClearCurrentWord(); // ...so erase it now.  How did we get here though?
                return;
            }
        }
    }
    protected virtual void EraseWordIfLowChanceOfFinishing(int threshold)
    {
        string currentWord = wb.GetCurrentWord();
        int count = wv.FindWordBandWithStubWord(currentWord).Range;
        if (count < threshold)
        {

            dh.DisplayDebugLog($"erasing {currentWord} with only {count} options");
            wb.ClearCurrentWord();
        }
    }
    protected virtual void FireOffCurrentWordIfPossible()
    {
        string currentWord = wb.GetCurrentWord();
        if (wv.CheckWordValidity(currentWord))
        {
            dh.DisplayDebugLog("firing off " + currentWord);
            wwz.AttemptToFireWordAsNPC();
        }
    }
    #endregion

}
