using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "ArenaSetting")]

public class ArenaSetting : ScriptableObject
{

    [SerializeField] public Sprite briefScreenIcon = null;
    [SerializeField] public string briefScreenText = null;
    public ArenaSettingHolder.ArenaSettingOptions aso = ArenaSettingHolder.ArenaSettingOptions.Plain;

    // letter dropping effects
    public float letterLifetime = 20;
    public int lettersPerWave = 3;
    public string lettersToNotDrop = "";
    public float percentageOfLettersAsMisty = 0;

    // power effects
    public int powerModifierForWordCountThisArena = 0;  
        // Set this to 1 to grant bonus for playing lots of words
        // Set to -1 to penalize playing lots of words this arena.
                                    

    //energy effects
    public float energyRegenRateModifier = 1;

    //ability effects 
    public TrueLetter.Ability abilityToAutoIgnite = TrueLetter.Ability.Normal;

    // word building effects
    public int maxWordLength = 7;

    public Sprite GetBriefIcon()
    {
        return briefScreenIcon;
    }

    public string GetBriefText()
    {
        return briefScreenText;
    }



}
