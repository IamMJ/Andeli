using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpgradePanelDescriptionHelper : MonoBehaviour
{
    //Normal, Shiny, Frozen, Lucky, Wispy, Mystic, Healthy, Heavy, Armored, Charged
    static string normalAbilityDesc = "normal - ";
    static string shinyAbilityDesc = "shiny";
    static string frozenAbilityDesc = "frozen";
    static string luckyAbilityDesc = "lucky";
    static string wispyAbilityDesc = "wispy";
    static string mysticAbilityDesc = "mystic";
    static string healthyAbilityDesc = "healthy";
    static string heavyAbilityDesc = "heavy";
    static string armoredAbilityDesc = "armored";
    static string chargedAbilityDesc = "charged";

    static int normalAbilityCost = 0;
    static int shinyAbilityCost = 2;
    static int frozenAbilityCost = 3;
    static int luckyAbilityCost = 4;
    static int wispyAbilityCost = 5;
    static int mysticAbilityCost = 6;
    static int healthyAbilityCost = 7;
    static int heavyAbilityCost = 8;
    static int armoredAbilityCost = 8;
    static int chargedAbilityCost = 10;

    static public string GetDescriptionForAbility(TrueLetter.Ability ability)
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

    static public int GetCostForAbility(TrueLetter.Ability ability)
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
