using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "StrategyValues")]
public class EnemyProfile : ScriptableObject
{
    public int MinimumPoints = 2;
    public float PointsWeight = 1f;

    public int FutureWordsThreshold = 50;
    public float FutureWordsWeight = 0f;
    
    public bool ShouldWordAlwaysBeValid = true;

    public SpellingStrategy.DeadEndSubstrategy deadEndSubstrategy = SpellingStrategy.DeadEndSubstrategy.TrimRecent;

    public float DistanceWeight = 1;

    public int Patience = 10;

    public float BaseSpeedMultiplier = 2;
    public float BaseEnergyRegenMultiplier = 0.5f;
}
