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
    
}
