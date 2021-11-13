using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "PlayerLetterMod")]
public class LetterMaskOld : ScriptableObject
{
    public TrueLetter AssociatedTrueLetter = null;

    //state
    public Sprite ShownSprite;
    public Color ShownColor;
    public int PowerMod = 0;
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
        return AssociatedTrueLetter.GetPower() + PowerMod;
    }

    public string GetExperienceString()
    {
        return $"{experience_Current} / {experience_NextLevel}";
    }

}
