using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "StrategyValues")]
public class EnemyProfile : ScriptableObject
{
    public float MinimumPoints = 2;
    public float PointsWeight = 1f;

    public int FutureWordsThreshold = 50;
    public float FutureWordsWeight = 0f;
    
    public bool ShouldWordAlwaysBeValid = true;

    public SpellingStrategy.DeadEndSubstrategy deadEndSubstrategy = SpellingStrategy.DeadEndSubstrategy.TrimRecent;

    public float DistanceWeight = 1;

    public float Patience = 10;

    public float BaseSpeedMultiplier = 2;
    public float BaseEnergyRegenMultiplier = 0.5f;

    //public TrueLetter.Ability A_Ability;
    //public TrueLetter.Ability B_Ability;
    //public TrueLetter.Ability C_Ability;
    //public TrueLetter.Ability D_Ability;
    //public TrueLetter.Ability E_Ability;
    //public TrueLetter.Ability F_Ability;
    //public TrueLetter.Ability G_Ability;
    //public TrueLetter.Ability H_Ability;
    //public TrueLetter.Ability I_Ability;
    //public TrueLetter.Ability J_Ability;
    //public TrueLetter.Ability K_Ability;
    //public TrueLetter.Ability L_Ability;
    //public TrueLetter.Ability M_Ability;
    //public TrueLetter.Ability N_Ability;
    //public TrueLetter.Ability O_Ability;
    //public TrueLetter.Ability P_Ability;
    //public TrueLetter.Ability Q_Ability;
    //public TrueLetter.Ability R_Ability;
    //public TrueLetter.Ability S_Ability;
    //public TrueLetter.Ability T_Ability;
    //public TrueLetter.Ability U_Ability;
    //public TrueLetter.Ability V_Ability;
    //public TrueLetter.Ability W_Ability;
    //public TrueLetter.Ability X_Ability;
    //public TrueLetter.Ability Y_Ability;
    //public TrueLetter.Ability Z_Ability;
}
