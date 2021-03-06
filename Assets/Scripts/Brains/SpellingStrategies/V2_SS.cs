using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class V2_SS : SpellingStrategy
{
    GameController gc;

    //state
    float currentPatience;

    public override void Start()
    {
        base.Start();
        gc = FindObjectOfType<GameController>();
        currentPatience = ep.Patience;
    }

    public override void UpdateStrategy()
    {
        PossibleWordStrategies pwo = PossibleWordStrategies.KeepBuildingCurrentWord;
        string cw = wb.GetCurrentWord();
        int cp = wb.CurrentPower;

        
        if (cp >= ep.MinimumPoints && wv.CheckWordValidity(cw))
        {
            currentPatience = ep.Patience;
            if (wwz.CheckIfSufficientEnergyToCast())
            {
                pwo = PossibleWordStrategies.FireWordWhenAble;
                OnRecommendedStrategyChange?.Invoke(pwo);
                CurrentRecommendedStrategy = pwo;
            }
            return;
        }  
        
        if (currentPatience <= 1f && wv.CheckWordValidity(cw))
        {
            currentPatience = ep.Patience;
            if (wwz.CheckIfSufficientEnergyToCast())
            {
                pwo = PossibleWordStrategies.FireWordWhenAble;
                OnRecommendedStrategyChange?.Invoke(pwo);
                CurrentRecommendedStrategy = pwo;
            }
            return;
        }

        if (CurrentBestLTT)
        {
            currentPatience = ep.Patience;
            pwo = PossibleWordStrategies.KeepBuildingCurrentWord;
            OnRecommendedStrategyChange?.Invoke(pwo);
            CurrentRecommendedStrategy = pwo;

            return;
        }

        currentPatience -= 1f;
        Debug.Log($"losing patience, at {currentPatience}");

        if (currentPatience <= -ep.Patience)
        {
            currentPatience = ep.Patience;
            Debug.Log($", erasing {cw}.");
            pwo = PossibleWordStrategies.EraseWord;
            OnRecommendedStrategyChange?.Invoke(pwo);
            CurrentRecommendedStrategy = pwo;

            return;
        }

        if (currentPatience <= 0f)
        {
            ExecuteDeadendStrategy();
        }

            //pwo = PossibleWordStrategies.NoStrategyAvailable;
            //OnRecommendedStrategyChange?.Invoke(pwo);
            //CurrentRecommendedStrategy = pwo;
        

        //if (true)// cw.Length >= 1 && wv.CheckWordValidity(cw) == false && !CurrentBestLTT)
        //{
        //    if (ep.ShouldWordAlwaysBeValid)
        //    {
        //        Debug.Log($"dead end and must be valid, erasing {cw}.");
        //        pwo = PossibleWordStrategies.EraseWord;
        //        OnRecommendedStrategyChange?.Invoke(pwo);
        //        CurrentRecommendedStrategy = pwo;
        //        currentPatience = ep.Patience;
        //        return;
        //    }
        //    else
        //    {
        //        currentPatience--;
        //        Debug.Log("losing patience");
        //        if (currentPatience <= 0)
        //        {
        //            if (wv.CheckWordValidity(cw))
        //            {
        //                Debug.Log($"firing {cw} as subpar.");
        //                pwo = PossibleWordStrategies.FireWord;
        //                OnRecommendedStrategyChange?.Invoke(pwo);
        //                CurrentRecommendedStrategy = pwo;
        //                currentPatience = ep.Patience;
        //                return;
        //            }
        //            else
        //            {
        //                ExecuteDeadendStrategy();
        //            }
        //        }
        //    }
        //}


    }

    public override void HandleLetterPickup(LetterTile newLetter)
    {

        if (newLetter != CurrentBestLTT)
        {
            Debug.Log($"picked up a {newLetter.Letter}, trying to get a {CurrentBestLTT.Letter}");
            wb.ClearLastLetterInWord();
        }
        else
        {
            UpdateStrategy();
        }

    }

    private void ExecuteDeadendStrategy()
    {
        Debug.Log("deadend executed");
        PossibleWordStrategies pwo;
        switch (ep.deadEndSubstrategy)
        {
            case DeadEndSubstrategy.EraseAll:
                pwo = PossibleWordStrategies.EraseWord;
                OnRecommendedStrategyChange?.Invoke(pwo);
                CurrentRecommendedStrategy = pwo;
                currentPatience = ep.Patience;
                return;

            case DeadEndSubstrategy.TrimRecent:
                wb.ClearLastLetterInWord();
                return;

            case DeadEndSubstrategy.Anagram:
                Debug.Log("no anagram implementation yet");
                currentPatience = ep.Patience;
                return;


        }
    }


    protected override float GenerateValueForLetterTile(LetterTile evaluatedLT)
    {
        string possWord = wb.GetCurrentWord() + evaluatedLT.Letter;
        float possPower = wb.CurrentPower + evaluatedLT.Power_Enemy;

        float wordPowerFactor = Mathf.Clamp(ep.PointsWeight * possPower, 1f, 999f); // include a way to increase value based on Letter Masks


        float wordValidityFactor = ConvertWordValidityToValue(possWord); //* (possPower / ep.MinimumPoints);

        //float futureWordFactor = ep.FutureWordsWeight * ConvertFutureWordsToValue(possWord) * ((float)ep.MinimumPoints/possPower);

        float distFactor = ep.DistanceWeight * (evaluatedLT.transform.position - transform.position).magnitude;

        float value = ((wordPowerFactor * wordValidityFactor )- distFactor);

        if (gc.debug_ShowAILetterValues)
        {
            Debug.Log($" wvf is: {wordValidityFactor}, from a Poss power is {possPower}, MinP is{ep.MinimumPoints}, validity: {ConvertWordValidityToValue(possWord)}");

            Debug.Log($"had {wb.GetCurrentWord()}, evaluated {evaluatedLT.Letter} as worth {value} " +
                $"(wpf: {wordPowerFactor}, wvf: {wordValidityFactor}, fwf: <in wvf>, df: {distFactor})");

            evaluatedLT.AssignAIValueForDebug(value);
        }



        return value;
    }

    private float ConvertWordValidityToValue(string futureWord)
    {
        if (futureWord.Length < 2) { return 1f; }
        if (ep.ShouldWordAlwaysBeValid == true )
        {
            if (wv.CheckWordValidity(futureWord))
            {
                return 1f;
            }
            else
            {
                return 0f;
            }
        }
        else
        {
            if (wv.CheckWordValidity(futureWord))
            {
                return 1f;
            }
            else
            {
                float futureWordFactor = ConvertFutureWordsToValue(futureWord) * ep.FutureWordsWeight; //* ((float)ep.MinimumPoints / possPower);
                return futureWordFactor;
            }

        }

    }

    private float ConvertFutureWordsToValue(string futureWord)
    {
        if (futureWord.Length == 0)
        {
            return 1f;
        }
        else
        {
            int futureWordCount = wv.FindWordBandWithStubWord(futureWord).Range;
            return Mathf.Clamp01((float)futureWordCount / (float)ep.FutureWordsThreshold);
        }
        
    }
}
