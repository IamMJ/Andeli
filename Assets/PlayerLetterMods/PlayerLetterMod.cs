using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "PlayerLetterMod")]
public class PlayerLetterMod : ScriptableObject
{
    [SerializeField] TrueLetter associatedTrueLetter = null;

    //state
    int powerMod;
    TrueLetter.Ability ability = TrueLetter.Ability.Normal;
    int experience_Current = 12;
    int experience_NextLevel = 345;

    public char GetLetter()
    {
        return associatedTrueLetter.GetLetter();
    }

    public float GetRarity()
    {
        return associatedTrueLetter.GetRarity();
    }

    public string GetBlurb()
    {
        return associatedTrueLetter.GetBlurb();
    }

    public TrueLetter.Ability GetAbility()
    {
        return ability;
    }

    public string GetAbilityDescription()
    {
        return "this is the ability description";
    }

    public int GetPower()
    {
        return associatedTrueLetter.GetPower() + powerMod;
    }

    public string GetExperienceString()
    {
        return $"{experience_Current} / {experience_NextLevel}";
    }

}
