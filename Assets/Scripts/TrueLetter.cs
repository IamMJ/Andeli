using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "TrueLetter")]
public class TrueLetter : ScriptableObject
{
    //param
    [SerializeField] char Letter;
    [SerializeField] int weight;
    [SerializeField] int Power;
    [SerializeField] Ability ability = Ability.Nothing;

    public enum Ability { Nothing, Shiny, Frozen, Fiery };



    public int ProbabilityTop;

    public int GetWeight()
    {
        return weight;
    }

    public char GetLetter()
    {
        return Letter;
    }

    public int GetPower()
    {
        return Power;
    }

    public Ability GetAbility()
    {
        return ability;
    }
    
}
