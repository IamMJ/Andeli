using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "TrueLetter")]
public class TrueLetter : ScriptableObject
{
    //param
    [SerializeField] public char Letter;
    [SerializeField] int weight;
    [SerializeField] public int Power;

    public int ProbabilityTop;

    public int GetWeight()
    {
        return weight;
    }
    
}
