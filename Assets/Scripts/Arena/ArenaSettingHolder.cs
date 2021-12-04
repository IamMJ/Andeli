using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArenaSettingHolder : MonoBehaviour
{
    [SerializeField] ArenaSetting arenaSetting_Default = null;
    [SerializeField] public ArenaSetting arenaSetting = null;
    LetterTileDropper ltd;
    WordMakerMemory playerMemory;
    WordMakerMemory enemyMemory;
    WordWeaponizer wwz_Player;
    WordWeaponizer wwz_Enemy;
    WordBuilder wbd_Player;
    WordBuilder wbd_Enemy;
    HealthManager hm;
    CombatPanel uid;
    TutorDialogManager tutorDM;
    

    public enum ArenaSettingOptions {Plain, Mists, Sandy, Blizzard, Jungle, Graveyard, Training }

    public void SetupArena(LetterTileDropper letterTileDropper, WordMakerMemory playerPlayMem, WordMakerMemory enemyPlayMem,
        WordWeaponizer wwzPlayer, WordWeaponizer wwzEnemy, WordBuilder wbPlayer, WordBuilder wbEnemy, HealthManager healthMan, CombatPanel uIDriver)
    {
        ltd = letterTileDropper;
        playerMemory = playerPlayMem;
        enemyMemory = enemyPlayMem;
        wwz_Player = wwzPlayer;
        wwz_Enemy = wwzEnemy;
        wbd_Player = wbPlayer;
        wbd_Enemy = wbEnemy;
        hm = healthMan;
        uid = uIDriver;
        //Modify various things affected in the arena by the arena setttings
        ImplementSelectedArenaSettings();

    }

    private void ImplementSelectedArenaSettings()
    {
        //Implement Plain first as the default
        ltd.SetupArenaParameters_Lifetime(arenaSetting_Default.letterLifetime);
        ltd.SetupArenaParameters_LettersInWave(arenaSetting_Default.lettersPerWave);
        ltd.SetupArenaParameters_TimeBetweenWaves(arenaSetting_Default.timeBetweenWaves);
        ltd.SetupArenaParameters_LettersToIgnore(arenaSetting_Default.lettersToIgnore);
        ltd.SetupArenaParameters_LettersAsMisty(arenaSetting_Default.percentageOfLettersAsMisty);
        ltd.SetupArenaParameters_MaxLettersOnBoard(arenaSetting_Default.maxLettersOnBoard);

        wbd_Enemy.SetupArenaParameters_PowerModifierForWordCount(arenaSetting_Default.powerModifierForWordCount);
        wbd_Player.SetupArenaParameters_PowerModifierForWordCount(arenaSetting_Default.powerModifierForWordCount);
        enemyMemory.SetupArenaParameters_AllowRepeatWords(arenaSetting_Default.shouldNotCountIfRepeatingWord);
        playerMemory.SetupArenaParameters_AllowRepeatWords(arenaSetting_Default.shouldNotCountIfRepeatingWord);
        wwz_Player.SetupArenaParameters_AbilityToAutoIgnite(arenaSetting_Default.abilityToAutoIgnite);
        wwz_Enemy.SetupArenaParameters_AbilityToAutoIgnite(arenaSetting_Default.abilityToAutoIgnite);

        hm.ResetHealthBars();
        Sprite playerSprite = wbd_Player.GetComponent<WordbuilderData>().GetMugShot();
        Sprite enemySprite = wbd_Enemy.GetComponent<WordbuilderData>().GetMugShot();
        hm.SetHealthBarIcons(playerSprite, enemySprite);

        switch (arenaSetting.aso)
        {

            case ArenaSettingOptions.Blizzard:
                ltd.SetupArenaParameters_LettersInWave(arenaSetting.lettersPerWave);
                ltd.SetupArenaParameters_TimeBetweenWaves(arenaSetting.timeBetweenWaves);
                ltd.SetupArenaParameters_MaxLettersOnBoard(arenaSetting.maxLettersOnBoard);
                wwz_Player.SetupArenaParameters_AbilityToAutoIgnite(arenaSetting.abilityToAutoIgnite);
                wwz_Enemy.SetupArenaParameters_AbilityToAutoIgnite(arenaSetting.abilityToAutoIgnite);
                return;

            case ArenaSettingOptions.Graveyard:
                wbd_Enemy.SetupArenaParameters_PowerModifierForWordCount(arenaSetting.powerModifierForWordCount);
                wbd_Player.SetupArenaParameters_PowerModifierForWordCount(arenaSetting.powerModifierForWordCount);
                enemyMemory.SetupArenaParameters_AllowRepeatWords(arenaSetting.shouldNotCountIfRepeatingWord);
                playerMemory.SetupArenaParameters_AllowRepeatWords(arenaSetting.shouldNotCountIfRepeatingWord);
                //wwz_Enemy.SetupArenaParameters_EnergyRegenRate(arenaSetting.energyRegenRateModifier);
                //wwz_Player.SetupArenaParameters_EnergyRegenRate(arenaSetting.energyRegenRateModifier);
                return;

            case ArenaSettingOptions.Jungle:
                //wwz_Enemy.SetupArenaParameters_EnergyRegenRate(arenaSetting.energyRegenRateModifier);
                //wwz_Player.SetupArenaParameters_EnergyRegenRate(arenaSetting.energyRegenRateModifier);
                wbd_Enemy.SetupArenaParameters_PowerModifierForWordCount(arenaSetting.powerModifierForWordCount);
                wbd_Player.SetupArenaParameters_PowerModifierForWordCount(arenaSetting.powerModifierForWordCount);
                wbd_Enemy.SetupArenaParameters_MaxLettersInWord(arenaSetting.maxWordLength);
                wbd_Player.SetupArenaParameters_MaxLettersInWord(arenaSetting.maxWordLength);
                return;

            case ArenaSettingOptions.Mists:

                return;

            case ArenaSettingOptions.Sandy:
                ltd.SetupArenaParameters_LettersInWave(arenaSetting.lettersPerWave);
                ltd.SetupArenaParameters_Lifetime(arenaSetting.letterLifetime);
                return;

            case ArenaSettingOptions.Training:
                ltd.SetupArenaParameters_LettersInWave(arenaSetting.lettersPerWave);
                ltd.SetupArenaParameters_TimeBetweenWaves(arenaSetting.timeBetweenWaves);
                ltd.SetupArenaParameters_MaxLettersOnBoard(arenaSetting.maxLettersOnBoard);
                ltd.SetupArenaParameters_LettersToIgnore(arenaSetting.lettersToIgnore);
                wwz_Player.SetupArenaParameters_EnergyRegenRate(arenaSetting.energyRegenRateModifier);
                tutorDM = GameObject.FindGameObjectWithTag("Tutor").GetComponent<TutorDialogManager>();
                tutorDM.SetupTutorDM(wwz_Player);
                return;

        }
    }

}
