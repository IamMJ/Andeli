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

    int normalAbilityCost = 0;
    int shinyAbilityCost = 2;
    int frozenAbilityCost = 3;
    int luckyAbilityCost = 4;
    int wispyAbilityCost = 5;
    int mysticAbilityCost = 6;
    int healthyAbilityCost = 7;
    int heavyAbilityCost = 8;
    int armoredAbilityCost = 8;
    int chargedAbilityCost = 10;

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

    public int GetCostForAbility(TrueLetter.Ability ability)
    {
        int output = 0;
        switch (ability)
        {
            case TrueLetter.Ability.Normal:
                output = normalAbilityCost;
                break;

            case TrueLetter.Ability.Armored:
                output = armoredAbilityCost;
                break;

            case TrueLetter.Ability.Charged:
                output = chargedAbilityCost;
                break;

            case TrueLetter.Ability.Frozen:
                output = frozenAbilityCost;
                break;

            case TrueLetter.Ability.Healthy:
                output = healthyAbilityCost;
                break;

            case TrueLetter.Ability.Heavy:
                output = heavyAbilityCost;
                break;

            case TrueLetter.Ability.Lucky:
                output = luckyAbilityCost;
                break;

            case TrueLetter.Ability.Mystic:
                output = mysticAbilityCost;
                break;

            case TrueLetter.Ability.Shiny:
                output = shinyAbilityCost;
                break;

            case TrueLetter.Ability.Wispy:
                output = wispyAbilityCost;
                break;
        }
        return output;

    }

}
