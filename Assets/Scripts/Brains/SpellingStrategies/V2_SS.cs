using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class V2_SS : SpellingStrategy
{

    //state
    float currentPatience;

    public override void Start()
    {
        base.Start();
        currentPatience = ep.Patience;
    }

    public override void UpdateStrategy()
    {
        PossibleWordStrategies pwo = PossibleWordStrategies.KeepBuildingCurrentWord;
        string cw = wb.GetCurrentWord();
        int cp = wb.CurrentPower;

        if (cp >= ep.MinimumPoints && wv.CheckWordValidity(cw) && wwz.CheckIfSufficientEnergyToCast())
        {
            pwo = PossibleWordStrategies.FireWord;
            OnRecommendedStrategyChange?.Invoke(pwo);
            CurrentRecommendedStrategy = pwo;
            return;
        }

        if (CurrentBestLTT)
        {
            pwo = PossibleWordStrategies.KeepBuildingCurrentWord;
            OnRecommendedStrategyChange?.Invoke(pwo);
            CurrentRecommendedStrategy = pwo;
            return;
        }
        else
        {
            pwo = PossibleWordStrategies.NoStrategyAvailable;
            OnRecommendedStrategyChange?.Invoke(pwo);
            CurrentRecommendedStrategy = pwo;
        }

        if (cw.Length > 1 &&  wv.CheckWordValidity(cw) == false && !CurrentBestLTT)
        {
            if (ep.ShouldWordAlwaysBeValid)
            {
                Debug.Log($"dead end and must be valid, erasing {cw}.");
                pwo = PossibleWordStrategies.EraseWord;
                OnRecommendedStrategyChange?.Invoke(pwo);
                CurrentRecommendedStrategy = pwo;
                currentPatience = ep.Patience;
                return;
            }
            else
            {
                currentPatience--;
                Debug.Log("losing patience");
                if (currentPatience <= 0)
                {
                    if (wv.CheckWordValidity(cw))
                    {
                        Debug.Log($"firing {cw} as subpar.");
                        pwo = PossibleWordStrategies.FireWord;
                        OnRecommendedStrategyChange?.Invoke(pwo);
                        CurrentRecommendedStrategy = pwo;
                        currentPatience = ep.Patience;
                        return;
                    }
                    else
                    {
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
                                currentPatience = Mathf.Round((float)ep.Patience / 2f);
                                return;

                            case DeadEndSubstrategy.Anagram:
                                Debug.Log("no anagram implementation yet");
                                currentPatience = Mathf.Round((float)ep.Patience / 2f);
                                return;


                        }
                    }
                }
            }
        }

    }


    protected override float GenerateValueForLetterTile(LetterTile evaluatedLT)
    {
        string possWord = wb.GetCurrentWord() + evaluatedLT.Letter;
        float possPower = wb.CurrentPower + evaluatedLT.Power_Enemy;

        float wordPowerFactor = ep.PointsWeight * possPower; // include a way to increase value based on Letter Masks

        float wordValidityFactor = ConvertWordValidityToBoolint(possWord) * (possPower / ep.MinimumPoints);
        //Debug.Log($" wvf is: {wordValidityFactor}, from a Poss power is {possPower}, MinP is{sv.MinimumPoints}, validity: {ConvertFutureWordValidityToBoolint(possWord)}");

        //float futureWordFactor = ep.FutureWordsWeight * ConvertFutureWordsToValue(possWord) * ((float)ep.MinimumPoints/possPower);

        float distFactor = ep.DistanceWeight * (evaluatedLT.transform.position - transform.position).magnitude;

        float value = ((wordPowerFactor * wordValidityFactor )- distFactor);
        evaluatedLT.AssignAIValueForDebug(value);

        //Debug.Log($"had {wb.GetCurrentWord()}, evaluated {evaluatedLT.Letter} as worth {value} " +
        //    $"(wpf: {wordPowerFactor}, wvf: {wordValidityFactor}, fwf: <in wvf>, df: {distFactor})");

        return value;
    }

    private float ConvertWordValidityToBoolint(string futureWord)
    {
        if (ep.ShouldWordAlwaysBeValid == true && futureWord.Length > 0)
        {
            if (wv.CheckWordValidity(futureWord))
            {
                return 1;
            }
            else
            {
                return 0;
            }
        }

        if (ep.ShouldWordAlwaysBeValid == false)
        {
            if (futureWord.Length > 2 && wv.CheckWordValidity(futureWord))
            {
                return 1;
            }
            else
            {
                float futureWordFactor = ep.FutureWordsWeight * ConvertFutureWordsToValue(futureWord); //* ((float)ep.MinimumPoints / possPower);
                return futureWordFactor;
            }

        }

        return 0;
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
