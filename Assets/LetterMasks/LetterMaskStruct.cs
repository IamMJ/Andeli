using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LetterMaskStruct : MonoBehaviour
{
    public char letter = '*';

    //state
    Sprite ShownSprite;
    Color ShownColor;
    int PowerMod = 0;
    float rarity = 0;
    TrueLetter.Ability ability = TrueLetter.Ability.Normal;
    int experience_Current = 12;
    int experience_NextLevel = 345;

    private void Start()
    {
        GetRarity();
    }

    public char GetLetter()
    {
        return letter;
    }

    public float GetRarity()
    {

        return rarity;
    }

    public string GetBlurb()
    {
        return "";
    }

    public TrueLetter.Ability GetAbility()
    {
        return ability;
    }

    public void SetAbility(TrueLetter.Ability newAbility)
    {
        ability = newAbility;
    }

    public string GetAbilityDescription()
    {
        return "this is the ability description";
    }

    public int GetPower()
    {
        return 0;
        //return AssociatedTrueLetter.GetPower() + PowerMod;
    }

    public string GetExperienceString()
    {
        return $"{experience_Current} / {experience_NextLevel}";
    }
}
