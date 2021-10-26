using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "StrategyValues")]
public class StrategyValues : ScriptableObject
{
    public int MinimumPoints = 2;
    public float PointsWeight = 1f;

    public int FutureWordsThreshold = 50;
    public float FutureWordsWeight = 0f;
    
    public bool ShouldWordAlwaysBeValid = true;

    public float DistanceWeight = 1;

    public float Patience = 10f;
}
