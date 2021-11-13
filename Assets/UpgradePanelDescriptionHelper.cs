using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpgradePanelDescriptionHelper : MonoBehaviour
{
    //Normal, Shiny, Frozen, Lucky, Wispy, Mystic, Healthy, Heavy, Armored, Charged
    string normalAbilityDesc = "normal - ";
    string shinyAbilityDesc = "shiny";
    string frozenAbilityDesc = "frozen";
    string luckyAbilityDesc = "lucky";
    string wispyAbilityDesc = "wispy";
    string mysticAbilityDesc = "mystic";
    string healthyAbilityDesc = "healthy";
    string heavyAbilityDesc = "heavy";
    string armoredAbilityDesc = "armored";
    string chargedAbilityDesc = "charged";

    public string GetDescriptionForAbility(TrueLetter.Ability ability)
    {
        string output = "default";
        switch (ability)
        {
            case TrueLetter.Ability.Normal:
                output = normalAbilityDesc;
                break;

            case TrueLetter.Ability.Armored:
                output = armoredAbilityDesc;
                break;

            case TrueLetter.Ability.Charged:
                output = chargedAbilityDesc;
                break;

            case TrueLetter.Ability.Frozen:
                output = frozenAbilityDesc;
                break;

            case TrueLetter.Ability.Healthy:
                output = healthyAbilityDesc;
                break;

            case TrueLetter.Ability.Heavy:
                output = heavyAbilityDesc;
                break;

            case TrueLetter.Ability.Lucky:
                output = luckyAbilityDesc;
                break;

            case TrueLetter.Ability.Mystic:
                output = mysticAbilityDesc;
                break;

            case TrueLetter.Ability.Shiny:
                output = shinyAbilityDesc;
                break;

            case TrueLetter.Ability.Wispy:
                output = wispyAbilityDesc;
                break;
        }
        return output;
    }

}
