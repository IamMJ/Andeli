using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "ArenaSetting")]

public class ArenaSetting : ScriptableObject
{

    [SerializeField] public Sprite settingIcon = null;
    [SerializeField] public string settingName = null;
    [SerializeField] public string settingDesc_0 = null;
    [SerializeField] public string settingDesc_1 = null;
    [SerializeField] public string settingDesc_2 = null;
    public ArenaSettingHolder.ArenaSettingOptions aso = ArenaSettingHolder.ArenaSettingOptions.Plain;

    // letter dropping effects
    public float letterLifetime = 20;
    public int lettersPerWave = 3;
    public float timeBetweenWaves = 7f;
    public string lettersToIgnore = "";
    public float percentageOfLettersAsMisty = 0;
    public int maxLettersOnBoard = 12;

    // power effects
    public int startingVictoryMeterBalance = 20;
    public int powerModifierForWordCount = 0;
        // Set this to 1 to grant bonus for playing lots of words
        // Set to -1 to penalize playing lots of words this arena.

    public bool shouldNotCountIfRepeatingWord = false;
                                    

    //energy effects
    public float energyRegenRateModifier = 1;
        // Set this to 1 for normal regen. 0.5 for half rate, and 2 for double rate.

    //ability effects 
    public TrueLetter.Ability abilityToAutoIgnite = TrueLetter.Ability.Normal;

    // word building effects
    public int maxWordLength = 7;

    public Sprite GetBriefIcon()
    {
        return settingIcon;
    }

    public string GetBriefText()
    {
        return settingName;
    }



}
