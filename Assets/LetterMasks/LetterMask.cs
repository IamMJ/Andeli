using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LetterMask : object
{
    public char letter = '*';

    //state
    public int PowerMod = 0;
    public float rarity = 0;
    public TrueLetter.Ability ability = TrueLetter.Ability.Normal;
    public int experience_Current = 12;
    public int experience_NextLevel = 345;

    //public char GetLetter()
    //{
    //    return letter;
    //}

    //public void SetLetter(char letter)
    //{
    //    this.letter = letter;
    //}

    //public float GetRarity()
    //{
    //    return rarity;
    //}

    //public string GetBlurb()
    //{
    //    return "";
    //}

    //public TrueLetter.Ability GetAbility()
    //{
    //    return ability;
    //}

    //public void SetAbility(TrueLetter.Ability newAbility)
    //{
    //    ability = newAbility;
    //}

    //public string GetAbilityDescription()
    //{
    //    return "this is the ability description";
    //}

    //public int GetPower()
    //{
    //    return 0;
    //    //return AssociatedTrueLetter.GetPower() + PowerMod;
    //}

    //public string GetExperienceString()
    //{
    //    return $"{experience_Current} / {experience_NextLevel}";
    //}
}
