using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "TrueLetter")]
public class PlayerLetterMod : ScriptableObject
{
    [SerializeField] char Letter;
    int powerMod;
    TrueLetter.Ability abilityMod = TrueLetter.Ability.Normal;
    public char GetLetter()
    {
        return Letter;
    }
}
