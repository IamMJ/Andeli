using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "PlayerLetterMod")]
public class LetterMask : ScriptableObject
{
    public TrueLetter AssociatedTrueLetter = null;

    //state
    int powerMod;
    [SerializeField] TrueLetter.Ability ability = TrueLetter.Ability.Normal;
    int experience_Current = 12;
    int experience_NextLevel = 345;

    public char GetLetter()
    {
        return AssociatedTrueLetter.GetLetter();
    }

    public float GetRarity()
    {
        return AssociatedTrueLetter.GetRarity();
    }

    public string GetBlurb()
    {
        return AssociatedTrueLetter.GetBlurb();
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
        return AssociatedTrueLetter.GetPower() + powerMod;
    }

    public string GetExperienceString()
    {
        return $"{experience_Current} / {experience_NextLevel}";
    }

}
